import uvicorn


formatter = {
    "()": "uvicorn.logging.ColourizedFormatter",
    "fmt": "[%(name)s] %(asctime)s %(levelprefix)s %(message)s",
    "datefmt": "%Y-%m-%d %H:%M:%S",
    "use_colors": True
}

logging_config = uvicorn.config.LOGGING_CONFIG

logging_config["loggers"]["bot.bot"] = {
    "handlers": ["bot"],
    "level": "INFO",
}

logging_config['formatters']['bot'] = formatter

logging_config['handlers']['bot'] = {
    'formatter': 'bot',
    'class': 'logging.StreamHandler',
    'stream': 'ext://sys.stdout'
}
