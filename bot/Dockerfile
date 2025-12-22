FROM python:3.11-slim AS builder
WORKDIR /app

RUN apt-get update \
    && apt-get install -y build-essential --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --upgrade pip \
    && pip wheel -r requirements.txt -w /wheels

FROM python:3.11-slim
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgcc1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=builder /wheels /wheels
RUN pip install --no-cache-dir /wheels/*

COPY . .
ENV PYTHONUNBUFFERED=1

CMD ["python", "main.py"]
