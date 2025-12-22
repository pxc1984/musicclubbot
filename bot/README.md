# Telegram Bot

Simple Telegram bot using aiogram. On `/start` it replies with a link and a button to open the project's webapp.

Environment variables (set in project root `.env` or compose):
- `TELEGRAM_TOKEN` — required
- `WEBAPP_URL` — optional, defaults to `http://localhost:5173`

Build and run with docker-compose (project root):

```bash
docker-compose up --build telegram-bot
```
