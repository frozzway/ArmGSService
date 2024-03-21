from bot.bot import Bot, Event
from bot.event import EventType
from bot.handler import MessageHandler

from armgs.database import SessionMaker
from armgs.settings import bots_tokens, settings
from armgs.models.models import Permission


bots = [Bot(token=token, api_url_base=settings.api_url, name=nick, timeout_s=60, poll_time_s=10)
            for token, nick in bots_tokens.items()]


def start_polling():
    for bot in bots:
        bot.dispatcher.add_handler(MessageHandler(callback=message_cb))
        bot.start_polling()


def stop_polling():
    for bot in bots:
        bot.stop()


def message_cb(bot: Bot, event: Event):
    session = SessionMaker()
    if event.type != EventType.NEW_MESSAGE:
        return

    permission = \
        session.query(Permission) \
        .where(Permission.chatId == event.from_chat, Permission.nick == bot.name) \
        .scalar()

    if not permission:
        permission = Permission(chatId=event.from_chat, nick=bot.name)
        session.add(permission)
        session.commit()