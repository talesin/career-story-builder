# Docker Container Development Guide

## Key Patterns for Career Story Builder

Docker provides:
- **Development container**: Hot reload with `dotnet watch`
- **Production container**: Optimized multi-stage build
- **Compose**: Service orchestration for app (and database in Phase 3+)

## Project Structure

```
CareerStoryBuilder/
├── .env or .envrc          # Port and environment config (see Configuration)
├── Dockerfile              # Multi-stage (dev, build, runtime)
├── docker-compose.yml      # Development configuration
├── docker-compose.prod.yml # Production configuration
├── docker-compose.remote.yml # Override for remote Docker hosts
├── .dockerignore           # Exclude files from context
└── scripts/
    ├── build.sh / build.ps1    # Build Docker images
    ├── run.sh / run.ps1        # Start/stop containers
    ├── test.sh / test.ps1      # Build and test in container
    └── shell.sh / shell.ps1    # Interactive shell in container
```

## Configuration

Configure the development port using either approach:

**Option 1: `.env` file** (standard Docker Compose)
```bash
# .env
APP_PORT=8001
```

**Option 2: `.envrc` with direnv** (auto-loads when entering directory)
```bash
# .envrc
export APP_PORT=8001
```

If neither file exists, `APP_PORT` defaults to `8001`.

Access the app at `http://localhost:8001`

## Multi-Stage Dockerfile

The single `Dockerfile` contains three stages, selected via `target` in docker-compose:

```dockerfile
# ========================================
# Stage: dev - Hot reload development
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev

WORKDIR /app

# Copy project files first for layer caching
COPY *.sln global.json Directory.Build.props ./
COPY src/Server/*.fsproj ./src/Server/
COPY src/Client/*.fsproj ./src/Client/
COPY src/Shared/*.fsproj ./src/Shared/
COPY tests/Server.Tests/*.fsproj ./tests/Server.Tests/
COPY tests/Client.Tests/*.fsproj ./tests/Client.Tests/

# Restore dependencies
RUN dotnet restore

# Source code mounted as volume at runtime (port configured via APP_PORT in .env)
EXPOSE 8001

# Use dotnet watch for rebuild on changes
# Note: --no-hot-reload because Blazor WASM hot reload has issues in containers
ENTRYPOINT ["dotnet", "watch", "--project", "src/Server", "--no-hot-reload"]

# ========================================
# Stage: build - Compile for production
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy everything and build
COPY . .
RUN dotnet publish src/Server -c Release -o /app/publish --no-restore || \
    (dotnet restore && dotnet publish src/Server -c Release -o /app/publish)

# ========================================
# Stage: runtime - Production
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Security: Run as non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "Server.dll"]
```

## Docker Compose (Development)

```yaml
# docker-compose.yml
services:
  app:
    build:
      context: .
      target: dev
    ports:
      - "${APP_PORT:-8001}:${APP_PORT:-8001}"
    volumes:
      # Mount source for hot reload
      - .:/app
      # Preserve nuget packages between rebuilds
      - nuget-cache:/root/.nuget/packages
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://0.0.0.0:${APP_PORT:-8001}

volumes:
  nuget-cache:
```

## Docker Compose (Production)

```yaml
# docker-compose.prod.yml
services:
  app:
    image: career-story-builder:${TAG:-latest}
    build:
      context: .
      target: runtime
    ports:
      - "80:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: "1.0"
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

## Docker Compose (Remote)

When using a remote Docker host (e.g., Docker context pointing to a remote machine), local volume mounts don't work. Use the remote override file:

```yaml
# docker-compose.remote.yml
services:
  app:
    volumes: !reset
      # Only keep named volumes, remove local path mounts
      - nuget-cache:/root/.nuget/packages
```

The `run.sh` script auto-detects remote Docker and applies this override. For manual usage:

```bash
docker compose -f docker-compose.yml -f docker-compose.remote.yml up
```

Note: Without local volume mounts, changes require rebuilding the image (`docker compose build`).

## Common Commands

```bash
# Development
./scripts/docker-run.sh              # Start dev environment
./scripts/docker-run.sh stop         # Stop all containers
./scripts/docker-run.sh logs         # Follow dev logs

# Build and Test
./scripts/docker-build.sh            # Build dev image
./scripts/docker-build.sh prod       # Build production image
./scripts/docker-test.sh             # Build and run all tests
./scripts/docker-test.sh build       # Build only
./scripts/docker-test.sh test        # Test only
./scripts/docker-shell.sh            # Interactive shell in container

# Direct docker compose
docker compose up              # Start dev services
docker compose up -d           # Start in background
docker compose logs -f app     # Follow app logs
docker compose down            # Stop all services
docker compose down -v         # Stop and remove volumes

# Production
docker compose -f docker-compose.prod.yml up -d
docker compose -f docker-compose.prod.yml logs -f

# Debug
docker compose exec app /bin/sh    # Shell into running container
docker compose ps                  # List running containers
docker compose top                 # Show running processes
```

## .dockerignore

```
# Build artifacts
bin/
obj/
publish/

# IDE
.vs/
.vscode/
.idea/
*.user

# Git
.git/
.gitignore

# Docker
Dockerfile*
docker-compose*
.dockerignore

# Local development
*.local
.env.local

# Documentation
docs/
*.md

# Tests (exclude source, keep project files for restore)
tests/**/*.fs
tests/**/bin/
tests/**/obj/

# Temporary files
temp_*
```

## Phase 3+: Adding PostgreSQL

When database support is added in Phase 3, extend docker-compose.yml:

```yaml
# docker-compose.yml (Phase 3+)
services:
  app:
    build:
      context: .
      target: dev
    ports:
      - "${APP_PORT:-8001}:${APP_PORT:-8001}"
    volumes:
      - .:/app
      - nuget-cache:/root/.nuget/packages
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://0.0.0.0:${APP_PORT:-8001}
      - ConnectionStrings__Default=Host=db;Database=career_stories;Username=postgres;Password=devpassword
    depends_on:
      - db
    networks:
      - career-network

  db:
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=career_stories
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=devpassword
    volumes:
      - postgres-data:/var/lib/postgresql/data
      - ./scripts/init-db.sql:/docker-entrypoint-initdb.d/init.sql
    ports:
      - "5432:5432"
    networks:
      - career-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  postgres-data:
  nuget-cache:

networks:
  career-network:
```

## Environment Variables

```bash
# .env or .envrc (safe defaults only)
APP_PORT=8001

# .env.local (not committed - secrets go here, Phase 2+)
# DATABASE_URL=Host=db;Database=career_stories;Username=postgres;Password=devpassword
# JWT_SECRET=dev-secret-key-change-in-production
```

## Production Checklist

- [x] Resource limits configured (memory, CPU)
- [x] Log rotation enabled (`max-size`, `max-file`)
- [x] Health checks defined
- [x] Restart policy set (`unless-stopped`)
- [x] Non-root user in container
- [ ] Secrets managed securely (not in image) - Phase 2+
- [ ] Monitoring configured - Future
- [ ] Backup strategy for database volume - Phase 3+

## Anti-Patterns to Avoid

| Anti-Pattern         | Problem                   | Solution                    |
| -------------------- | ------------------------- | --------------------------- |
| `latest` tag         | Unpredictable deployments | Use specific version tags   |
| Root user            | Security vulnerability    | Use `USER` directive        |
| No resource limits   | Runaway containers        | Set `--memory` and `--cpus` |
| Secrets in images    | Security risk             | Use env vars or secrets     |
| No health checks     | Silent failures           | Add `HEALTHCHECK`           |
| Single massive layer | Slow rebuilds             | Order for layer caching     |
