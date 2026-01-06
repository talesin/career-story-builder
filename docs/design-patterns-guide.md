# Design Patterns Guide (Functional)

## Key Patterns for Career Story Builder

Functional design principles guide the architecture:

- **Immutability**: All domain types are immutable records
- **Railway-Oriented Programming**: Validation with Result type
- **Data transformation pipelines**: Processing story data
- **SOLID principles**: Applied to F# modules and functions

## Domain Examples

### Railway-Oriented Validation Pipeline

Reference: `design#data-flow`

```fsharp
// Railway-Oriented Programming: chain validations, fail on first error

type ValidationError = FieldEmpty of string | TooLong of string * int

let validateName name =
    if String.IsNullOrWhiteSpace name then Error (FieldEmpty "name")
    else Ok name

let validateEmail email =
    if String.IsNullOrWhiteSpace email then Error (FieldEmpty "email")
    elif not (email.Contains "@") then Error (FieldEmpty "email")
    else Ok email

// Compose validators using Result.bind (the "railway")
let validateUser name email =
    validateName name
    |> Result.bind (fun n ->
        validateEmail email
        |> Result.map (fun e -> { Name = n; Email = e }))
```

### Alternative: Using result Computation Expression

```fsharp
// FSharpPlus result CE - same logic, cleaner syntax
let validateUser name email =
    result {
        let! n = validateName name
        let! e = validateEmail email
        return { Name = n; Email = e }
    }
```

**Fail-fast vs Error Accumulation**:

The examples above use `Result.bind` which **fails on first error**. This is appropriate for:

- API validation (stop processing on first problem)
- Database operations (rollback on first failure)

For **form validation** where you want to show all errors at once, use FSharpPlus `Validation` type instead. See [FSharpPlus Guide](fsharpplus-guide.md#validation-with-error-accumulation).

**Trade-offs**:

- Nested `bind` chains make the railroad tracks explicit
- Computation expressions are more readable but hide the mechanics

### Data Transformation Pipeline

```fsharp
// Functional pipelines for data processing

let filterByStatus status = List.filter (fun x -> x.Status = status)
let sortByDate = List.sortByDescending (fun x -> x.CreatedAt)
let take n = List.truncate n

// Pipeline composition with |>
let getRecentActive count items =
    items
    |> filterByStatus Active
    |> sortByDate
    |> take count

// Alternative: function composition with >>
let getRecentActive' count =
    filterByStatus Active >> sortByDate >> take count
```

### SOLID in Functional F\#

Reference: `design#solid`

```fsharp
// Single Responsibility Principle - each module has one job
module Validation = let validate x = ...
module Persistence = let save x = ...
module Formatting = let toJson x = ...

// Open-Closed Principle - extend via new functions, not modification
type Formatter<'T> = 'T -> string

let jsonFormatter: Formatter<User> = JsonSerializer.Serialize
let csvFormatter: Formatter<User> = fun u -> $"{u.Name},{u.Email}"

// Add new formatters without changing existing code
let format (formatter: Formatter<'T>) item = formatter item

// Dependency Inversion Principle - depend on abstractions
type IRepository<'T> =
    abstract GetById: Guid -> Task<'T option>
    abstract Save: 'T -> Task<unit>

// High-level code depends on abstraction, not concrete implementation
let saveIfValid (repo: IRepository<_>) validate item =
    match validate item with
    | Ok valid -> repo.Save valid
    | Error _ -> Task.FromResult ()
```

### Functional Dependency Injection

In F#, dependency injection is primarily about passing values. Choose the pattern based on complexity.

See: `fsharp#pure-functional`

#### Pattern 1: Pass dependencies as parameters

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

#### Pattern 2: Record of functions (capabilities bundle)

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

| Scenario                             | Pattern                                                                                          |
| ------------------------------------ | ------------------------------------------------------------------------------------------------ |
| Few dependencies, shallow call graph | Explicit parameters                                                                              |
| Many dependencies, want simplicity   | Record of functions                                                                              |
| Framework requires DI classes        | Thin wrapper (see ASP.NET Guide)                                                                 |
| Deep dependency threading            | Reader monad (see [FSharpPlus Guide](fsharpplus-guide.md#reader-monad-for-dependency-injection)) |

**Guidelines**:

- Keep dependency parameters on the left, business inputs on the right
- Bind dependencies once in the composition root using partial application
- Keep dependency records small per module; avoid one mega-record for the whole app
- Prefer "capabilities" naming over "Service" naming

> *For advanced functional DI using the Reader monad pattern, see [FSharpPlus Guide](fsharpplus-guide.md#reader-monad-for-dependency-injection).*

### State Machine Pattern

Reference: `design#state-management`

```fsharp
// State machine for form workflow

type FormState =
    | Empty
    | Editing of data: string
    | Submitting of data: string
    | Complete of result: string
    | Failed of error: string

type FormEvent =
    | Start
    | Update of string
    | Submit
    | Succeed of string
    | Fail of string

let transition state event =
    match state, event with
    | Empty, Start -> Editing ""
    | Editing _, Update data -> Editing data
    | Editing data, Submit -> Submitting data
    | Submitting _, Succeed result -> Complete result
    | Submitting data, Fail error -> Failed error
    | Failed _, Start -> Empty  // Retry
    | state, _ -> state  // Invalid transition, stay put
```

### Command Pattern for Operations

Reference: `design#gof-behavioral`

```fsharp
// Command pattern for undo/redo

type Command =
    | SetName of string
    | SetEmail of string
    | SetAge of int

type History = { Past: Command list; Future: Command list }

let apply user cmd =
    match cmd with
    | SetName n -> { user with Name = n }
    | SetEmail e -> { user with Email = e }
    | SetAge a -> { user with Age = a }

let execute user history cmd =
    apply user cmd, { Past = cmd :: history.Past; Future = [] }

let undo user history =
    match history.Past with
    | [] -> user, history
    | cmd :: rest ->
        let restored = rest |> List.rev |> List.fold apply emptyUser
        restored, { Past = rest; Future = cmd :: history.Future }
```

## Pattern Selection Guide

| Problem               | Pattern                      | Reference                |
| --------------------- | ---------------------------- | ------------------------ |
| Validate input        | Railway-Oriented Programming | design#data-flow         |
| Transform data        | Pipeline/Composition         | design#data-flow         |
| Manage state          | State Machine                | design#state-management  |
| Undo/redo             | Command                      | design#gof-behavioral    |
| Extend behavior       | Strategy (functions)         | design#gof-behavioral    |
| Build complex objects | Builder                      | design#gof-creational    |
| Notify changes        | Observer (events)            | design#gof-behavioral    |

## See Also

- `design#immutability` - examples TBD
- `design#persistent-data` - examples TBD
- `design#recursion-iteration` - examples TBD
- `design#laziness` - examples TBD
- `design#testing` - examples TBD
- `design#gof-introduction` - examples TBD
- `design#gof-creational` - examples TBD
- `design#gof-structural` - examples TBD
- `design#patterns-review` - examples TBD
