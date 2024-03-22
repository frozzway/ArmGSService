from typing import Literal

from sqlmodel import SQLModel, Field
from armgs.models.utils import form_data

parseModes = Literal['HTML', 'MarkdownV2']


class Dto(SQLModel):
    """Базовый объект передачи данных"""
    token: str = Field(description='Токен бота')
    chatId: str = Field(description='Уникальный ник, email пользователя или id чата')


class TextDto(Dto):
    """Объект передачи данных для отправки сообщения в чат"""
    text: str = Field(description='Текст сообщения. Можно упомянуть пользователя, добавив в текст его userId в следующем формате @[userId].')
    parseMode: parseModes | None = Field(description='Режим обработки форматирования из текста сообщения', default=None)


@form_data
class FileTextDto(Dto):
    """Объект передачи данных для отправки файла в чат"""
    caption: str | None = None
    parseMode: parseModes | None = Field(description='Режим обработки форматирования из текста сообщения', default=None)


class BaseVm(SQLModel):
    """Ответ с API"""
    ok: bool = Field(description='Статус запроса')
    description: str | None = Field(description='Описание ошибки')


class MessageVm(BaseVm):
    """Ответ с API на команду отправки сообщения"""
    msgId: str | None = Field(description='Идентификатор сообщения')


class TokenValidationVm(BaseVm):
    """Ответ с API на команду валидации токена"""
    userId: str = Field(description='Уникальный идентификатор бота')
    nick: str = Field(description='Уникальный ник бота')
    firstName: str = Field(description='Имя бота')
    about: str | None = Field(description='Описание бота')


class PermissionBase(SQLModel):
    """Полученные разрешения на отправку сообщений ботами"""
    chatId: str = Field(description='Уникальный ник, email пользователя или id чата')
    nick: str = Field(description='Уникальный ник бота')


class Permission(PermissionBase, table=True):
    id: int | None = Field(default=None, primary_key=True)


class PermissionVm(PermissionBase):
    """Модель отображения сущности 'Полученные разрешения на отправку сообщений ботами'"""
    hasPermission: bool = Field(description="Наличие разрешения на отправку сообщений")
