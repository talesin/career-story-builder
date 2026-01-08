# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Career Story Builder is an AI-assisted application for writing SAR-format career stories (Situation, Action, Result). Built with F# for both frontend and backend using Bolero (F# for Blazor) and ASP.NET Core.

## Technology Stack

- **Frontend**: Bolero (F# MVU-style framework on Blazor WebAssembly)
- **Backend**: ASP.NET Core with F#
- **Data Access**: Dapper + Dapper.FSharp (SQL-first, not ORM)
- **Testing**: bUnit (component testing), Expecto (unit/integration)
- **Containerization**: Docker with docker-compose

## Build Commands

```bash
# Development (with hot reload)
docker compose up

# Production build
docker compose build

# Run tests
dotnet test

# Single test (Expecto)
dotnet test --filter "FullyQualifiedName~TestName"
```

## Architecture

### Communication Model
- Async-first: All API calls and database operations are non-blocking
- Frontend uses HTTP APIs (JSON) with shared F# DTOs between client/server
- SignalR available for real-time updates

### Project Structure (Planned)
- **Shared library**: Domain types, DTOs, validation logic (compiles to both server and WASM)
- **Server**: ASP.NET Core APIs, authentication, database access
- **Client**: Bolero MVU components

## F# Style Guidelines

Follow the patterns in `docs/fsharp-style-guide.md`. Key points:

- Use `_.Property` shorthand for simple lambdas: `items |> List.filter _.IsActive`
- Use `Option.defaultValue`, `Option.contains`, `Option.iter` over pattern matching for simple cases
- Name Result-returning functions with `try` prefix: `tryGetPerson`, `tryValidateEmail`
- Avoid mutation; use `fold`, `scan`, higher-order functions instead of mutable loops
- Use single-case DUs to prevent primitive obsession: `type EmailAddress = EmailAddress of string`
- Model states explicitly with discriminated unions, not boolean flags
- Use `task { }` for .NET interop, `and!` for concurrent operations (F# 10)
- Stroustrup record formatting style (opening brace on same line)

## Reference Documentation

All guides reference materials in `~/Documents/Code/_references/`. See `docs/` for project-specific guides:

| Guide                 | Purpose                            |
| --------------------- | ---------------------------------- |
| fsharp-guide.md       | Domain modeling, F# patterns       |
| fsharp-style-guide.md | Coding conventions (comprehensive) |
| bolero-guide.md       | Frontend MVU architecture          |
| blazor-guide.md       | Blazor component model             |
| aspnet-guide.md       | API endpoints, middleware          |
| data-access-guide.md  | Dapper patterns                    |
| testing-guide.md      | bUnit + Expecto                    |
| docker-guide.md       | Container configuration            |

## Domain Model

Core domain is SAR stories with:
- Stories linked to Roles and Projects in employment history
- AI-assisted iterative refinement workflow
- Per-section and overall quality scoring
- Draft/in-review/complete status tracking
- Free-form tagging system

See `docs/user-stories.md` for feature requirements and `docs/delivery-plan.md` for phased implementation.
