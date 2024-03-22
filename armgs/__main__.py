import uvicorn

from armgs.settings import settings
from armgs.logging_config import logging_config


uvicorn.run(
    'armgs:app',
    host=settings.server_host,
    port=settings.server_port,
    reload=True,
    log_level='info',
    log_config=logging_config
)
