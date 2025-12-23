# Telegram Bot

Environment variables (set in project root `.env` or compose):

- `BOT_TOKEN` — required
- `POSTGRES_URL` — required
- `WEBAPP_URL` — optional, defaults to `http://localhost:5173`

Build and run with docker-compose (project root):

```bash
docker-compose up --build bot
```
