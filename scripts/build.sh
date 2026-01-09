#!/usr/bin/env bash
set -e

cd "$(dirname "$0")/.."

case "${1:-dev}" in
  dev)
    echo "Building development image..."
    docker compose build
    ;;
  prod)
    echo "Building production image..."
    docker compose -f docker-compose.prod.yml build
    ;;
  *)
    echo "Usage: $0 [dev|prod]"
    echo "  dev  - Build development image (default)"
    echo "  prod - Build production image"
    exit 1
    ;;
esac

echo "Build complete."
