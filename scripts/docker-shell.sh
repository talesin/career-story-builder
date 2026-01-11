#!/usr/bin/env bash
cd "$(dirname "$0")/.."

# Drop into an interactive shell in the container
docker compose run --rm --no-deps --entrypoint "/usr/bin/env bash" -it app
