import os
import sys
import asyncio

from aiogram import Bot, Dispatcher, F
from aiogram.types import Message, InlineKeyboardMarkup, InlineKeyboardButton
from aiogram.filters import Command
from aiogram.utils.i18n import I18n, gettext as _
from aiogram.utils.i18n.middleware import I18nMiddleware


BOT_TOKEN = os.getenv("BOT_TOKEN")
WEBAPP_URL = os.getenv("WEBAPP_URL", "http://localhost:5173")

if not BOT_TOKEN:
    print("Error: BOT_TOKEN environment variable is not set.")
    sys.exit(1)


# ---- i18n setup ----
i18n = I18n(
    path="locales",
    default_locale="en",
    domain="bot",
)

class MyI18nMiddleware(I18nMiddleware):
    async def get_locale(self, event, data):
        # Telegram user language (e.g. "en", "ru")
        user = data.get("event_from_user")
        return user.language_code if user and user.language_code else "en"


# ---- bot & dispatcher ----
bot = Bot(token=BOT_TOKEN)
dp = Dispatcher()

dp.message.middleware(MyI18nMiddleware(i18n))


# ---- handlers ----
@dp.message(Command("start"))
async def cmd_start(message: Message):
    keyboard = InlineKeyboardMarkup(
        inline_keyboard=[
            [
                InlineKeyboardButton(
                    text=_("Open webapp"),
                    url=WEBAPP_URL,
                )
            ]
        ]
    )

    await message.answer(
        _("Welcome! Click the button below to open the webapp:\n{url}").format(
            url=WEBAPP_URL
        ),
        reply_markup=keyboard,
    )


@dp.message(Command("help"))
async def cmd_help(message: Message):
    await message.answer(
        _("Send /start to get the webapp link.")
    )


# ---- entrypoint ----
async def main():
    await dp.start_polling(bot)


if __name__ == "__main__":
    asyncio.run(main())
