from aiogram.fsm.state import StatesGroup, State


class EditSong(StatesGroup):
    menu = State()
    roles = State()
