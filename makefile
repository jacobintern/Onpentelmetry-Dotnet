PROJECT_NAME=opentelemetry-dotnet

up:
	@echo "🚀 Starting containers..."
	docker compose up -d --build

down:
	@echo "🧹 Stopping & removing containers, images, volumes..."
	docker compose down --rmi all --volumes --remove-orphans

rebuild:
	docker compose down -v --remove-orphans
	docker compose build --no-cache
	docker compose up -d

logs:
	@echo "📜 Showing API container logs..."
	docker logs -f dotnet-otel-api

ps:
	@docker compose ps