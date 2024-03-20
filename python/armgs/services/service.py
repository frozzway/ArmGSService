from fastapi import UploadFile

from armgs.models import TextDto, MessageVm, TokenValidationVm, FileTextDto
from armgs.services.deps import SessionDp, HttpClientDp
from armgs.settings import settings


class ArmGSService:
    def __init__(self, session: SessionDp, client: HttpClientDp) -> None:
        self.session = session
        self.client = client

    async def send_text(self, dto: TextDto) -> MessageVm:
        response = await self.client.get(f'{settings.api_url}/messages/sendText', params=dto.model_dump(exclude_none=True))
        return MessageVm.model_construct(**response.json())

    async def validate_token(self, token: str) -> TokenValidationVm:
        response = await self.client.get(f'{settings.api_url}/self/get?token={token}')
        return TokenValidationVm.model_construct(**response.json())

    async def send_file(self, dto: FileTextDto, file: UploadFile) -> MessageVm:
        files = {'file': (file.filename, await file.read(), 'application/octet-stream')}
        response = await self.client.post(f'{settings.api_url}/messages/sendFile', params=dto.model_dump(exclude_none=True), files=files)
        return MessageVm.model_construct(**response.json())
