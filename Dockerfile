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
