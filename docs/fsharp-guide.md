# F# Language Quick Reference

## Key Patterns for Career Story Builder

F# is the primary language for this project. The SAR story domain benefits from:
- **Discriminated unions** for modeling story components
- **Records** for immutable data structures
- **Result type** for validation and error handling
- **Pattern matching** for processing different story states

## Domain Examples

### SAR Story Domain Types

Reference: `fsharp#domain-modeling-records`, `fsharp#records-unions`

```fsharp
// Core domain types for career stories
// Single-case DUs with .Value members for type safety and convenient extraction

type StoryTitle = private StoryTitle of string with
    member this.Value = match this with StoryTitle s -> s

type StorySituation = StorySituation of string with
    member this.Value = match this with StorySituation s -> s

type StoryAction = StoryAction of string with
    member this.Value = match this with StoryAction a -> a

type StoryResult = StoryResult of string with
    member this.Value = match this with StoryResult r -> r

type Story = {
    Title: StoryTitle
    Situation: StorySituation
    Action: StoryAction
    Result: StoryResult
}

// Usage
let story : Story = {
    Title = StoryTitle "Led migration project"
    Situation = StorySituation "Legacy system needed modernization"
    Action = StoryAction "Designed migration strategy with rollback plan"
    Result = StoryResult "Zero downtime, 40% performance improvement"
}

// Access values via .Value member
let title = story.Title.Value
```

### Story State with Discriminated Unions

Reference: `fsharp#discriminated-unions`, `fsharp#pattern-matching`

```fsharp
// Story editing workflow states
type StoryDraft =
    | Empty
    | HasSituation of StorySituation
    | HasAction of StorySituation * StoryAction
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
    else Ok (StoryTitle title)

let validateSituation (situation: StorySituation) =
    if String.IsNullOrWhiteSpace situation.Value then Error (FieldEmpty "situation")
    else Ok situation

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
