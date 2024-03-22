from contextlib import asynccontextmanager

from fastapi import FastAPI
from sqlmodel import SQLModel

from armgs.api import router
from armgs.database import async_engine as engine
from armgs.polling import start_polling, stop_polling


@asynccontextmanager
async def lifespan(app: FastAPI):
    """Жизненный цикл микросервиса"""
    async with engine.begin() as conn:
        await conn.run_sync(SQLModel.metadata.create_all)
    start_polling()
    yield
    stop_polling()


app = FastAPI(title='Сервис по взаимодействию с ArmGS', lifespan=lifespan)
app.include_router(router)
