from fastapi import FastAPI
from armgs.api import router


app = FastAPI(title='Сервис по взаимодействию с ArmGS')

app.router.include_router(router)
