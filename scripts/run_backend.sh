#!/bin/bash

set -o allexport
source .env
cd backend
go run ./cmd/server
