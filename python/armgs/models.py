import inspect
from typing import Literal, Type, Annotated

from pydantic import Field, BaseModel
from fastapi import Form


parseModes = Literal['HTML', 'MarkdownV2']


def form_data(cls: Type[BaseModel]):
    new_parameters = []

    for field_name, model_field in cls.model_fields.items():
        new_parameters.append(
             inspect.Parameter(
                 model_field.alias if model_field.alias else field_name,
                 inspect.Parameter.POSITIONAL_ONLY,
                 default=model_field.default,
                 annotation=Annotated[model_field.annotation, Form()]
             )
         )

    async def as_form_func(**data):
        return cls(**data)

    sig = inspect.signature(as_form_func)
    sig = sig.replace(parameters=new_parameters)
    as_form_func.__signature__ = sig
    setattr(cls, 'as_form', as_form_func)
    return cls


class Dto(BaseModel):
    token: str = Field(description='Токен бота')
    chatId: str = Field(description='Уникальный ник, email пользователя или id чата')


class TextDto(Dto):
    text: str = Field(description='Текст сообщения. Можно упомянуть пользователя, добавив в текст его userId в следующем формате @[userId].')
    parseMode: parseModes | None = Field(description='Режим обработки форматирования из текста сообщения', default=None)


@form_data
class FileTextDto(Dto):
    caption: str | None = None
    parseMode: parseModes | None = Field(description='Режим обработки форматирования из текста сообщения', default=None)


class BaseVm(BaseModel):
    ok: bool = Field(description='Статус запроса')
    description: str | None = Field(description='Описание ошибки')


class MessageVm(BaseVm):
    msgId: str | None = Field(description='Идентификатор сообщения')


class TokenValidationVm(BaseVm):
    userId: str = Field(description='Уникальный идентификатор бота')
    nick: str = Field(description='Уникальный ник бота')
    firstName: str = Field(description='Имя бота')
    about: str | None = Field(description='Описание бота')
