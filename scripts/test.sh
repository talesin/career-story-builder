#!/usr/bin/env bash
set -e

cd "$(dirname "$0")/.."

case "${1:-all}" in
  build)
    dotnet build
    ;;
  test)
    dotnet test
    ;;
  all)
    dotnet build
    dotnet test
    ;;
  *)
    echo "Usage: $0 [build|test|all]"
    echo "  build - Build the solution"
    echo "  test  - Run tests only"
    echo "  all   - Build and run tests (default)"
    exit 1
    ;;
esac
