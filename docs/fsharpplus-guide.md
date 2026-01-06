# FSharpPlus Advanced Patterns Guide

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

## Domain Examples

### Validation with Error Accumulation

Reference: `fsharpplus#validation`, `fsharpplus#data-types`

```fsharp
open FSharpPlus
open FSharpPlus.Data

// Validation accumulates ALL errors, unlike Result which short-circuits

let validateName name : Validation<string list, string> =
    if String.IsNullOrWhiteSpace name then Failure ["Name required"]
    else Success name

let validateEmail email : Validation<string list, string> =
    if String.IsNullOrWhiteSpace email then Failure ["Email required"]
    elif not (email.Contains "@") then Failure ["Invalid email"]
    else Success email

// Combine validations - applicative style, all run, errors accumulate
let validateUser name email =
    (fun n e -> { Name = n; Email = e })
    <!> validateName name
    <*> validateEmail email

// Usage
let result = validateUser "" "bad"
// result = Failure ["Name required"; "Invalid email"]
// ALL errors collected, not just the first one!
```

### Generic Operators

Reference: `fsharpplus#operators`

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

Reference: `fsharpplus#lens`

```fsharp
open FSharpPlus.Lens

// Define lenses for nested structure: Person -> Address -> City
let inline _address f person =
    f person.Address |> map (fun a -> { person with Address = a })

let inline _city f address =
    f address.City |> map (fun c -> { address with City = c })

// Compose lenses
let _personCity = _address << _city

// Read nested value
let city = person ^. _personCity

// Update nested value immutably
let updated = person |> setl _personCity "New York"

// Transform nested value
let upperCity = person |> over _personCity String.toUpper
```

### Reader Monad for Dependency Injection

Reference: `fsharpplus#reader-monad`

```fsharp
open FSharpPlus.Data

// Dependencies as a record
type Deps = { GetUser: int -> string option; Log: string -> unit }

// Operations that depend on environment
let greet userId : Reader<Deps, string> =
    Reader (fun deps ->
        match deps.GetUser userId with
        | Some name -> $"Hello, {name}!"
        | None -> "User not found")

let greetAndLog userId : Reader<Deps, unit> =
    Reader (fun deps ->
        let msg = Reader.run deps (greet userId)
        deps.Log msg)

// Run with dependencies
let deps = { GetUser = (fun id -> if id = 1 then Some "Alice" else None)
             Log = printfn "%s" }

greetAndLog 1 |> Reader.run deps  // Prints: Hello, Alice!
```

### Computation Expression Examples

Reference: `fsharpplus#computation-expressions`

```fsharp
open FSharpPlus

// Generic monad CE works with any monad
let optionWorkflow = monad {
    let! user = findUser userId
    let! email = user.Email
    return email
}

// Applicative CE for independent operations (can run in parallel)
let fetchAllData = applicative {
    let! users = loadUsers ()
    and! config = loadConfig ()
    return { Users = users; Config = config }
}
```

## Pattern Comparison

| Need                | Result          | Validation       | When to Use                 |
| ------------------- | --------------- | ---------------- | --------------------------- |
| Fail on first error | Yes             | No               | Parse, IO operations        |
| Collect all errors  | No              | Yes              | Form validation, user input |
| Performance         | Better          | Slightly slower  | Most cases: Result          |
| User feedback       | Shows one error | Shows all errors | Forms: Validation           |

## Setup

### Package Installation

Add to your `.fsproj`:

```xml
<PackageReference Include="FSharpPlus" Version="1.*" />
```

### Common Imports

```fsharp
open FSharpPlus          // Generic operators (map, bind, etc.)
open FSharpPlus.Data     // Validation, Reader, NonEmptyList, etc.
open FSharpPlus.Lens     // Optics (only when needed for nested updates)
```

### When FSharpPlus is Required vs Optional

| Feature | FSharpPlus Required? | Alternative |
| ------- | -------------------- | ----------- |
| Error accumulation (Validation) | **Required** | Custom implementation is complex and error-prone |
| `Option.toResultWith` | **Required** | No built-in F# equivalent |
| Generic operators (`map`, `bind`) | Optional | Use type-specific functions (`List.map`, `Option.bind`) |
| Lenses | Optional | Use standard F# copy-and-update (`{ record with Field = ... }`) |
| Reader monad | Optional | Pass dependencies as function parameters |

**Recommendation for this project:**

- Use FSharpPlus for `Validation` and `Option.toResultWith` - these are genuinely useful and have no good alternatives
- Prefer standard F# for everything else unless you're working with code that already uses FSharpPlus patterns

## See Also

- `fsharpplus#getting-started` - examples TBD
- `fsharpplus#abstractions` - examples TBD
- `fsharpplus#monad-transformers` - examples TBD
