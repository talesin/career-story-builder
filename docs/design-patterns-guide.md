# Design Patterns Guide (Functional)

> References: `$REFERENCES/design/`

## Quick Links by Task

### Functional Design Principles
| Task                       | Reference                                         |
| -------------------------- | ------------------------------------------------- |
| Immutability basics        | `$REFERENCES/design/index.md#immutability`        |
| Persistent data structures | `$REFERENCES/design/index.md#persistent-data`     |
| Recursion patterns         | `$REFERENCES/design/index.md#recursion-iteration` |
| Lazy evaluation            | `$REFERENCES/design/index.md#laziness`            |
| State management           | `$REFERENCES/design/index.md#state-management`    |
| Data flow pipelines        | `$REFERENCES/design/index.md#data-flow`           |
| SOLID in functional code   | `$REFERENCES/design/index.md#solid`               |
| Testing patterns           | `$REFERENCES/design/index.md#testing`             |

### Gang of Four (OO Patterns)
| Task                | Reference                                      |
| ------------------- | ---------------------------------------------- |
| Pattern overview    | `$REFERENCES/design/index.md#gof-introduction` |
| Creational patterns | `$REFERENCES/design/index.md#gof-creational`   |
| Structural patterns | `$REFERENCES/design/index.md#gof-structural`   |
| Behavioral patterns | `$REFERENCES/design/index.md#gof-behavioral`   |
| Patterns in FP      | `$REFERENCES/design/index.md#patterns-review`  |

## Key Patterns for Career Story Builder

Functional design principles guide the architecture:
- **Immutability**: All domain types are immutable records
- **Railway-Oriented Programming**: Validation with Result type
- **Data transformation pipelines**: Processing story data
- **SOLID principles**: Applied to F# modules and functions

## Primary References

### Railway-Oriented Programming
- **Error Handling**: `$REFERENCES/design/index.md#data-flow`
  - Result type for validation
  - Bind/map for chaining
  - Error accumulation

### SOLID in F#
- **Functional SOLID**: `$REFERENCES/design/index.md#solid`
  - SRP: Small focused functions
  - OCP: Extend via composition
  - DIP: Depend on abstractions (interfaces/functions)

## Domain Examples

### Railway-Oriented Validation Pipeline

```fsharp
// Railway-Oriented Programming for story validation
// See: $REFERENCES/design/Design-Chapter11-DataFlow.md

module StoryValidation =
    type ValidationError =
        | TitleEmpty
        | TitleTooLong of max: int
        | SituationEmpty
        | TaskEmpty
        | NoActions
        | ResultEmpty

    // Individual validators return Result
    let validateTitle (title: string) : Result<string, ValidationError> =
        if String.IsNullOrWhiteSpace title then
            Error TitleEmpty
        elif title.Length > 200 then
            Error (TitleTooLong 200)
        else
            Ok title

    let validateSituation (situation: Situation) : Result<Situation, ValidationError> =
        if String.IsNullOrWhiteSpace situation.Context then
            Error SituationEmpty
        else
            Ok situation

    let validateTask (task: Task) : Result<Task, ValidationError> =
        if String.IsNullOrWhiteSpace task.Challenge then
            Error TaskEmpty
        else
            Ok task

    let validateActions (actions: Action list) : Result<Action list, ValidationError> =
        if List.isEmpty actions then
            Error NoActions
        else
            Ok actions

    let validateResult (result: Result) : Result<Result, ValidationError> =
        if String.IsNullOrWhiteSpace result.Outcome then
            Error ResultEmpty
        else
            Ok result

    // Compose validators using Result.bind (the "railway")
    let validate (story: StoryDraft) : Result<Story, ValidationError> =
        validateTitle story.Title
        |> Result.bind (fun title ->
            validateSituation story.Situation
            |> Result.bind (fun situation ->
                validateTask story.Task
                |> Result.bind (fun task ->
                    validateActions story.Actions
                    |> Result.bind (fun actions ->
                        validateResult story.Result
                        |> Result.map (fun result ->
                            { Id = StoryId (Guid.NewGuid())
                              Title = title
                              Situation = situation
                              Task = task
                              Actions = actions
                              Result = result
                              Tags = story.Tags
                              CreatedAt = DateTimeOffset.UtcNow
                              UpdatedAt = DateTimeOffset.UtcNow })))))
```

### Alternative: Using result Computation Expression

The same validation logic can be written more clearly using FSharpPlus:

```fsharp
open FSharpPlus

let validate (story: StoryDraft) : Result<Story, ValidationError> =
    result {
        let! title = validateTitle story.Title
        let! situation = validateSituation story.Situation
        let! task = validateTask story.Task
        let! actions = validateActions story.Actions
        let! storyResult = validateResult story.Result
        return { Id = StoryId (Guid.NewGuid())
                 Title = title
                 Situation = situation
                 Task = task
                 Actions = actions
                 Result = storyResult
                 Tags = story.Tags
                 CreatedAt = DateTimeOffset.UtcNow
                 UpdatedAt = DateTimeOffset.UtcNow }
    }
```

**Trade-offs**:
- Nested `bind` chains make the railroad tracks explicit
- Computation expressions are more readable but hide the mechanics
- Both fail on first error; use `Validation` type for error accumulation

> *Note: For form validation where you want to show all errors at once, use FSharpPlus `Validation` type instead of `Result`. See [FSharpPlus Guide](fsharpplus-guide.md#validation-with-error-accumulation).*

### Data Transformation Pipeline

```fsharp
// Functional pipelines for data processing
// See: $REFERENCES/design/Design-Chapter11-DataFlow.md

module StoryProcessing =

    // Transform story for display
    let toDisplayModel (story: Story) : StoryDisplayModel =
        { Title = story.Title
          Summary = generateSummary story
          ActionCount = List.length story.Actions
          Tags = story.Tags
          LastUpdated = story.UpdatedAt.ToString("MMM dd, yyyy") }

    // Generate summary from STAR components
    let generateSummary (story: Story) =
        [ story.Situation.Context
          story.Task.Challenge
          story.Result.Outcome ]
        |> List.filter (not << String.IsNullOrWhiteSpace)
        |> List.map (fun s -> if s.Length > 100 then s.Substring(0, 97) + "..." else s)
        |> String.concat " | "

    // Filter and sort stories
    let filterByTag tag =
        List.filter (fun s -> List.contains tag s.Tags)

    let sortByRecent =
        List.sortByDescending (fun s -> s.UpdatedAt)

    let take n =
        List.truncate n

    // Pipeline composition
    let getRecentStoriesWithTag tag count stories =
        stories
        |> filterByTag tag
        |> sortByRecent
        |> take count
        |> List.map toDisplayModel

    // Alternative: using |> with partial application
    let getRecentStoriesWithTag' tag count =
        filterByTag tag
        >> sortByRecent
        >> take count
        >> List.map toDisplayModel
```

### SOLID in Functional F#

```fsharp
// Single Responsibility Principle
// Each module has one reason to change

module StoryPersistence =
    let save (repo: IStoryRepository) story = repo.Save story
    let load (repo: IStoryRepository) id = repo.GetById id

module StoryValidation =
    let validate story = ...

module StoryFormatting =
    let toMarkdown story = ...
    let toHtml story = ...

// Open-Closed Principle
// Extend behavior without modifying existing code

type StoryExporter = Story -> string

let markdownExporter: StoryExporter = fun story ->
    sprintf "# %s\n\n## Situation\n%s\n..." story.Title story.Situation.Context

let htmlExporter: StoryExporter = fun story ->
    sprintf "<h1>%s</h1><h2>Situation</h2><p>%s</p>..." story.Title story.Situation.Context

let jsonExporter: StoryExporter = fun story ->
    JsonSerializer.Serialize(story)

// Add new exporters without changing existing code
let exportStory (exporter: StoryExporter) story =
    exporter story

// Dependency Inversion Principle
// Depend on abstractions

type IStoryRepository =
    abstract GetById: StoryId -> Task<Story option>
    abstract GetAll: unit -> Task<Story list>
    abstract Save: Story -> Task<unit>
    abstract Delete: StoryId -> Task<unit>

// High-level module depends on abstraction
type StoryService(repo: IStoryRepository, validator: Story -> Result<Story, ValidationError list>) =
    member _.Create(draft: StoryDraft) = task {
        match validator (toStory draft) with
        | Ok story ->
            do! repo.Save story
            return Ok story
        | Error errors ->
            return Error errors
    }
```

### Functional Dependency Injection

In F#, dependency injection is primarily about passing values. Choose the pattern based on complexity.

See: `$REFERENCES/fsharp/index.md#pure-functional`

**Pattern 1: Pass dependencies as parameters**

Use when the dependency list is short and call graph is shallow.

```fsharp
module UserEmail =
    type Clock = unit -> DateTime
    type SendEmail = string -> string -> unit

    let sendWelcomeEmail (clock: Clock) (sendEmail: SendEmail) (toAddr: string) =
        let now = clock()
        sendEmail toAddr $"Welcome! Time: {now:O}"

// Composition root binds dependencies once
module CompositionRoot =
    let clock () = DateTime.UtcNow
    let sendEmail toAddr body = printfn "To=%s Body=%s" toAddr body

    let sendWelcomeEmailBound = UserEmail.sendWelcomeEmail clock sendEmail
```

**Pattern 2: Record of functions (capabilities bundle)**

Use when you have many dependencies or want to avoid long parameter lists.

```fsharp
type UserDeps =
    { Clock: unit -> DateTime
      SendEmail: string -> string -> unit
      LoadUser: int -> string option }

module UserWorkflow =
    let notifyUser (deps: UserDeps) (userId: int) =
        match deps.LoadUser userId with
        | None -> ()
        | Some email ->
            let now = deps.Clock()
            deps.SendEmail email $"Hello. Time: {now:O}"

// Composition root
let deps : UserDeps =
    { Clock = fun () -> DateTime.UtcNow
      SendEmail = fun toAddr body -> printfn "To=%s Body=%s" toAddr body
      LoadUser = fun userId -> if userId = 1 then Some "a@b.com" else None }

let notifyUser = UserWorkflow.notifyUser deps
```

**Decision guide**:

| Scenario                             | Pattern                          |
| ------------------------------------ | -------------------------------- |
| Few dependencies, shallow call graph | Explicit parameters              |
| Many dependencies, want simplicity   | Record of functions              |
| Framework requires DI classes        | Thin wrapper (see ASP.NET Guide) |
| Deep dependency threading is noisy   | Consider Reader style            |

**Guidelines**:
- Keep dependency parameters on the left, business inputs on the right
- Bind dependencies once in the composition root using partial application
- Keep dependency records small per module; avoid one mega-record for the whole app
- Prefer "capabilities" naming over "Service" naming

### State Machine Pattern

```fsharp
// State machine for story editing workflow
// See: $REFERENCES/design/Design-Chapter05-Statefulness.md

type EditingState =
    | Idle
    | EditingSituation of StoryDraft
    | EditingTask of StoryDraft
    | EditingActions of StoryDraft
    | EditingResult of StoryDraft
    | Previewing of Story
    | Saving of Story
    | Saved of Story
    | Error of StoryDraft * ValidationError list

type EditingEvent =
    | StartNew
    | LoadExisting of Story
    | UpdateSituation of Situation
    | UpdateTask of Task
    | AddAction of Action
    | RemoveAction of int
    | UpdateResult of Result
    | Preview
    | Save
    | SaveSucceeded of Story
    | SaveFailed of ValidationError list
    | Cancel

let transition state event =
    match state, event with
    | Idle, StartNew ->
        EditingSituation { empty with Id = StoryId (Guid.NewGuid()) }

    | Idle, LoadExisting story ->
        EditingSituation (toDraft story)

    | EditingSituation draft, UpdateSituation situation ->
        EditingTask { draft with Situation = situation }

    | EditingTask draft, UpdateTask task ->
        EditingActions { draft with Task = task }

    | EditingActions draft, AddAction action ->
        EditingActions { draft with Actions = draft.Actions @ [action] }

    | EditingActions draft, UpdateResult result ->
        EditingResult { draft with Result = result }

    | EditingResult draft, Preview ->
        match validate draft with
        | Ok story -> Previewing story
        | Error errors -> Error (draft, errors)

    | Previewing story, Save ->
        Saving story

    | Saving story, SaveSucceeded saved ->
        Saved saved

    | Saving story, SaveFailed errors ->
        Error (toDraft story, errors)

    | _, Cancel ->
        Idle

    | state, _ ->
        state  // Invalid transition, stay in current state
```

### Command Pattern for Operations

```fsharp
// Command pattern for undo/redo
// See: $REFERENCES/design/DesignPatterns-Chapter05-BehavioralPatterns.md#command

type StoryCommand =
    | SetTitle of string
    | SetSituation of Situation
    | SetTask of Task
    | AddAction of Action
    | RemoveAction of int
    | SetResult of Result
    | AddTag of string
    | RemoveTag of string

type CommandHistory = {
    Past: StoryCommand list
    Future: StoryCommand list
}

let applyCommand (story: StoryDraft) (command: StoryCommand) =
    match command with
    | SetTitle title -> { story with Title = title }
    | SetSituation situation -> { story with Situation = situation }
    | SetTask task -> { story with Task = task }
    | AddAction action -> { story with Actions = story.Actions @ [action] }
    | RemoveAction step -> { story with Actions = story.Actions |> List.filter (fun a -> a.Step <> step) }
    | SetResult result -> { story with Result = result }
    | AddTag tag -> { story with Tags = tag :: story.Tags }
    | RemoveTag tag -> { story with Tags = story.Tags |> List.filter ((<>) tag) }

let executeWithHistory (story: StoryDraft) (history: CommandHistory) (command: StoryCommand) =
    let newStory = applyCommand story command
    let newHistory = { Past = command :: history.Past; Future = [] }
    newStory, newHistory

let undo (story: StoryDraft) (history: CommandHistory) =
    match history.Past with
    | [] -> story, history
    | cmd :: rest ->
        let original = // reconstruct from empty + all commands except last
            history.Past
            |> List.rev
            |> List.tail
            |> List.fold applyCommand emptyDraft
        original, { Past = rest; Future = cmd :: history.Future }
```

## Pattern Selection Guide

| Problem               | Pattern                      | Reference                |
| --------------------- | ---------------------------- | ------------------------ |
| Validate input        | Railway-Oriented Programming | Design-Chapter11         |
| Transform data        | Pipeline/Composition         | Design-Chapter11         |
| Manage state          | State Machine                | Design-Chapter05         |
| Undo/redo             | Command                      | DesignPatterns-Chapter05 |
| Extend behavior       | Strategy (functions)         | DesignPatterns-Chapter05 |
| Build complex objects | Builder                      | DesignPatterns-Chapter03 |
| Notify changes        | Observer (events)            | DesignPatterns-Chapter05 |
