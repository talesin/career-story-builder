# F# Language Quick Reference

## Quick Links by Task

| Task                           | Topic                          |
| ------------------------------ | ------------------------------ |
| Define domain types            | fsharp#domain-modeling-records |
| Use discriminated unions       | fsharp#discriminated-unions    |
| Pattern matching               | fsharp#pattern-matching        |
| Handle errors with Result      | fsharp#rich-domains            |
| Railway-oriented programming   | fsharp#railway-oriented        |
| Async programming              | fsharp#async-programming       |
| Collections (List, Array, Seq) | fsharp#collections             |
| Computation expressions        | fsharp#computation-expressions |
| Testing with Expecto           | fsharp#testing-expecto         |
| Code style                     | fsharp#style-guide             |

## Key Patterns for Career Story Builder

F# is the primary language for this project. The Star story domain benefits from:
- **Discriminated unions** for modeling story components
- **Records** for immutable data structures
- **Result type** for validation and error handling
- **Pattern matching** for processing different story states

## Primary References

### Domain Modeling
- **Records and DUs**: `fsharp#records-unions`
  - Record syntax and copy-and-update
  - Anonymous records
  - Single-case discriminated unions for type safety

- **Rich Domain Building**: `fsharp#rich-domains`
  - Option type for optional fields
  - Result type for validation
  - Making illegal states unrepresentable

### Error Handling
- **Railway-Oriented Programming**: `fsharp#railway-oriented`
  - bind, map, and composition
  - Error track handling
  - Validation pipelines

### Async and I/O
- **Task-based Async**: `fsharp#async-programming`
  - `task { }` computation expressions
  - Parallel vs sequential execution

## Domain Examples

### Star Story Domain Types

```fsharp
// Core domain types for career stories
// Wrapped in Star module to avoid collisions with System.Task and F# Result type

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

Follow the F# style guide: `fsharp#style-guide`

Key conventions:
- Use `camelCase` for values and functions
- Use `PascalCase` for types, modules, and DU cases
- Prefer `|>` pipeline over nested function calls
- Keep functions small and focused
