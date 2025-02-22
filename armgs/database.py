from sqlalchemy import create_engine
from sqlalchemy.ext.asyncio import create_async_engine
from sqlalchemy.engine import URL
from sqlalchemy.ext.asyncio import async_sessionmaker
from sqlalchemy.orm import sessionmaker

from armgs.settings import settings


url_params = {
    'username': settings.db_username,
    'password': settings.db_password,
    'host': settings.db_host,
    'port': settings.db_port,
    'database': settings.db_database
}

url_object = URL.create(
    'postgresql+psycopg2',
    **url_params
)

async_url_object = URL.create(
    'postgresql+asyncpg',
    **url_params
)

engine = create_engine(url_object)
async_engine = create_async_engine(async_url_object)

SessionMaker = sessionmaker(engine)
AsyncSessionMaker = async_sessionmaker(async_engine, expire_on_commit=False)
