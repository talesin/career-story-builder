# Docker Container Development Guide

> References: `$REFERENCES/docker/`

## Quick Links by Task

| Task | Reference |
|------|-----------|
| Docker concepts | `$REFERENCES/docker/index.md#introduction` |
| Install Docker | `$REFERENCES/docker/index.md#installing` |
| Write Dockerfiles | `$REFERENCES/docker/index.md#images` |
| Multi-stage builds | `$REFERENCES/docker/index.md#multi-stage` |
| Run containers | `$REFERENCES/docker/index.md#containers` |
| View logs | `$REFERENCES/docker/index.md#exploring` |
| Debug containers | `$REFERENCES/docker/index.md#debugging` |
| Docker Compose | `$REFERENCES/docker/index.md#compose` |
| Production deployment | `$REFERENCES/docker/index.md#production` |
| Security | `$REFERENCES/docker/index.md#advanced` |

## Key Patterns for Career Story Builder

Docker provides:
- **Development container**: Hot reload with `dotnet watch`
- **Production container**: Optimized multi-stage build
- **Compose**: Full stack with database and app

## Primary References

### Dockerfile Best Practices
- **Multi-Stage Builds**: `$REFERENCES/docker/index.md#images`
  - Build stage with SDK
  - Production stage with runtime only

### Docker Compose
- **Service Configuration**: `$REFERENCES/docker/index.md#compose`
  - Service definitions
  - Networks and volumes
  - Environment variables

## Domain Examples

### Development Dockerfile

```dockerfile
# Development Dockerfile - enables hot reload
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev

WORKDIR /app

# Install development tools
RUN dotnet tool install -g dotnet-watch

# Copy solution and project files first (layer caching)
COPY *.sln ./
COPY src/CareerStoryBuilder.Server/*.fsproj ./src/CareerStoryBuilder.Server/
COPY src/CareerStoryBuilder.Client/*.fsproj ./src/CareerStoryBuilder.Client/
COPY src/CareerStoryBuilder.Shared/*.fsproj ./src/CareerStoryBuilder.Shared/

# Restore dependencies
RUN dotnet restore

# Source code mounted as volume at runtime
# COPY . .  (not needed - using volume mount)

EXPOSE 5000 5001

# Use dotnet watch for hot reload
ENTRYPOINT ["dotnet", "watch", "--project", "src/CareerStoryBuilder.Server"]
```

### Production Dockerfile (Multi-Stage)

```dockerfile
# ========================================
# Stage 1: Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files and restore
COPY *.sln ./
COPY src/CareerStoryBuilder.Server/*.fsproj ./src/CareerStoryBuilder.Server/
COPY src/CareerStoryBuilder.Client/*.fsproj ./src/CareerStoryBuilder.Client/
COPY src/CareerStoryBuilder.Shared/*.fsproj ./src/CareerStoryBuilder.Shared/

RUN dotnet restore

# Copy source and build
COPY . .
RUN dotnet publish src/CareerStoryBuilder.Server \
    -c Release \
    -o /app/publish \
    --no-restore

# ========================================
# Stage 2: Production Runtime
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Security: Run as non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "CareerStoryBuilder.Server.dll"]
```

### Docker Compose (Development)

```yaml
# docker-compose.yml
version: "3.8"

services:
  app:
    build:
      context: .
      dockerfile: Dockerfile.dev
    ports:
      - "5000:5000"
      - "5001:5001"
    volumes:
      # Mount source for hot reload
      - .:/app
      # Preserve nuget packages
      - nuget-cache:/root/.nuget/packages
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000;https://+:5001
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

### Docker Compose (Production)

```yaml
# docker-compose.prod.yml
version: "3.8"

services:
  app:
    image: career-story-builder:${TAG:-latest}
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=${DATABASE_URL}
    depends_on:
      db:
        condition: service_healthy
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

  db:
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=career_stories
      - POSTGRES_USER=${DB_USER}
      - POSTGRES_PASSWORD=${DB_PASSWORD}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    restart: unless-stopped
    deploy:
      resources:
        limits:
          memory: 256M
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${DB_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  postgres-data:
```

### Common Commands

```bash
# Development
docker compose up              # Start all services
docker compose up -d           # Start in background
docker compose logs -f app     # Follow app logs
docker compose down            # Stop all services
docker compose down -v         # Stop and remove volumes

# Build
docker compose build           # Build all images
docker compose build --no-cache app  # Rebuild without cache

# Production
docker compose -f docker-compose.prod.yml up -d
docker compose -f docker-compose.prod.yml logs -f

# Debug
docker compose exec app /bin/sh    # Shell into running container
docker compose ps                  # List running containers
docker compose top                 # Show running processes
```

### .dockerignore

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

# Local development
*.local
.env.local

# Documentation
docs/
*.md

# Tests
tests/
**/*Tests/
```

### Environment Variables

```bash
# .env (development - DO NOT COMMIT)
DATABASE_URL=Host=db;Database=career_stories;Username=postgres;Password=devpassword
JWT_SECRET=dev-secret-key-change-in-production
ASPNETCORE_ENVIRONMENT=Development

# .env.production (template - DO NOT COMMIT SECRETS)
DATABASE_URL=Host=prod-db;Database=career_stories;Username=app;Password=${DB_PASSWORD}
JWT_SECRET=${JWT_SECRET}
ASPNETCORE_ENVIRONMENT=Production
```

## Production Checklist

From `$REFERENCES/docker/index.md#production`:

- [ ] Resource limits configured (memory, CPU)
- [ ] Log rotation enabled (`max-size`, `max-file`)
- [ ] Health checks defined
- [ ] Restart policy set (`unless-stopped`)
- [ ] Non-root user in container
- [ ] Secrets managed securely (not in image)
- [ ] Monitoring configured
- [ ] Backup strategy for database volume

## Anti-Patterns to Avoid

| Anti-Pattern | Problem | Solution |
|--------------|---------|----------|
| `latest` tag | Unpredictable deployments | Use specific version tags |
| Root user | Security vulnerability | Use `USER` directive |
| No resource limits | Runaway containers | Set `--memory` and `--cpus` |
| Secrets in images | Security risk | Use env vars or secrets |
| No health checks | Silent failures | Add `HEALTHCHECK` |
| Single massive layer | Slow rebuilds | Order for layer caching |
