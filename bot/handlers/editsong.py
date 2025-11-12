import logging

from aiogram import Router
from aiogram.enums import ContentType
from aiogram.types import User, CallbackQuery, Message
from aiogram_dialog import Dialog, Window, DialogManager
from aiogram_dialog.widgets.input import MessageInput
from aiogram_dialog.widgets.text import Const, Format
from aiogram_dialog.widgets.kbd import Button, Row, Column, Cancel, Url
from aiogram_dialog.widgets.kbd import ScrollingGroup, Select
from sqlalchemy import select

from bot.models import Song
from bot.services.database import get_db_session
from bot.services.settings import settings
from bot.services.url import parse_url
from bot.states.editsong import EditSong

router = Router()
logger = logging.getLogger(__name__)

async def song_info_getter(dialog_manager: DialogManager, **kwargs) -> dict:
    async with get_db_session() as session:
        result = await session.execute(select(Song).where(Song.id == int(dialog_manager.start_data["song_id"])).limit(1))
        song = result.scalar_one_or_none()
    return {
        "song_id": dialog_manager.start_data["song_id"],
        "song_title": song.title,
        "song_link": song.link,
    }

router.include_router(Dialog(
    Window(
        Format("ID: {song_id}\nНазвание: {song_title}"),
        Url(Const("Ссылка"), url=Format("{song_link}"), id="song_link"),
        Cancel(Const("Назад")),
        getter=song_info_getter,
        state=EditSong.menu,
    )
))