#!/bin/bash
set -e

cd "$(dirname "$0")/.."

# Detect if Docker daemon is remote (tcp:// or ssh://)
is_docker_remote() {
  local context
  context=$(docker context inspect --format '{{.Endpoints.docker.Host}}' 2>/dev/null || echo "")
  [[ "$context" =~ ^(tcp|ssh):// ]]
}

if is_docker_remote; then
  echo "Remote Docker detected - volume mounts disabled (no hot reload)" >&2
  COMPOSE_CMD="docker compose -f docker-compose.yml -f docker-compose.remote.yml"
else
  COMPOSE_CMD="docker compose"
fi

case "${1:-dev}" in
  dev)
    echo "Starting development environment..."
    $COMPOSE_CMD up
    ;;
  prod)
    echo "Starting production environment..."
    docker compose -f docker-compose.prod.yml up -d
    echo "Production started in background. Use 'docker compose -f docker-compose.prod.yml logs -f' to view logs."
    ;;
  stop)
    echo "Stopping all containers..."
    docker compose down 2>/dev/null || true
    docker compose -f docker-compose.yml -f docker-compose.remote.yml down 2>/dev/null || true
    docker compose -f docker-compose.prod.yml down 2>/dev/null || true
    ;;
  logs)
    $COMPOSE_CMD logs -f
    ;;
  *)
    echo "Usage: $0 [dev|prod|stop|logs]"
    echo "  dev  - Start development environment (default)"
    echo "  prod - Start production environment (detached)"
    echo "  stop - Stop all containers"
    echo "  logs - Follow development logs"
    exit 1
    ;;
esac
