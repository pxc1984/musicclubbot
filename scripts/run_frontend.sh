#!/bin/bash

set -o allexport
source .env
cd frontend
npm run dev
