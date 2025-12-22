all: help

help:
	@echo "Makefile commands:"
	@echo "  all          - Show this help message"
	@echo "  generate     - Generate TypeScript code from .proto files"

generate:
	@cd frontend && npx buf generate --clean
	@echo "Generated TypeScript code from .proto files."
