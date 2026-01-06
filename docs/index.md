# Documentation Index

## Project Documentation

| Document | Purpose |
| -------- | ------- |
| [User Stories](user-stories.md) | Feature requirements organized by domain |
| [Delivery Plan](delivery-plan.md) | Phased implementation roadmap with MOSCOW prioritization |
| [Implementation Plan](implementation-plan.md) | Detailed technical implementation steps for each phase |
| [Data Types](data-types.md) | Core F# domain types and interfaces |
| [Technology](technology.md) | Technical architecture and stack decisions |

## Reference Guides

| Layer        | Technology             | Guide                                             |
| ------------ | ---------------------- | ------------------------------------------------- |
| Language     | F#                     | [F# Guide](fsharp-guide.md)                       |
| Language     | F#                     | [F# Style Guide](fsharp-style-guide.md)           |
| Frontend     | Bolero (F# for Blazor) | [Bolero Guide](bolero-guide.md)                   |
| UI Framework | Blazor                 | [Blazor Guide](blazor-guide.md)                   |
| Backend      | ASP.NET Core           | [ASP.NET Guide](aspnet-guide.md)                  |
| Data Access  | Dapper + Dapper.FSharp | [Data Access Guide](data-access-guide.md)         |
| Testing      | bUnit + Expecto        | [Testing Guide](testing-guide.md)                 |
| Containers   | Docker                 | [Docker Guide](docker-guide.md)                   |
| Design       | Functional Patterns    | [Design Patterns Guide](design-patterns-guide.md) |
| Advanced F#  | FSharpPlus             | [FSharpPlus Guide](fsharpplus-guide.md)           |

## Quick Start by Task

| Task                      | Guide(s)                                                         |
| ------------------------- | ---------------------------------------------------------------- |
| Define STAR domain types  | [F# Guide](fsharp-guide.md#domain-modeling)                      |
| Create story editor UI    | [Bolero Guide](bolero-guide.md), [Blazor Guide](blazor-guide.md) |
| Build API endpoints       | [ASP.NET Guide](aspnet-guide.md)                                 |
| Store stories in database | [Data Access Guide](data-access-guide.md)                        |
| Validate story input      | [FSharpPlus Guide](fsharpplus-guide.md#validation)               |
| Test components           | [Testing Guide](testing-guide.md)                                |
| Containerize application  | [Docker Guide](docker-guide.md)                                  |

## External Reference Mapping

The guides in this directory use bare `tech#anchor` tags (e.g., `fsharp#railway-oriented`, `bolero#mvu-architecture`) that reference external documentation topics in a separate repository. Always use the bare `tech#anchor` form without any `$REFERENCES/` path prefix.

### Purpose of External References

External references serve as:
- **Navigation aids** - "Quick Links by Task" tables for deeper reading
- **Authority citations** - Pointing to canonical documentation
- **Extended learning** - For when someone needs more than the guide provides

### Self-Sufficiency of Local Guides

**These guides are fully usable without external references.** Each guide contains substantial domain-specific examples, complete working code snippets, decision tables, and anti-pattern warnings. The external references are "nice to have" for deeper learning but not required to understand and apply the patterns.

### Configuration

If `.references.md` exists in the project root, read it for path configuration, reference sources, and usage rules.

### Recreating External References

If setting up an external reference repository:

1. Create an index file per technology (e.g., `fsharp/index.md`, `bolero/index.md`)
2. Use anchors matching the `#anchor` portion of tags in local guides
3. Structure each index with sections that the local guides can link to
4. Map topic prefixes to paths (e.g., `fsharp#` → `fsharp/index.md#`)
