# Career Story Builder

An AI supported career story builder, helping professionals write STAR style career stories useful for building resumes, personal talent reviews or promotion documents.

## STAR Story Domain

This application helps professionals write career stories using the STAR format:

- **S**ituation - Context and background
- **T**ask - The challenge or responsibility
- **A**ction - Steps taken to address the task
- **R**esult - Outcomes and impact

## Architecture Overview

The application uses **asynchronous communication** between the frontend and backend:

- **Responsive UI** - The frontend remains interactive during data operations, with loading states and optimistic updates where appropriate
- **Non-blocking data access** - All API calls and database operations are async, preventing UI freezes
- **Event-driven updates** - The UI reacts to data changes without requiring full page refreshes

This async-first approach ensures a smooth user experience even during longer operations like AI-assisted story generation.

## Documentation

- [User Stories](docs/user-stories.md) - Feature requirements organized by domain
- [Delivery Plan](docs/delivery-plan.md) - Phased implementation roadmap
- [Technology](docs/technology.md) - Technical architecture and stack decisions

## Reference Guides

| Layer        | Technology             | Guide                                                  |
| ------------ | ---------------------- | ------------------------------------------------------ |
| Language     | F#                     | [F# Guide](docs/fsharp-guide.md)                       |
| Language     | F#                     | [F# Style Guide](docs/fsharp-style-guide.md)           |
| Frontend     | Bolero (F# for Blazor) | [Bolero Guide](docs/bolero-guide.md)                   |
| UI Framework | Blazor                 | [Blazor Guide](docs/blazor-guide.md)                   |
| Backend      | ASP.NET Core           | [ASP.NET Guide](docs/aspnet-guide.md)                  |
| Data Access  | Dapper + Dapper.FSharp | [Data Access Guide](docs/data-access-guide.md)         |
| Testing      | bUnit + Expecto        | [Testing Guide](docs/testing-guide.md)                 |
| Containers   | Docker                 | [Docker Guide](docs/docker-guide.md)                   |
| Design       | Functional Patterns    | [Design Patterns Guide](docs/design-patterns-guide.md) |
| Advanced F#  | FSharpPlus             | [FSharpPlus Guide](docs/fsharpplus-guide.md)           |

## Quick Start by Task

| Task                      | Guide(s)                                                                   |
| ------------------------- | -------------------------------------------------------------------------- |
| Define STAR domain types  | [F# Guide](docs/fsharp-guide.md#domain-modeling)                           |
| Create story editor UI    | [Bolero Guide](docs/bolero-guide.md), [Blazor Guide](docs/blazor-guide.md) |
| Build API endpoints       | [ASP.NET Guide](docs/aspnet-guide.md)                                      |
| Store stories in database | [Data Access Guide](docs/data-access-guide.md)                             |
| Validate story input      | [FSharpPlus Guide](docs/fsharpplus-guide.md#validation)                    |
| Test components           | [Testing Guide](docs/testing-guide.md)                                     |
| Containerize application  | [Docker Guide](docs/docker-guide.md)                                       |

## Reference Location

All guides reference materials in the `$REFERENCES` environment variable:

```
$REFERENCES = ~/Documents/Code/_references/
```
