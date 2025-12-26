# Career Story Builder - Reference Guides

Quick reference guides linking to `$REFERENCES` for the Career Story Builder project.

## Technology Stack

| Layer | Technology | Guide |
|-------|------------|-------|
| Language | F# | [F# Guide](fsharp-guide.md) |
| Frontend | Bolero (F# for Blazor) | [Bolero Guide](bolero-guide.md) |
| UI Framework | Blazor | [Blazor Guide](blazor-guide.md) |
| Backend | ASP.NET Core | [ASP.NET Guide](aspnet-guide.md) |
| Data Access | Dapper + Dapper.FSharp | [Data Access Guide](data-access-guide.md) |
| Testing | bUnit + Expecto | [Testing Guide](testing-guide.md) |
| Containers | Docker | [Docker Guide](docker-guide.md) |
| Design | Functional Patterns | [Design Patterns Guide](design-patterns-guide.md) |
| Advanced F# | FSharpPlus | [FSharpPlus Guide](fsharpplus-guide.md) |

## STAR Story Domain

This application helps professionals write career stories using the STAR format:

- **S**ituation - Context and background
- **T**ask - The challenge or responsibility
- **A**ction - Steps taken to address the task
- **R**esult - Outcomes and impact

## Reference Location

All guides reference materials in the `$REFERENCES` environment variable:
```
$REFERENCES = ~/Documents/Code/_references/
```

## Quick Start by Task

| Task | Guide(s) |
|------|----------|
| Define STAR domain types | [F# Guide](fsharp-guide.md#domain-modeling) |
| Create story editor UI | [Bolero Guide](bolero-guide.md), [Blazor Guide](blazor-guide.md) |
| Build API endpoints | [ASP.NET Guide](aspnet-guide.md) |
| Store stories in database | [Data Access Guide](data-access-guide.md) |
| Validate story input | [FSharpPlus Guide](fsharpplus-guide.md#validation) |
| Test components | [Testing Guide](testing-guide.md) |
| Containerize application | [Docker Guide](docker-guide.md) |
