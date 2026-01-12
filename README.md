# Career Story Builder

An AI supported career story builder, helping professionals write SAR style career stories useful for building resumes, personal talent reviews or promotion documents.

## SAR Story Domain

This application helps professionals write career stories using the SAR format:

- **S**ituation - Context and background
- **A**ction - Steps taken to address the situation
- **R**esult - Outcomes and impact

## Architecture Overview

The application uses **asynchronous communication** between the frontend and backend:

- **Responsive UI** - The frontend remains interactive during data operations, with loading states and optimistic updates where appropriate
- **Non-blocking data access** - All API calls and database operations are async, preventing UI freezes
- **Event-driven updates** - The UI reacts to data changes without requiring full page refreshes

This async-first approach ensures a smooth user experience even during longer operations like AI-assisted story generation.

## Prerequisites

- **Docker** with **Docker Compose** plugin
  - [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Compose)
  - Or [Docker Engine](https://docs.docker.com/engine/install/) + [Compose plugin](https://docs.docker.com/compose/install/)

## Quick Start

**macOS/Linux (bash):**

```bash
./scripts/run.sh        # Start dev server (hot reload)
./scripts/test.sh       # Run tests
./scripts/run.sh stop   # Stop
```

**Windows (PowerShell):**

```powershell
./scripts/run.ps1       # Start dev server (hot reload)
./scripts/test.ps1      # Run tests
./scripts/run.ps1 stop  # Stop
```

Open <http://localhost:8001> in your browser.

## Documentation

See [docs/index.md](docs/index.md) for the full documentation index including:

- Project documentation (user stories, delivery plan, technology decisions)
- Reference guides for each technology layer
- Quick start guide by task
