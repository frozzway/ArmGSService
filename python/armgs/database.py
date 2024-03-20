from sqlalchemy.ext.asyncio import create_async_engine
from sqlalchemy.engine import URL
from sqlalchemy.ext.asyncio import async_sessionmaker

from armgs.settings import settings


url_object = URL.create(
    settings.db_dialect,
    username=settings.db_username,
    password=settings.db_password,
    host=settings.db_host,
    port=settings.db_port,
    database=settings.db_database,
)

engine = create_async_engine(url_object)

Session = async_sessionmaker(engine, expire_on_commit=False)

