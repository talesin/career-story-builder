# F# Style Guide

This guide establishes consistent coding patterns for F# compiled applications. The goal is conciseness without sacrificing readability, following modern F# idioms and leveraging F# 10+ features.

## Table of Contents

1. [Underscore Shorthand](#1-underscore-shorthand)
2. [Option Handling](#2-option-handling)
3. [Result Handling](#3-result-handling)
4. [Type Annotations](#4-type-annotations)
5. [Function Composition](#5-function-composition)
6. [Negation Style](#6-negation-style)
7. [String Handling](#7-string-handling)
8. [Collection Conversions](#8-collection-conversions)
9. [Pattern Matching vs Combinators](#9-pattern-matching-vs-combinators)
10. [Domain Modeling](#10-domain-modeling)
11. [Error Handling Architecture](#11-error-handling-architecture)
12. [Module Organization](#12-module-organization)
13. [Async and Task Patterns](#13-async-and-task-patterns)
14. [F# 10 Features](#14-f-10-features)
15. [Formatting and Indentation](#15-formatting-and-indentation)
16. [Naming Conventions](#16-naming-conventions)
17. [Anti-Patterns to Avoid](#17-anti-patterns-to-avoid)

---

## 1. Underscore Shorthand

Use `_.property` shorthand for simple property access in lambdas.

```fsharp
// Prefer
items |> List.filter _.IsActive
items |> List.map _.Name.Trim()
option |> Option.bind _.Child
option |> Option.map _.Value

// Avoid
items |> List.filter (fun x -> x.IsActive)
items |> List.map (fun x -> x.Name.Trim())
option |> Option.bind (fun x -> x.Child)
```

**When NOT to use:** When the lambda body involves more than property access.

```fsharp
// Keep explicit lambda when there's logic
items |> List.filter (fun x -> x.Age > 18 && x.IsActive)
items |> List.map (fun x -> $"{x.FirstName} {x.LastName}")
```

---

## 2. Option Handling

### Use `Option.defaultValue` for simple defaults

```fsharp
// Prefer
person.BirthDate |> Option.defaultValue "N/A"

// Avoid
match person.BirthDate with
| Some d -> d
| None -> "N/A"
```

### Use `Option.defaultWith` when the default has side effects or is expensive

```fsharp
// Prefer
getValue key
|> Option.defaultWith (fun () ->
    logger.Error("Key not found: {Key}", key)
    computeExpensiveDefault())

// Avoid
match getValue key with
| Some value -> value
| None ->
    logger.Error("Key not found: {Key}", key)
    computeExpensiveDefault()
```

### Use `Option.contains` for equality checks

```fsharp
// Prefer
extractId record |> Option.contains targetId

// Avoid
match extractId record with
| Some id -> id = targetId
| None -> false

// Also avoid
extractId record
|> Option.map ((=) targetId)
|> Option.defaultValue false
```

### Use `Option.iter` for side effects on Some

```fsharp
// Prefer
person.Name |> Option.iter (fun n -> logger.Info("Name: {Name}", n))

// Avoid
match person.Name with
| Some n -> logger.Info("Name: {Name}", n)
| None -> ()
```

### Use `Option.bind` for chained optional operations

```fsharp
// Prefer
record.Period
|> Option.bind _.Start
|> Option.bind tryParseDateTime

// Avoid
match record.Period with
| Some p ->
    match p.Start with
    | Some s -> tryParseDateTime s
    | None -> None
| None -> None
```

### Use `Option.orElseWith` for fallback chains

```fsharp
// Prefer
primaryValue
|> Option.orElseWith (fun () -> secondaryValue)
|> Option.orElseWith (fun () -> tertiaryValue)

// Avoid nested matches
```

### Avoid `Option.get` - it's unsafe

```fsharp
// Avoid - throws on None
let value = maybeValue |> Option.get

// Prefer - explicit handling
let value =
    maybeValue
    |> Option.defaultWith (fun () ->
        failwith "Expected value to be present")

// Or pattern match if context requires it
match maybeValue with
| Some v -> v
| None -> // handle appropriately
```

---

## 3. Result Handling

### Use Result for operations that can fail with domain errors

```fsharp
// Prefer - explicit error handling
let tryGetPerson (conn: SqlConnection) (id: PersonId) : Result<Person option, DatabaseError> =
    try
        conn.Query<Person>(sql, {| id = PersonId.value id |})
        |> Seq.tryHead
        |> Ok
    with
    | :? SqlException as ex -> Error (QueryFailed ex.Message)
```

### Name Result-returning functions with `try` prefix

```fsharp
// Naming convention
let tryGetPerson ...       // Returns Result<Person option, Error>
let tryInsertRecord ...    // Returns Result<int, Error>
let tryValidateEmail ...   // Returns Result<EmailAddress, ValidationError>
```

### Define a `Result.ofOption` helper

```fsharp
module Result =
    let ofOption errorValue = function
        | Some x -> Ok x
        | None -> Error errorValue

// Usage
conn.Query<Person>(sql, params)
|> Seq.tryHead
|> Result.ofOption (NotFound "Person not found")
```

### Match is appropriate for complex Result flows

When handling Results with multiple branches, logging, or transactions, explicit matching is clearer:

```fsharp
// Match is appropriate here - clear control flow
match tryGetPerson conn id with
| Error msg ->
    logger.Error("Query failed: {Error}", msg)
    ProcessingResult.failed result
| Ok None ->
    logger.Warning("Person not found")
    ProcessingResult.notFound result
| Ok (Some person) ->
    // continue processing
```

### Use Railway-Oriented Programming for validation pipelines

```fsharp
// Chain validations with bind
let validateOrder order =
    order
    |> validateCustomerId
    |> Result.bind validateShippingAddress
    |> Result.bind validateLineItems
    |> Result.bind validatePaymentInfo

// Use applicative style for error accumulation
let validatePerson name email age =
    let createPerson n e a = { Name = n; Email = e; Age = a }
    createPerson
    <!> validateName name
    <*> validateEmail email
    <*> validateAge age
```

---

## 4. Type Annotations

### Omit when inference is clear

```fsharp
// Prefer - types are obvious from usage
let sanitizeForFilename s =
    s.Replace("/", "_").Replace("\\", "_")

let getDir sourceDir subdir =
    Path.Combine(sourceDir, "tmp", subdir)

// Avoid - redundant annotations
let sanitizeForFilename (s: string) : string =
    s.Replace("/", "_").Replace("\\", "_")
```

### Keep annotations for public API clarity

```fsharp
// Keep - clarifies the contract
let tryGetRecords (conn: SqlConnection) (ids: RecordId list) : Result<RecordData list, DatabaseError> =
    // implementation

// Keep - complex return type benefits from annotation
let getOrFetchWith<'T>
    (logger: ILogger)
    (cachePath: string)
    (resourceName: string)
    (forceRefresh: bool)
    (isEmpty: 'T -> bool)
    (fetch: unit -> 'T)
    : 'T option =
    // implementation
```

### Keep annotations on record types for serialization/ORM

```fsharp
// Required for Dapper/EF Core/JSON serialization
[<CLIMutable>]
type RecordData = {
    RecordId: int64
    PersonId: string
    AccountId: string
}
```

---

## 5. Function Composition

### Use `>>` for simple pipelines in let bindings

```fsharp
// Prefer - clean composition
let getLevel = Args.getValue "--log-level" >> parseLevelOpt
let fromArgs = getLevel >> createLogger

// Avoid - unnecessary eta expansion
let getLevel parsed = Args.getValue "--log-level" parsed |> parseLevelOpt
```

### Prefer `|>` for multi-step transformations with intermediate clarity

```fsharp
// Prefer - each step is clear
records
|> List.filter (isMatchingRecord idUrn targetId)
|> List.sortBy _.Period
|> List.tryHead

// Avoid - composition can be hard to read for complex chains
let findFirst = List.filter (isMatchingRecord idUrn targetId) >> List.sortBy _.Period >> List.tryHead
```

### Design functions for partial application

```fsharp
// Configuration parameters first, data last
let validateWith rules data = // validation logic
let transformWith config data = // transformation logic

// Create specialized functions
let validateEmail = validateWith emailRules
let validateAge = validateWith ageRules

// Use in pipelines
let processData data =
    data
    |> validateEmail
    |> Result.bind validateAge
```

---

## 6. Negation Style

### Standardize on `not (...)` with parentheses

```fsharp
// Prefer
if not (String.IsNullOrEmpty value) then
if not (List.isEmpty items) then
if not (state.Switches.Contains name) then

// Avoid - less readable
if not <| String.IsNullOrEmpty value then
if value |> String.IsNullOrEmpty |> not then
```

---

## 7. String Handling

### Use `String.IsNullOrWhiteSpace` for user input validation

```fsharp
// Prefer - catches empty and whitespace-only
if not (String.IsNullOrWhiteSpace input) then

// Use IsNullOrEmpty only when whitespace is valid
if not (String.IsNullOrEmpty connectionString) then
```

### Consider a helper for defaulting empty strings

```fsharp
// Helper
let orDefault defaultValue s =
    if String.IsNullOrEmpty s then defaultValue else s

// Usage
let display = location.Display |> orDefault "Unknown"
```

### Use interpolated strings

```fsharp
// Prefer
$"person_{Cache.sanitizeForFilename id}.json"
$"{firstName} {lastName}"

// Avoid
sprintf "person_%s.json" (Cache.sanitizeForFilename id)
String.Format("{0} {1}", firstName, lastName)
```

Unless explicit typing will help simplify type inference:

```fsharp
// Prefer when inference needs help
let fullname first last =
    sprintf "%s %s" first last

// Avoid adding annotations just for interpolation
let fullname (first: string) (last: string) =
    $"{first} {last}"
```

---

## 8. Collection Conversions

### Use the most direct conversion

```fsharp
// For lists
items |> Seq.toList      // Prefer
items |> List.ofSeq      // Also acceptable

// For single items
items |> Seq.tryHead     // Prefer for Option<'T>

// For sets
items |> Set.ofSeq       // Direct to Set

// For arrays
items |> Seq.toArray     // Prefer
items |> Array.ofSeq     // Also acceptable
```

### Prefer `List.tryHead` over `Seq.tryHead |> Option...` when you have a list

```fsharp
// If you already have a list
myList |> List.tryHead

// If you have a seq (e.g., from Dapper)
results |> Seq.tryHead
```

### Choose collections based on use case

```fsharp
// Array: indexed access, mutation, interop
let data = [| 1; 2; 3 |]

// List: functional transformations, pattern matching
let items = [ 1; 2; 3 ]

// Seq: lazy evaluation, large datasets
let lazy = seq { 1..1000000 } |> Seq.filter isValid |> Seq.take 10

// Set: unique values, membership testing
let unique = Set.ofList [ 1; 2; 2; 3 ]

// Map: key-value lookup
let lookup = Map.ofList [ ("a", 1); ("b", 2) ]
```

---

## 9. Pattern Matching vs Combinators

### Use combinators for simple transformations

```fsharp
// Prefer combinators
let parseLevelOpt = Option.map parseLevel >> Option.defaultValue LogEventLevel.Information

let isMatchingRecord idUrn targetId record =
    extractId idUrn record |> Option.contains targetId
```

### Use pattern matching for multiple branches with different logic

```fsharp
match result with
| Error msg ->
    logger.Error("Failed: {Error}", msg)
    ProcessingResult.failed acc
| Ok (Some existing) ->
    logger.Warning("Already exists")
    ProcessingResult.skipped acc
| Ok None ->
    // Insert logic
    ProcessingResult.inserted acc
```

**Destructuring with guards:**

```fsharp
match parts with
| [| siteIdStr; mrn; csn; firstName; lastName |]
    when not (String.IsNullOrWhiteSpace mrn) ->
    Valid (siteId, mrn, csn, firstName, lastName)
| _ ->
    Invalid line
```

**Transaction/workflow control:**

```fsharp
// Match makes the flow explicit
match tryGetPerson conn id with
| Ok (Some person) ->
    match tryGetRecord conn person.Id recordId with
    | Ok (Some record) -> Ok (person, record)
    | Ok None -> createRecord conn person recordId
    | Error msg -> Error msg
| Ok None -> Error (NotFound "Person not found")
| Error msg -> Error msg
```

### Always handle all cases explicitly

```fsharp
// Prefer - exhaustive handling
let processStatus status =
    match status with
    | Pending -> "Processing pending order"
    | Processing -> "Order is being processed"
    | Shipped -> "Order has been shipped"
    | Delivered -> "Order delivered successfully"

// Avoid - wildcard hides new cases
let processStatus status =
    match status with
    | Pending -> "Processing pending order"
    | _ -> "Handling order"
```

---

## 10. Domain Modeling

### Use single-case discriminated unions to prevent primitive obsession

```fsharp
// Define domain-specific types
type EmailAddress = EmailAddress of string
type CustomerId = CustomerId of Guid
type OrderId = OrderId of int

// Constructor with validation
module EmailAddress =
    let create str =
        if String.length str > 0 && str.Contains("@")
        then Ok (EmailAddress str)
        else Error "Invalid email format"

    let value (EmailAddress str) = str
```

### Model states explicitly with discriminated unions

```fsharp
// Prefer - explicit states
type OrderStatus =
    | Pending
    | Processing of startedAt: DateTime
    | Shipped of trackingNumber: string
    | Delivered of deliveredAt: DateTime
    | Cancelled of reason: string

// Avoid - boolean flags
type Order = {
    IsPending: bool
    IsProcessing: bool
    IsShipped: bool
    TrackingNumber: string option  // Only valid if shipped
}
```

### Make illegal states unrepresentable

```fsharp
// Good - impossible to have a shipped order without tracking
type ShippedOrder = {
    OrderId: OrderId
    TrackingNumber: string  // Required, not optional
    ShippedAt: DateTime
}

type Order =
    | Pending of PendingOrder
    | Shipped of ShippedOrder
    | Delivered of DeliveredOrder
```

### Use constrained types for business rules

```fsharp
type PositiveInt = private PositiveInt of int

module PositiveInt =
    let create n =
        if n > 0 then Ok (PositiveInt n)
        else Error "Must be positive"

    let value (PositiveInt n) = n

type Quantity = private Quantity of int

module Quantity =
    let create n =
        if n >= 0 && n <= 10000 then Ok (Quantity n)
        else Error "Quantity must be between 0 and 10000"

    let value (Quantity n) = n
```

---

## 11. Error Handling Architecture

### Three classes of errors

#### Domain Errors - Use Result

```fsharp
type ValidationError =
    | EmptyName
    | InvalidEmail of string
    | AgeOutOfRange of int

type DomainError =
    | ValidationFailed of ValidationError list
    | CustomerNotFound of CustomerId
    | InsufficientStock of ProductId
```

#### Infrastructure Errors - Context dependent

```fsharp
type DatabaseError =
    | ConnectionTimeout
    | QueryFailed of string
    | DeadlockDetected

// Convert to domain errors at boundaries
let mapDbError = function
    | ConnectionTimeout -> "Database temporarily unavailable"
    | QueryFailed msg -> $"Data access failed: {msg}"
    | DeadlockDetected -> "Please retry your request"
```

#### Panics - Use exceptions

```fsharp
// Let these bubble up - they indicate programmer errors
let getArrayElement index array =
    if index < 0 || index >= Array.length array
    then invalidArg "index" "Index out of bounds"
    else array.[index]
```

### Fail fast vs error accumulation

```fsharp
// Fail fast - use bind
let processOrder order =
    validateOrder order
    |> Result.bind processPayment
    |> Result.bind shipOrder

// Error accumulation - use applicative
let validatePerson name email age =
    (validateName name, validateEmail email, validateAge age)
    |||> Result.map3 (fun n e a -> { Name = n; Email = e; Age = a })
```

---

## 12. Module Organization

### Group types with their behavior

```fsharp
module Customer =
    type CustomerId = CustomerId of Guid

    type CustomerInfo = {
        Name: string
        Email: EmailAddress
    }

    type Customer = {
        Id: CustomerId
        Info: CustomerInfo
    }

    // Constructor functions
    let createId () = CustomerId (Guid.NewGuid())

    let create name email =
        { Id = createId()
          Info = { Name = name; Email = email } }

    // Behavior functions
    let updateEmail newEmail customer =
        { customer with Info = { customer.Info with Email = newEmail } }
```

### Separate pure from impure functions

```fsharp
// Pure functions (domain logic)
module Domain =
    let calculateShipping weight distance =
        let baseRate = 5.0m
        let weightRate = weight * 0.1m
        let distanceRate = distance * 0.01m
        baseRate + weightRate + distanceRate

    let validateOrder order =
        // Pure validation logic

// Impure functions (infrastructure)
module Infrastructure =
    let saveOrder connectionString order =
        // Database save

    let sendEmail smtpConfig email content =
        // Email sending
```

### Use dependency injection through function parameters

```fsharp
// Instead of DI containers, pass functions
let processOrderWorkflow
    (validateCustomer: CustomerId -> Result<Customer, Error>)
    (saveOrder: Order -> Result<Order, Error>)
    (sendEmail: EmailAddress -> Order -> unit)
    (order: Order) =

    validateCustomer order.CustomerId
    |> Result.bind (fun customer ->
        saveOrder order
        |> Result.map (fun saved ->
            sendEmail customer.Email saved
            saved))

// Wire up at composition root
let main () =
    let processor =
        processOrderWorkflow
            (Database.validateCustomer connectionString)
            (Database.saveOrder connectionString)
            (Email.sendOrderConfirmation emailConfig)

    // Use processor...
```

---

## 13. Async and Task Patterns

### Prefer `task { }` for .NET interop

```fsharp
// Prefer task for modern .NET APIs
let fetchDataAsync (url: string) = task {
    use client = new HttpClient()
    let! response = client.GetStringAsync(url)
    return parseJson response
}

// Use async when you need F#-specific features like cancellation propagation
let longRunningAsync () = async {
    let! token = Async.CancellationToken
    // Use token for cancellation checking
}
```

### Use `and!` for concurrent operations (F# 10)

```fsharp
// Sequential - one after another
task {
    let! a = fetchA()
    let! b = fetchB()
    return combineAB a b
}

// Concurrent - both at once (F# 10)
task {
    let! a = fetchA()
    and! b = fetchB()
    return combineAB a b
}
```

### Handle async error propagation

```fsharp
let tryFetchAsync url = task {
    try
        let! result = fetchAsync url
        return Ok result
    with
    | :? HttpRequestException as ex -> return Error (NetworkError ex.Message)
    | :? TimeoutException -> return Error Timeout
}
```

---

## 14. F# 10 Features

### Scoped warning suppression

```fsharp
// Suppress warning for a specific section
#nowarn 25
let processPartial (Some x) =    // FS0025 suppressed
    x + 1
#warnon 25
// FS0025 enabled again
```

### Access modifiers on auto-property accessors

```fsharp
// Before F# 10 - verbose
type Ledger() =
    [<DefaultValue>] val mutable private _Balance: decimal
    member this.Balance
        with public get() = this._Balance
        and private set v = this._Balance <- v

// F# 10 - concise
type Ledger() =
    member val Balance = 0m with public get, private set
```

### Struct-based optional parameters for performance

```fsharp
// Standard (heap-allocated Option)
type Api() =
    static member Search(?query: string) =
        match query with
        | Some q -> searchFor q
        | None -> getAll()

// F# 10 - struct-based (stack-allocated ValueOption)
type Api() =
    static member Search([<Struct>] ?query: string) =
        match query with
        | ValueSome q -> searchFor q
        | ValueNone -> getAll()
```

### Simplified type annotations in computation expressions

```fsharp
// Before F# 10 - parentheses required
async {
    let! (result: int) = computeAsync()
    use! (resource: IDisposable) = acquireAsync()
    return result
}

// F# 10 - no parentheses needed
async {
    let! result: int = computeAsync()
    use! resource: IDisposable = acquireAsync()
    return result
}
```

### Discard pattern in `use!` bindings

```fsharp
// Before F# 10
task {
    use! _ignored = acquireLockAsync()
    // do work
}

// F# 10
task {
    use! _ = acquireLockAsync()
    // do work
}
```

### Explicit `seq` for sequence expressions

```fsharp
// Deprecated in F# 10 - avoid bare braces
let numbers = { 1..10 }  // Warning FS3873

// Prefer explicit seq
let numbers = seq { 1..10 }
```

---

## 15. Formatting and Indentation

### Use 4-space indentation

```fsharp
let myFunction x =
    let y = x + 1
    let z = y * 2
    z
```

### Avoid name-length sensitive formatting

```fsharp
// Prefer - independent of name length
let myLongValueName =
    someExpression
    |> anotherExpression

// Avoid - breaks if name changes
let myLongValueName = someExpression
                      |> anotherExpression
```

### Pipeline operator on new line

```fsharp
// Prefer
let result =
    data
    |> validate
    |> transform
    |> save

// Avoid
let result = data
           |> validate
           |> transform
```

### Match clauses not indented from match

```fsharp
// Correct
match value with
| Some x -> x
| None -> 0

// Wrong
match value with
    | Some x -> x
    | None -> 0
```

### Record formatting - pick one style consistently

```fsharp
// Stroustrup (recommended for this codebase)
type Person = {
    Name: string
    Age: int
}

let person = {
    Name = "Alice"
    Age = 30
}

// Or Aligned
type Person =
    {
        Name: string
        Age: int
    }
```

### Separate top-level declarations with blank lines

```fsharp
let thing1 = 1 + 1

let thing2 = 1 + 2

type ThisThat = This | That
```

---

## 16. Naming Conventions

| Element         | Convention        | Example           |
| --------------- | ----------------- | ----------------- |
| Types           | PascalCase        | `CustomerService` |
| Modules         | PascalCase        | `StringUtils`     |
| Functions       | camelCase         | `calculateTotal`  |
| Values          | camelCase         | `customerName`    |
| Parameters      | camelCase         | `inputValue`      |
| Type parameters | 'PascalCase or 'a | `'T`, `'a`        |
| DU cases        | PascalCase        | `Some`, `None`    |
| Record fields   | PascalCase        | `FirstName`       |

### Function naming

```fsharp
// Use verbs for actions
let calculateTotal items = ...
let validateEmail input = ...
let sendNotification user = ...

// Use try prefix for Result-returning functions
let tryParseInt s = ...
let tryGetUser id = ...

// Use is/has for predicates
let isValid x = ...
let hasPermission user = ...
```

---

## 17. Anti-Patterns to Avoid

### Primitive obsession

```fsharp
// Avoid
let createOrder (customerId: int) (email: string) = ...

// Prefer
let createOrder (customerId: CustomerId) (email: EmailAddress) = ...
```

### Mutable state and mutation

**Avoid mutation unless there is a significant, measured performance benefit.** Immutability is a core principle of functional programming that leads to more predictable, testable, and thread-safe code.

#### Use higher-order functions instead of loops with mutation

```fsharp
// Avoid - mutable accumulator
let mutable total = 0
for item in items do
    total <- total + item.Price

// Prefer - fold or specialized functions
let total = items |> List.sumBy _.Price

// Or explicit fold for custom accumulation
let total = items |> List.fold (fun acc item -> acc + item.Price) 0
```

#### Use recursion for complex iteration

```fsharp
// Avoid - mutable state tracking
let mutable found = None
let mutable i = 0
while found.IsNone && i < items.Length do
    if items.[i].IsMatch then
        found <- Some items.[i]
    i <- i + 1

// Prefer - recursive function (tail-recursive for performance)
let rec findFirst items =
    match items with
    | [] -> None
    | head :: _ when head.IsMatch -> Some head
    | _ :: tail -> findFirst tail

// Or use built-in functions
let found = items |> List.tryFind _.IsMatch
```

#### Use fold for building up results

```fsharp
// Avoid - mutable list building
let mutable results = []
for item in items do
    if item.IsValid then
        results <- item.Transform() :: results

// Prefer - fold or filter/map
let results =
    items
    |> List.filter _.IsValid
    |> List.map _.Transform()

// For complex accumulation, use fold
let results =
    ([], items)
    ||> List.fold (fun acc item ->
        if item.IsValid then item.Transform() :: acc
        else acc)
    |> List.rev
```

#### Use copy-and-update for record modifications

```fsharp
// Avoid - mutable fields
type Person = { mutable Name: string; mutable Age: int }
let updatePerson person =
    person.Name <- "New Name"
    person.Age <- person.Age + 1

// Prefer - immutable records with copy-and-update
type Person = { Name: string; Age: int }
let updatePerson person =
    { person with Name = "New Name"; Age = person.Age + 1 }
```

#### Use scan for intermediate results

```fsharp
// Avoid - mutable running total
let mutable runningTotal = 0
let totals = ResizeArray<int>()
for value in values do
    runningTotal <- runningTotal + value
    totals.Add(runningTotal)

// Prefer - scan
let totals = values |> List.scan (+) 0 |> List.tail
```

#### When mutation is acceptable

Mutation may be appropriate when:

1. **Measured performance requirement** - You've profiled and identified a bottleneck
2. **Encapsulated mutation** - The mutation is hidden behind a functional interface
3. **Interop scenarios** - Working with .NET APIs that require mutation

```fsharp
// Acceptable - encapsulated mutation for performance
let buildLookup items =
    let dict = Dictionary<string, Item>()
    for item in items do
        dict.[item.Key] <- item
    dict :> IReadOnlyDictionary<_, _>  // Expose as immutable

// Acceptable - StringBuilder for string building
let buildReport items =
    let sb = StringBuilder()
    for item in items do
        sb.AppendLine($"{item.Name}: {item.Value}") |> ignore
    sb.ToString()
```

### Not using the type system

```fsharp
// Avoid - stringly typed
type Order = {
    Status: string  // "pending", "shipped", etc.
}

// Prefer - type safe
type OrderStatus = Pending | Shipped | Delivered
type Order = {
    Status: OrderStatus
}
```

### Incomplete pattern matching

```fsharp
// Avoid - wildcard hides new cases
match status with
| Pending -> handlePending()
| _ -> handleOther()

// Prefer - explicit handling
match status with
| Pending -> handlePending()
| Shipped -> handleShipped()
| Delivered -> handleDelivered()
```

### Overusing Option.get

```fsharp
// Avoid - throws on None
let value = maybeValue.Value
let value = Option.get maybeValue

// Prefer
let value = maybeValue |> Option.defaultValue fallback
// or
match maybeValue with
| Some v -> v
| None -> handleMissing()
```

### Creating overly complex composition

```fsharp
// Avoid - hard to debug
let process = validate >> transform >> filter >> map >> sort >> group >> aggregate >> format

// Prefer - named intermediate steps
let process input =
    input
    |> validate
    |> Result.map (fun valid ->
        valid
        |> transform
        |> filter
        |> formatOutput)
```

### Using exceptions for control flow

```fsharp
// Avoid
try
    let user = findUser id
    processUser user
with
| :? UserNotFoundException -> handleMissing()

// Prefer
match tryFindUser id with
| Some user -> processUser user
| None -> handleMissing()
```

---

## Quick Reference

| Pattern              | Prefer                               | Avoid                      |                              |
| -------------------- | ------------------------------------ | -------------------------- | ---------------------------- |
| Property access      | `_.Name`                             | `(fun x -> x.Name)`        |                              |
| Default value        | `Option.defaultValue "N/A"`          | `match ... None -> "N/A"`  |                              |
| Default with effect  | `Option.defaultWith (fun () -> ...)` | `match ... None -> ...`    |                              |
| Equality check       | `Option.contains target`             | `Option.map ((=) target) \ | > Option.defaultValue false` |
| Side effect on Some  | `Option.iter`                        | `match ... Some -> ... \   | None -> ()`                  |
| Negation             | `not (expr)`                         | `not <\                    | expr`                        |
| String interpolation | `$"Hello {name}"`                    | `sprintf "Hello %s" name`  |                              |
| Result-returning fn  | `tryDoSomething`                     | `doSomething`              |                              |
| Empty check          | `String.IsNullOrWhiteSpace`          | Manual length/null checks  |                              |
| Concurrent tasks     | `let! a = x() and! b = y()`          | Sequential `let!` bindings |                              |
| Struct optionals     | `[<Struct>] ?param`                  | `?param` in hot paths      |                              |
| Accumulation         | `List.fold`, `List.sumBy`            | `mutable` with `for` loops |                              |
| Iteration            | `List.tryFind`, recursion            | `while` with mutable state |                              |
| Record updates       | `{ record with Field = value }`      | Mutable record fields      |                              |
