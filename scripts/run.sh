#!/bin/bash
set -e

cd "$(dirname "$0")/.."

case "${1:-dev}" in
  dev)
    echo "Starting development environment..."
    docker compose up
    ;;
  prod)
    echo "Starting production environment..."
    docker compose -f docker-compose.prod.yml up -d
    echo "Production started in background. Use 'docker compose -f docker-compose.prod.yml logs -f' to view logs."
    ;;
  down)
    echo "Stopping all containers..."
    docker compose down
    docker compose -f docker-compose.prod.yml down 2>/dev/null || true
    ;;
  logs)
    docker compose logs -f
    ;;
  *)
    echo "Usage: $0 [dev|prod|down|logs]"
    echo "  dev  - Start development environment (default)"
    echo "  prod - Start production environment (detached)"
    echo "  down - Stop all containers"
    echo "  logs - Follow development logs"
    exit 1
    ;;
esac
