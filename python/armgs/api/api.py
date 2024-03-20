from typing import Annotated

from fastapi import APIRouter, Depends, UploadFile

from armgs.models import TokenValidationVm, MessageVm, BaseVm, TextDto, FileTextDto
from armgs.services import ArmGSService


router = APIRouter()

ArmGSServiceDp = Annotated[ArmGSService, Depends(ArmGSService)]
FileDto = Annotated[FileTextDto, Depends(FileTextDto.as_form)]


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
