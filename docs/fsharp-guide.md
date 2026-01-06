# F# Language Quick Reference

## Key Patterns for Career Story Builder

F# is the primary language for this project. The STAR story domain benefits from:
- **Discriminated unions** for modeling story components
- **Records** for immutable data structures
- **Result type** for validation and error handling
- **Pattern matching** for processing different story states

> **Naming Convention**: We use "STAR" when referring to the interview methodology (Situation, Task, Action, Result) and "Star" (PascalCase) for F# module/type names to avoid collisions with common types like `System.Threading.Tasks.Task`.

## Domain Examples

### Star Story Domain Types

Reference: `fsharp#domain-modeling-records`, `fsharp#records-unions`

```fsharp
// Core domain types for career stories
//
// NOTE: The Star module wrapper is specific to this domain because STAR acronym
// components (Task, Result) collide with common types (System.Threading.Tasks.Task,
// F#'s Result<'T,'E>). This is NOT a typical F# pattern - normally you'd define
// types at module level without wrapping. We use it here solely to avoid these
// name collisions while keeping the STAR terminology.

module Star =
    type Situation = Situation of string
    type Task = Task of string
    type Action = Action of string
    type Result = Result of string

type Story = {
    Title: string
    Situation: Star.Situation
    Task: Star.Task
    Action: Star.Action
    Result: Star.Result
}

// Usage
let story : Story = {
    Title = "Led migration project"
    Situation = Star.Situation "Legacy system needed modernization"
    Task = Star.Task "Migrate 500k records to new platform"
    Action = Star.Action "Designed migration strategy with rollback plan"
    Result = Star.Result "Zero downtime, 40% performance improvement"
}
```

### Story State with Discriminated Unions

Reference: `fsharp#discriminated-unions`, `fsharp#pattern-matching`

```fsharp
// Story editing workflow states
type StoryDraft =
    | Empty
    | HasSituation of Star.Situation
    | HasTask of Star.Situation * Star.Task
    | HasAction of Star.Situation * Star.Task * Star.Action
    | Complete of Story

// Validation errors
type ValidationError =
    | FieldEmpty of field: string
    | TitleTooLong of maxLength: int
```

### Result-Based Validation

Reference: `fsharp#railway-oriented`, `fsharp#rich-domains`

```fsharp
// Railway-oriented validation (fail-fast)
let validateTitle title =
    if String.IsNullOrWhiteSpace title then Error (FieldEmpty "title")
    elif title.Length > 200 then Error (TitleTooLong 200)
    else Ok title

let validateSituation (Star.Situation s) =
    if String.IsNullOrWhiteSpace s then Error (FieldEmpty "situation")
    else Ok (Star.Situation s)

// Compose validations using Result.bind
let validateStory title situation =
    validateTitle title
    |> Result.bind (fun t ->
        validateSituation situation
        |> Result.map (fun s -> t, s))
```

For error accumulation (collecting all errors), see [FSharpPlus Guide](fsharpplus-guide.md#validation-with-error-accumulation).

### Async Data Loading

Reference: `fsharp#async-programming`, `fsharp#computation-expressions`

```fsharp
// Load story from database
let loadStory (id: StoryId) : Task<Story option> = task {
    let! result = storyRepository.GetById id
    return result
}

// Save story with validation
let saveStory (story: Story) : Task<Result<Story, ValidationError list>> = task {
    match validateStory story with
    | Ok validStory ->
        do! storyRepository.Save validStory
        return Ok validStory
    | Error errors ->
        return Error errors
}
```

## Code Style

Reference: `fsharp#style-guide`

Key conventions:
- Use `camelCase` for values and functions
- Use `PascalCase` for types, modules, and DU cases
- Prefer `|>` pipeline over nested function calls
- Keep functions small and focused

## See Also

- `fsharp#collections` - examples TBD
- `fsharp#testing-expecto` - examples TBD
