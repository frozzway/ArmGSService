import inspect
from typing import Type, Annotated

from pydantic import BaseModel
from fastapi import Form


def form_data(cls: Type[BaseModel]):
    """Функция-декоратор для преобразования класса модели Pydantic в form-data"""
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
