import uvicorn

from armgs.settings import settings


uvicorn.run(
    'armgs:app',
    host=settings.server_host,
    port=settings.server_port,
    reload=True,
)
