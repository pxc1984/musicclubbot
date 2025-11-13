import logging

from aiogram import Router
from aiogram.enums import ContentType
from aiogram.types import User, CallbackQuery, Message
from aiogram_dialog import Dialog, Window, DialogManager
from aiogram_dialog.widgets.input import MessageInput
from aiogram_dialog.widgets.text import Const, Format
from aiogram_dialog.widgets.kbd import Button, Row, Column, Cancel, Url
from aiogram_dialog.widgets.kbd import ScrollingGroup, Select
from sqlalchemy import select, delete
from sqlalchemy.orm.sync import update
from sqlalchemy.orm import selectinload


from bot.models import Song, SongParticipation, Person
from bot.services.database import get_db_session
from bot.services.strings import is_valid_title
from bot.services.settings import settings
from bot.services.songparticipation import song_participation_list_out
from bot.services.url import parse_url
from bot.states.adminpanel import AdminPanel
from bot.states.createevent import CreateEvent
from bot.states.participations import MyParticipations

router = Router()

async def admin_panel_getter(dialog_manager: DialogManager, event_from_user: User, **kwargs):
    if event_from_user.id not in settings.ADMIN_IDS:
        await dialog_manager.done()
    return {}

router.include_router(
    Dialog(
        Window(
            Const("Админ панель"),
            Button(
                Const("Создать мероприятие"),
                id="create",
                on_click=lambda c, b, m: m.start(
                    CreateEvent.title, data={"started_id": c.from_user.id}
                ),
            ),
            Cancel(Const("Назад")),
            getter=admin_panel_getter,
            state=AdminPanel.menu,
        )
    )
)
