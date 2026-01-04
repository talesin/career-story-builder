# F# Language Quick Reference

## Quick Links by Task

| Task                           | Topic                             |
| ------------------------------ | --------------------------------- |
| Define domain types            | fsharp#domain-modeling-records    |
| Use discriminated unions       | fsharp#discriminated-unions       |
| Pattern matching               | fsharp#pattern-matching           |
| Handle errors with Result      | fsharp#rich-domains               |
| Railway-oriented programming   | fsharp#railway-oriented           |
| Async programming              | fsharp#async-programming          |
| Collections (List, Array, Seq) | fsharp#collections                |
| Computation expressions        | fsharp#computation-expressions    |
| Testing with Expecto           | fsharp#testing-expecto            |
| Code style                     | fsharp#style-guide                |

## Key Patterns for Career Story Builder

F# is the primary language for this project. The STAR story domain benefits from:
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

### STAR Story Domain Types

```fsharp
// Core domain types for career stories
type StoryId = StoryId of Guid

type Situation = {
    Context: string
    When: DateOnly option
    Where: string option
}

type Task = {
    Challenge: string
    Responsibility: string
    Stakeholders: string list
}

type Action = {
    Step: int
    Description: string
    Skills: string list
}

type Result = {
    Outcome: string
    Impact: string option
    Metrics: string option
}

type Story = {
    Id: StoryId
    Title: string
    Situation: Situation
    Task: Task
    Actions: Action list
    Result: Result
    Tags: string list
    CreatedAt: DateTimeOffset
    UpdatedAt: DateTimeOffset
}
```

### Story State with Discriminated Unions

```fsharp
// Story editing workflow states
type StoryDraft =
    | Empty
    | HasSituation of Situation
    | HasTask of Situation * Task
    | HasActions of Situation * Task * Action list
    | Complete of Story

// Story validation result
type StoryValidation =
    | Valid of Story
    | Invalid of ValidationError list

and ValidationError =
    | MissingSituation
    | MissingTask
    | NoActions
    | MissingResult
    | TitleTooShort of minLength: int
    | TitleTooLong of maxLength: int
```

### Result-Based Validation

```fsharp
// Railway-oriented validation
let validateTitle (title: string) : Result<string, ValidationError> =
    if String.IsNullOrWhiteSpace title then
        Error (TitleTooShort 1)
    elif title.Length > 200 then
        Error (TitleTooLong 200)
    else
        Ok title

let validateSituation (situation: Situation) : Result<Situation, ValidationError> =
    if String.IsNullOrWhiteSpace situation.Context then
        Error MissingSituation
    else
        Ok situation

let validateActions (actions: Action list) : Result<Action list, ValidationError> =
    if List.isEmpty actions then
        Error NoActions
    else
        Ok actions

// Compose validations using Result.bind
let validateStory (draft: StoryDraft) : Result<Story, ValidationError list> =
    // See FSharpPlus guide for error accumulation
    ...
```

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
