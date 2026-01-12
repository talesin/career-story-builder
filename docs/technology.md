# Technology

## Operations

We will use docker and containers for both local development (running a local server in watch mode) and to build a production ready container that holds the web site and API.

### Docker Strategy

#### Development Container

- Mount source code as a volume to enable live editing
- Run `dotnet watch` inside the container for hot reload
- Expose ports for both the application and any debugging tools
- Use a development-specific Dockerfile or compose profile that includes SDK tooling
- Environment variables configure connection strings, API keys, and debug settings

#### Production Container

- Multi-stage build: first stage compiles with the .NET SDK, second stage runs with the slim ASP.NET runtime
- Publish as a self-contained or framework-dependent app depending on size/startup tradeoffs
- Final image based on `mcr.microsoft.com/dotnet/aspnet` for minimal footprint
- No SDK, source code, or development dependencies in the final image
- Health checks configured for orchestrator readiness/liveness probes

#### Compose Configuration

- `docker-compose.yml` defines services for the app, database, and any supporting infrastructure (Redis, message queues)
- Separate `docker-compose.override.yml` or profiles for development vs production settings
- Named volumes for database persistence across container restarts
- Network isolation between services

#### Typical Workflow

1. `docker compose up` starts the full local environment
2. Code changes trigger hot reload via mounted volumes and `dotnet watch`
3. `docker compose build` creates production images
4. Images tagged and pushed to a container registry for deployment

## F# Backend + Frontend using Blazor (Tech Stack Overview)

### Core platform

- ASP.NET Core
  Hosting layer for HTTP APIs, SignalR hubs, authentication, middleware, and deployment. Supports both Blazor Server and Blazor WebAssembly hosting models.

- Blazor
  Component-based web UI framework running on .NET. Supports WebAssembly (client-side) and Server (remote UI over SignalR).

### F# frontend options

- Bolero
  F#-first framework built on top of Blazor. Provides MVU-style architecture, typed HTML helpers, and F#-friendly abstractions.

- Raw Blazor Components in F#
  Writing standard Blazor components directly in F# without Bolero. Lower abstraction and framework risk, but more verbose and less ergonomic.

### Application models

- Bolero MVU
  Elmish-style Model–View–Update loop layered over Blazor rendering. Centralized state, explicit messages, predictable updates.

- Blazor Component Model
  Stateful components with lifecycle methods and dependency injection. Familiar to Blazor users, less opinionated than MVU.

### Backend communication

- HTTP APIs (JSON)
  Standard request-response APIs using HttpClient from Blazor WASM. Shared F# DTOs compile to both server and client.

- SignalR
  Real-time messaging abstraction over WebSockets, SSE, or long polling. First-class support in Blazor. Suitable for live updates and push-based UI.

### Shared code

- Shared F# Class Library
  Domain types, DTOs, validation logic, and contracts shared between server and client (WASM).

- System.Text.Json + FSharp.SystemTextJson
  Built-in JSON serializer extended with native F# type support. Discriminated unions
  serialize as camelCase strings (e.g., `WorkflowStep.Clarification` → `"clarification"`).
  Configuration in `src/Shared/Json.fs`. See [FSharp.SystemTextJson docs](fsharp-json#union-formats).

### State and effects

- Bolero Commands
  Explicit side-effect handling in MVU-style applications, similar to Elmish commands.

- Dependency Injection (DI)
  Unified DI model across ASP.NET Core and Blazor for services, clients, and infrastructure concerns.

### Authentication and security

- ASP.NET Core Authentication
  Cookie-based auth, JWT, or external identity providers (OIDC, Azure AD, Auth0).

- Blazor Authorization Components
  Declarative authorization in UI using roles and policies.

### Hosting models

- Blazor WebAssembly (Hosted)
  Client runs in the browser; backend serves APIs and static assets. Clear frontend/backend boundary.

- Blazor Server
  UI executes on the server and communicates over SignalR. No frontend deployment, but higher latency sensitivity and scalability considerations.

### Persistence and infrastructure

- Entity Framework Core (F#-friendly usage)
  ORM for relational databases, often wrapped to reduce OO friction in F#.

- Dapper / SQL-first access
  Lightweight data access with explicit SQL and strong performance characteristics.

### Tooling and build

- .NET SDK tooling
  Single build and dependency system for backend and frontend. No Node.js required.

- Blazor Hot Reload
  Supported, but reliability varies in complex F# projects.

### Testing

- xUnit / Expecto
  Unit and integration testing for shared domain logic and backend services.

- bUnit
  Component testing framework for Blazor UI.

- Playwright for .NET
  User journeys, navigation, form flows, accessibility-aware testing

### Operational considerations

- Payload size
  Blazor WebAssembly apps ship the .NET runtime, leading to larger initial downloads than JS SPAs.

- Startup time
  Slower cold-start compared to Fable or TypeScript SPAs; acceptable for authenticated or internal apps.
