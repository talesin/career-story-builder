# Copilot / AI Agent Instructions (minimal)

This file contains Copilot/GitHub-AI-specific guidance only. For full, authoritative repository rules and workflows, read `CLAUDE.md`.

Key Copilot guidance:

- Read `CLAUDE.md` first and follow its instructions; this file is intentionally minimal.
- Keep changes small and focused: prefer incremental edits and small PRs.
- After making changes, run a local compile and tests:

```bash
dotnet build
dotnet test
```

- When changing shared DTOs/types, update both the `Client` and `Server` projects and ensure the shared project compiles for both targets.
- For development, use the docker workflow:

```bash
docker compose up    # dev/hot-reload environment
docker compose build # production build
```

- Follow repository F# conventions (single-case DUs, `try` prefix for Result-returning functions, `task { }` + `and!` for async). See `CLAUDE.md` and `docs/fsharp-style-guide.md` if needed.
- Dapper is used for data access; prefer SQL-first patterns from `docs/data-access-guide.md` (do not introduce an ORM).
- PR checklist (minimal): small scope, update docs/tests, `dotnet build` passes, run relevant tests, ensure shared DTOs compile.

If you want this file expanded to include code snippets (Dapper template, `task { }` example, or PR checklist with file links), tell me which snippet to add.
