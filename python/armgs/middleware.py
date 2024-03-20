from fastapi import Request

from armgs.app import app
from armgs.models import BaseVm


@app.middleware('http')
async def process(request: Request, call_next):
    response = await call_next(request)
    if not response.get('ok'):
        response = BaseVm.model_construct(description=response.get('description'), ok=response.get('ok'))
    return response
