# FSharpPlus Advanced Patterns Guide

> References: `$REFERENCES/fsharpplus/`

## Quick Links by Task

| Task                            | Reference                                                 |
| ------------------------------- | --------------------------------------------------------- |
| Getting started                 | `$REFERENCES/fsharpplus/index.md#getting-started`         |
| Generic operators               | `$REFERENCES/fsharpplus/index.md#operators`               |
| Abstractions (Functor, Monad)   | `$REFERENCES/fsharpplus/index.md#abstractions`            |
| Computation expressions         | `$REFERENCES/fsharpplus/index.md#computation-expressions` |
| Validation (error accumulation) | `$REFERENCES/fsharpplus/index.md#validation`              |
| Reader monad (DI)               | `$REFERENCES/fsharpplus/index.md#reader-monad`            |
| Lenses for nested updates       | `$REFERENCES/fsharpplus/index.md#lens`                    |
| Monad transformers              | `$REFERENCES/fsharpplus/index.md#monad-transformers`      |

## When to Use FSharpPlus

This project uses FSharpPlus for advanced functional patterns. Consider FSharpPlus when you need:

| Need                             | FSharpPlus Solution       | Alternative              |
| -------------------------------- | ------------------------- | ------------------------ |
| Accumulate all validation errors | `Validation<'Errors, 'T>` | Roll your own error list |
| Convert Option to Result         | `Option.toResultWith`     | N/A in standard F#       |
| Generic map/bind across types    | `map`, `bind` operators   | Type-specific functions  |
| Deeply nested record updates     | Lenses (`^.`, `.->`)      | Manual copy-and-update   |
| Dependency injection pattern     | Reader monad              | Function parameters      |

**Default to FSharpPlus** for utilities like `Option.toResultWith` rather than defining custom helpers. This maintains consistency and leverages a well-tested library.

**For simple cases**, standard F# `Result` with `result { }` CE is sufficient when you only need fail-fast behavior.

## Key Patterns for Career Story Builder

FSharpPlus provides advanced functional patterns:
- **Validation**: Accumulate ALL validation errors (vs Result which short-circuits)
- **Operators**: Generic `map`, `bind`, `apply` work across types
- **Lenses**: Update deeply nested story structures
- **Reader**: Dependency injection pattern

## Primary References

### Validation
- **Error Accumulation**: `$REFERENCES/fsharpplus/index.md#data-types`
  - `Validation<'Error, 'T>` vs `Result<'T, 'Error>`
  - `Success` and `Failure` cases
  - Applicative style with `<!>` and `<*>`

### Operators
- **Generic Functions**: `$REFERENCES/fsharpplus/index.md#operators`
  - `map` (`<!>`, `<<|`, `|>>`)
  - `apply` (`<*>`)
  - `bind` (`>>=`, `=<<`)
  - Kleisli composition (`>=>`, `<=<`)

### Lenses
- **Optics**: `$REFERENCES/fsharpplus/index.md#lens`
  - View with `^.`
  - Set with `.->`
  - Update with `%->`

## Domain Examples

### Validation with Error Accumulation

```fsharp
open FSharpPlus
open FSharpPlus.Data

// Validation accumulates ALL errors, unlike Result which short-circuits

type StoryError =
    | TitleEmpty
    | TitleTooLong of max: int
    | SituationEmpty
    | TaskEmpty
    | NoActions
    | ResultEmpty
    | InvalidTag of string

type StoryErrors = StoryError list

// Individual validators return Validation
let validateTitle (title: string) : Validation<StoryErrors, string> =
    if String.IsNullOrWhiteSpace title then
        Failure [TitleEmpty]
    elif title.Length > 200 then
        Failure [TitleTooLong 200]
    else
        Success title

let validateSituation (situation: Situation) : Validation<StoryErrors, Situation> =
    if String.IsNullOrWhiteSpace situation.Context then
        Failure [SituationEmpty]
    else
        Success situation

let validateTask (task: Task) : Validation<StoryErrors, Task> =
    if String.IsNullOrWhiteSpace task.Challenge then
        Failure [TaskEmpty]
    else
        Success task

let validateActions (actions: Action list) : Validation<StoryErrors, Action list> =
    if List.isEmpty actions then
        Failure [NoActions]
    else
        Success actions

let validateResult (result: StoryResult) : Validation<StoryErrors, StoryResult> =
    if String.IsNullOrWhiteSpace result.Outcome then
        Failure [ResultEmpty]
    else
        Success result

let validateTags (tags: string list) : Validation<StoryErrors, string list> =
    let invalidTags = tags |> List.filter (fun t -> t.Length > 50)
    if List.isEmpty invalidTags then
        Success tags
    else
        Failure (invalidTags |> List.map InvalidTag)

// Combine validations - accumulates ALL errors
let validateStory (draft: StoryDraft) : Validation<StoryErrors, Story> =
    let createStory title situation task actions result tags =
        { Id = StoryId (Guid.NewGuid())
          Title = title
          Situation = situation
          Task = task
          Actions = actions
          Result = result
          Tags = tags
          CreatedAt = DateTimeOffset.UtcNow
          UpdatedAt = DateTimeOffset.UtcNow }

    // Applicative style - all validations run, errors accumulate
    createStory
    <!> validateTitle draft.Title
    <*> validateSituation draft.Situation
    <*> validateTask draft.Task
    <*> validateActions draft.Actions
    <*> validateResult draft.Result
    <*> validateTags draft.Tags

// Usage
let result = validateStory {
    Title = ""  // Error: TitleEmpty
    Situation = { Context = ""; When = None; Where = None }  // Error: SituationEmpty
    Task = { Challenge = ""; Responsibility = ""; Stakeholders = [] }  // Error: TaskEmpty
    Actions = []  // Error: NoActions
    Result = { Outcome = ""; Impact = None; Metrics = None }  // Error: ResultEmpty
    Tags = []
}

// result = Failure [TitleEmpty; SituationEmpty; TaskEmpty; NoActions; ResultEmpty]
// ALL errors collected, not just the first one!
```

### Generic Operators

```fsharp
open FSharpPlus

// map works on any Functor (Option, Result, List, Async, etc.)
let storyTitles = stories |> map (fun s -> s.Title)

// Same as List.map for lists
let titles1: string list = map (_.Title) stories

// Same as Option.map for options
let title2: string option = map (_.Title) (Some story)

// Same as Result.map for results
let title3: Result<string, _> = map (_.Title) (Ok story)

// bind works on any Monad
let findAndValidate id =
    findStory id           // returns Option<Story>
    >>= validateStory      // returns Validation<_, Story>

// Kleisli composition for monadic pipelines
let loadAndProcess: Guid -> Async<Result<ProcessedStory, Error>> =
    loadFromDb >=> validate >=> enrich >=> save
```

### Lenses for Nested Updates

```fsharp
open FSharpPlus
open FSharpPlus.Lens

// Define lenses for Story structure
let inline _situation f story =
    f story.Situation |> map (fun s -> { story with Situation = s })

let inline _context f situation =
    f situation.Context |> map (fun c -> { situation with Context = c })

let inline _task f story =
    f story.Task |> map (fun t -> { story with Task = t })

let inline _challenge f task =
    f task.Challenge |> map (fun c -> { task with Challenge = c })

let inline _result f story =
    f story.Result |> map (fun r -> { story with Result = r })

let inline _outcome f result =
    f result.Outcome |> map (fun o -> { result with Outcome = o })

// Compose lenses
let _situationContext = _situation << _context
let _taskChallenge = _task << _challenge
let _resultOutcome = _result << _outcome

// Read nested values
let context = story ^. _situationContext
let challenge = story ^. _taskChallenge

// Update nested values immutably
let updatedStory =
    story
    |> setl _situationContext "New context for the situation"
    |> setl _taskChallenge "Updated challenge description"
    |> over _resultOutcome String.toUpper  // Transform value

// Practical example: update story with user input
let handleSituationUpdate newContext story =
    story |> setl _situationContext newContext

let handleTaskUpdate newChallenge story =
    story |> setl _taskChallenge newChallenge
```

### Reader Monad for Dependency Injection

```fsharp
open FSharpPlus
open FSharpPlus.Data

// Dependencies as a record
type StoryDependencies = {
    Repository: IStoryRepository
    Validator: Story -> Validation<StoryErrors, Story>
    Logger: ILogger
}

// Operations that depend on environment
let getStory (id: StoryId) : Reader<StoryDependencies, Task<Story option>> =
    Reader (fun deps -> deps.Repository.GetById id)

let saveStory (story: Story) : Reader<StoryDependencies, Task<unit>> =
    Reader (fun deps -> task {
        deps.Logger.LogInformation("Saving story {Id}", story.Id)
        do! deps.Repository.Save story
    })

let validateAndSave (draft: StoryDraft) : Reader<StoryDependencies, Task<Validation<StoryErrors, Story>>> =
    Reader (fun deps -> task {
        match deps.Validator (toStory draft) with
        | Success story ->
            do! deps.Repository.Save story
            return Success story
        | Failure errors ->
            deps.Logger.LogWarning("Validation failed: {Errors}", errors)
            return Failure errors
    })

// Compose Reader operations
let createStoryWorkflow draft = monad {
    let! validated = validateAndSave draft
    match validated with
    | Success story ->
        return! saveStory story |> Reader.map (fun _ -> Success story)
    | Failure errors ->
        return Failure errors
}

// Run with dependencies
let runWorkflow deps =
    createStoryWorkflow someDraft
    |> Reader.run deps
```

### Computation Expression Examples

```fsharp
open FSharpPlus

// Generic monad CE works with any monad
let optionWorkflow = monad {
    let! story = findStory storyId
    let! user = findUser story.UserId
    return { Story = story; Author = user }
}

// Applicative CE for parallel/independent operations
let fetchAllData = applicative {
    let! stories = loadStories ()
    and! users = loadUsers ()
    and! tags = loadTags ()
    return { Stories = stories; Users = users; Tags = tags }
}

// Validation with applicative
let validatedStory = applicative {
    let! title = validateTitle draft.Title
    and! situation = validateSituation draft.Situation
    and! task = validateTask draft.Task
    and! actions = validateActions draft.Actions
    and! result = validateResult draft.Result
    return createStory title situation task actions result
}
```

## Pattern Comparison

| Need                | Result          | Validation       | When to Use                 |
| ------------------- | --------------- | ---------------- | --------------------------- |
| Fail on first error | Yes             | No               | Parse, IO operations        |
| Collect all errors  | No              | Yes              | Form validation, user input |
| Performance         | Better          | Slightly slower  | Most cases: Result          |
| User feedback       | Shows one error | Shows all errors | Forms: Validation           |

## Quick Setup

```fsharp
// In your .fsproj
<PackageReference Include="FSharpPlus" Version="*" />

// In your code
open FSharpPlus          // Generic operators
open FSharpPlus.Data     // Validation, Reader, etc.
open FSharpPlus.Lens     // Optics (when needed)
```
