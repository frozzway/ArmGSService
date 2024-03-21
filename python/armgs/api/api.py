from typing import Annotated

from fastapi import APIRouter, Depends, UploadFile, Query

from armgs.models import TokenValidationVm, MessageVm, BaseVm, TextDto, FileTextDto, PermissionVm
from armgs.services import ArmGSService

router = APIRouter(tags=['v1'])

ArmGSServiceDp = Annotated[ArmGSService, Depends(ArmGSService)]
FileDto = Annotated[FileTextDto, Depends(FileTextDto.as_form)]
BotNick = Annotated[str, Query(description='Уникальный ник бота')]
ChatId = Annotated[str, Query(description='Уникальный ник, email пользователя или id чата')]


@router.get('/validateToken',
            response_model=TokenValidationVm | BaseVm,
            description='Метод валидации токена')
async def validate_token(token: str, service: ArmGSServiceDp):
    return await service.validate_token(token)


@router.post('/sendText',
             response_model=MessageVm | BaseVm,
             description='Метод отправки сообщения в чат или пользователю')
async def send_text(dto: TextDto, service: ArmGSServiceDp):
    return await service.send_text(dto)


@router.post('/sendFile',
             response_model=MessageVm | BaseVm,
             description='Метод отправки файла с сообщением в чат или пользователю')
async def send_file(dto: FileDto, file: UploadFile, service: ArmGSServiceDp):
    return await service.send_file(dto, file)


@router.get('/checkPermission',
            response_model=PermissionVm | BaseVm,
            description='Метод проверки наличия разрешения на отправку сообщений пользователю')
async def check_permission(nick: BotNick, chatId: ChatId, service: ArmGSServiceDp):
    return await service.check_permission(nick, chatId)


@router.post('/checkPermissions',
            response_model=list[ChatId] | BaseVm,
            description='Метод проверки наличия разрешения на отправку сообщений пользователям')
async def check_permissions(nick: BotNick, chatsId: list[ChatId], service: ArmGSServiceDp):
    return await service.check_permissions(nick, chatsId)
