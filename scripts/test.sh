#!/bin/bash
set -e

cd "$(dirname "$0")/.."

# Run build and/or tests inside the container
# Uses docker compose run with entrypoint override for one-off commands

run_in_container() {
  docker compose run --rm --no-deps --entrypoint "$1" app "${@:2}"
}

case "${1:-all}" in
  build)
    echo "Building in container..."
    run_in_container dotnet build
    ;;
  test)
    echo "Running tests in container..."
    run_in_container dotnet test
    ;;
  all)
    echo "Building in container..."
    run_in_container dotnet build
    echo ""
    echo "Running tests in container..."
    run_in_container dotnet test
    ;;
  *)
    echo "Usage: $0 [build|test|all]"
    echo "  build - Build the solution"
    echo "  test  - Run tests"
    echo "  all   - Build and run tests (default)"
    exit 1
    ;;
esac

echo ""
echo "Done."
