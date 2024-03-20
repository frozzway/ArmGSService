from collections.abc import AsyncGenerator
from typing import Annotated

from httpx import AsyncClient
from fastapi import Depends

from armgs.database import Session


async def get_session() -> AsyncGenerator[Session, None]:
    session = Session()
    await session.begin()
    try:
        yield session
    finally:
        await session.close()


async def get_http_client() -> AsyncGenerator[AsyncClient, None]:
    async with AsyncClient(timeout=20) as client:
        yield client


SessionDp = Annotated[Session, Depends(get_session)]
HttpClientDp = Annotated[AsyncClient, Depends(get_http_client)]
