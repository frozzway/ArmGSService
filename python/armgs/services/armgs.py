from fastapi import UploadFile
from sqlmodel import select, col, delete

from armgs.models import TextDto, MessageVm, TokenValidationVm, FileTextDto, PermissionVm, Permission
from armgs.services.dependencies import AsyncSessionDp, HttpClientDp
from armgs.settings import settings, bots_tokens


class ArmGSService:
    def __init__(self, session: AsyncSessionDp, client: HttpClientDp) -> None:
        self.session = session
        self.client = client

    async def send_text(self, dto: TextDto) -> MessageVm:
        """Метод отправки сообщения в чат или пользователю"""
        response = await self.client.get(f'{settings.api_url}/messages/sendText', params=dto.model_dump(exclude_none=True))
        vm = response.json()
        if vm.get('description') == 'Permission denied':
            await self.erase_permission(dto.token, dto.chatId)
        return MessageVm.model_construct(**vm)

    async def validate_token(self, token: str) -> TokenValidationVm:
        """Метод валидации токена"""
        response = await self.client.get(f'{settings.api_url}/self/get?token={token}')
        return TokenValidationVm.model_construct(**response.json())

    async def send_file(self, dto: FileTextDto, file: UploadFile) -> MessageVm:
        """Метод отправки файла с сообщением в чат или пользователю"""
        files = {'file': (file.filename, await file.read(), 'application/octet-stream')}
        response = await self.client.post(f'{settings.api_url}/messages/sendFile', params=dto.model_dump(exclude_none=True), files=files)
        vm = response.json()
        if vm.get('description') == 'Permission denied':
            await self.erase_permission(dto.token, dto.chatId)
        return MessageVm.model_construct(**vm)

    async def check_permission(self, bot_nick: str, chat_id: str) -> PermissionVm:
        """Метод проверки наличия разрешения на отправку сообщений пользователю"""
        query = select(Permission)\
                .where(Permission.nick == bot_nick,
                       Permission.chatId == chat_id)
        result = (await self.session.execute(query)).scalar()
        return PermissionVm(chatId=chat_id, nick=bot_nick, hasPermission=bool(result))

    async def check_permissions(self, bot_nick: str, chats_id: list[str]) -> list[str]:
        """Метод проверки наличия разрешения на отправку сообщений пользователям"""
        query = select(Permission)\
                .where(Permission.nick == bot_nick,
                       col(Permission.chatId).in_(chats_id))
        result = (await self.session.execute(query)).scalars()
        return [p.chatId for p in result]

    async def erase_permission(self, bot_token: str, chat_id: str) -> None:
        """Удаление информации о наличии разрешения на отправку сообщений пользователю"""
        bot_nick = bots_tokens.get(bot_token)
        if not bot_nick:
            return
        query = delete(Permission).where(Permission.chatId == chat_id, Permission.nick == bot_nick)
        await self.session.execute(query)
        await self.session.commit()
