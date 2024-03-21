from collections.abc import AsyncGenerator
from typing import Annotated

from httpx import AsyncClient
from fastapi import Depends
from sqlalchemy.ext.asyncio import AsyncSession

from armgs.database import AsyncSessionMaker


async def get_async_session() -> AsyncGenerator[AsyncSession, None]:
    session = AsyncSessionMaker()
    await session.begin()
    try:
        yield session
    finally:
        await session.close()


async def get_http_client() -> AsyncGenerator[AsyncClient, None]:
    async with AsyncClient(timeout=20) as client:
        yield client


AsyncSessionDp = Annotated[AsyncSession, Depends(get_async_session)]
HttpClientDp = Annotated[AsyncClient, Depends(get_http_client)]
