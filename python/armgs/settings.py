from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    server_host: str = '0.0.0.0'
    server_port: int = '8080'
    timezone: str = 'Asia/Yekaterinburg'

    db_username: str = 'postgres'
    db_password: str = '123'
    db_host: str = 'localhost'
    db_port: str = '5432'
    db_database: str = 'ArmGSMicroservice'

    api_url: str = 'https://api.armgs.team/bot/v1'


settings = Settings(
    _env_file='.env',
    _env_file_encoding='utf-8',
)


bots_tokens = {
    '001.0138958387.3206533949:89260743436': 'oadbot'
}
