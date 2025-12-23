#!/bin/bash

set -o allexport
source .env
cd bot
source .venv/bin/activate
uv run src/main.py
