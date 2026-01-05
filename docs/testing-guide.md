# Testing Patterns Guide

## Quick Links by Task

### bUnit (Component Testing)

| Task               | Topic                         |
| ------------------ | ----------------------------- |
| Getting started    | bunit#getting-started         |
| Passing parameters | bunit#passing-parameters      |
| Injecting services | bunit#injecting-services      |
| Mocking components | bunit#substituting-components |
| Triggering events  | bunit#triggering-events       |
| Async testing      | bunit#async-state             |
| Verifying output   | bunit#verifying-output        |
| Test doubles       | bunit#test-doubles            |

### F# Testing

| Task                   | Topic                  |
| ---------------------- | ---------------------- |
| Types over tests       | fsharp#testing-expecto |
| Expecto framework      | fsharp#testing-expecto |
| Property-based testing | fsharp#testing-expecto |

## Key Patterns for Career Story Builder

Testing strategy:

1. **Domain logic**: Unit tests with Expecto, property-based tests with FsCheck
2. **Components**: bUnit tests for Bolero/Blazor components
3. **Integration**: Test API endpoints and database operations
4. **Types over tests**: Leverage F# type system to prevent bugs at compile time

## Primary References

### F# Testing Philosophy

- **Types Over Tests**: `fsharp#testing-expecto`
  - Use the type system to make illegal states unrepresentable
  - Test business logic, not type-enforced constraints
  - See also: [F# Style Guide](fsharp-style-guide.md) for domain modeling patterns that reduce test surface

### Component Testing

- **bUnit Patterns**: `bunit#getting-started`
  - Render components
  - Find elements
  - Trigger events
  - Assert markup

## Domain Examples

### Story Validation Tests (Expecto)

```fsharp
open Expecto

let storyValidationTests = testList "Story Validation" [
    test "valid story passes validation" {
        let story = {
            Id = StoryId (Guid.NewGuid())
            Title = "Led migration to cloud infrastructure"
            Situation = { Context = "Legacy on-prem system..."; When = Some (DateOnly(2023, 1, 1)); Where = Some "Acme Corp" }
            Task = { Challenge = "Migrate 50+ services"; Responsibility = "Technical lead"; Stakeholders = ["Engineering"; "Ops"] }
            Actions = [
                { Step = 1; Description = "Assessed current state"; Skills = ["Analysis"] }
                { Step = 2; Description = "Designed migration plan"; Skills = ["Architecture"; "AWS"] }
            ]
            Result = { Outcome = "100% migrated in 6 months"; Impact = Some "40% cost reduction"; Metrics = Some "$500K saved annually" }
            Tags = ["cloud"; "leadership"]
            CreatedAt = DateTimeOffset.UtcNow
            UpdatedAt = DateTimeOffset.UtcNow
        }

        let result = StoryValidator.validate story
        Expect.isOk result "Story should be valid"
    }

    test "story with empty title fails validation" {
        let story = { validStory with Title = "" }
        let result = StoryValidator.validate story
        Expect.isError result "Should fail with empty title"

        match result with
        | Error errors -> Expect.contains errors (TitleTooShort 1) "Should have TitleTooShort error"
        | _ -> failtest "Expected error"
    }

    test "story with no actions fails validation" {
        let story = { validStory with Actions = [] }
        let result = StoryValidator.validate story
        Expect.isError result "Should fail with no actions"
    }

    testProperty "all valid stories have non-empty titles" <| fun (story: Story) ->
        match StoryValidator.validate story with
        | Ok validStory -> validStory.Title.Length > 0
        | Error _ -> true  // Invalid stories are allowed to have empty titles
]
```

### Property-Based Testing with FsCheck

```fsharp
open FsCheck

// Custom generators for domain types
type StoryGenerators =
    static member Story() =
        gen {
            let! title = Arb.generate<NonEmptyString> |> Gen.map (fun s -> s.Get)
            let! context = Arb.generate<NonEmptyString> |> Gen.map (fun s -> s.Get)
            let! challenge = Arb.generate<NonEmptyString> |> Gen.map (fun s -> s.Get)
            let! outcome = Arb.generate<NonEmptyString> |> Gen.map (fun s -> s.Get)
            let! actionCount = Gen.choose (1, 10)
            let! actions = Gen.listOfLength actionCount (gen {
                let! desc = Arb.generate<NonEmptyString> |> Gen.map (fun s -> s.Get)
                return { Step = 1; Description = desc; Skills = [] }
            })

            return {
                Id = StoryId (Guid.NewGuid())
                Title = title
                Situation = { Context = context; When = None; Where = None }
                Task = { Challenge = challenge; Responsibility = ""; Stakeholders = [] }
                Actions = actions |> List.mapi (fun i a -> { a with Step = i + 1 })
                Result = { Outcome = outcome; Impact = None; Metrics = None }
                Tags = []
                CreatedAt = DateTimeOffset.UtcNow
                UpdatedAt = DateTimeOffset.UtcNow
            }
        } |> Arb.fromGen

let propertyTests = testList "Story Properties" [
    testProperty "validated stories can be serialized and deserialized" <| fun (story: Story) ->
        match StoryValidator.validate story with
        | Ok valid ->
            let json = JsonSerializer.Serialize(valid)
            let deserialized = JsonSerializer.Deserialize<Story>(json)
            deserialized = valid
        | Error _ -> true

    testProperty "story actions are always ordered by step" <| fun (story: Story) ->
        let sorted = story.Actions |> List.sortBy (fun a -> a.Step)
        story.Actions = sorted
]
```

### Component Tests (bUnit)

```fsharp
open Bunit
open Xunit

type StoryFormTests() =
    inherit BunitContext()

    [<Fact>]
    member this.``Form renders with empty fields initially``() =
        // Arrange
        let cut = this.Render<StoryFormComponent>()

        // Assert
        let titleInput = cut.Find("input#title")
        Assert.Empty(titleInput.GetAttribute("value"))

    [<Fact>]
    member this.``Title input updates model on change``() =
        // Arrange
        let mutable capturedTitle = ""
        let cut = this.Render<StoryFormComponent>(parameters => parameters
            .Add(p => p.OnTitleChanged, fun t -> capturedTitle <- t))

        // Act
        cut.Find("input#title").Change("Led cloud migration")

        // Assert
        Assert.Equal("Led cloud migration", capturedTitle)

    [<Fact>]
    member this.``Add Action button adds new action field``() =
        // Arrange
        let cut = this.Render<StoryFormComponent>()
        let initialCount = cut.FindAll(".action-item").Count

        // Act
        cut.Find("button.add-action").Click()

        // Assert
        let newCount = cut.FindAll(".action-item").Count
        Assert.Equal(initialCount + 1, newCount)

    [<Fact>]
    member this.``Submit shows validation errors for empty form``() =
        // Arrange
        let cut = this.Render<StoryFormComponent>()

        // Act
        cut.Find("button[type=submit]").Click()

        // Assert
        cut.WaitForAssertion(fun () ->
            let errors = cut.FindAll(".validation-error")
            Assert.NotEmpty(errors)
        )

    [<Fact>]
    member this.``Form with valid data calls save handler``() =
        // Arrange
        let mutable savedStory: Story option = None
        let mockService = Mock<IStoryService>()
        mockService.Setup(fun s -> s.Save(It.IsAny<Story>()))
            .ReturnsAsync(Ok validStory)

        this.Services.AddSingleton(mockService.Object)

        let cut = this.Render<StoryFormComponent>(parameters => parameters
            .Add(p => p.OnSaved, fun s -> savedStory <- Some s))

        // Act - fill form
        cut.Find("input#title").Change("Test Story")
        cut.Find("textarea#situation").Change("Test context...")
        cut.Find("textarea#task").Change("Test challenge...")
        cut.Find("button.add-action").Click()
        cut.Find("input.action-description").Change("First action")
        cut.Find("textarea#result").Change("Test outcome...")

        // Submit
        cut.Find("button[type=submit]").Click()

        // Assert
        cut.WaitForAssertion(fun () ->
            Assert.True(savedStory.IsSome)
        )
```

### Mocking Services for Tests

```fsharp
type StoryListTests() =
    inherit BunitContext()

    [<Fact>]
    member this.``List shows loading state initially``() =
        // Arrange - slow service
        let tcs = TaskCompletionSource<Story list>()
        let mockService = Mock<IStoryService>()
        mockService.Setup(fun s -> s.GetAll()).Returns(tcs.Task)

        this.Services.AddSingleton(mockService.Object)

        // Act
        let cut = this.Render<StoryListComponent>()

        // Assert - loading shown
        cut.Find(".loading-spinner") |> ignore

        // Complete the async
        tcs.SetResult([validStory])

        // Assert - stories shown
        cut.WaitForAssertion(fun () ->
            cut.Find(".story-card") |> ignore
        )

    [<Fact>]
    member this.``Empty list shows placeholder message``() =
        // Arrange
        let mockService = Mock<IStoryService>()
        mockService.Setup(fun s -> s.GetAll()).ReturnsAsync([])

        this.Services.AddSingleton(mockService.Object)

        // Act
        let cut = this.Render<StoryListComponent>()

        // Assert
        cut.WaitForAssertion(fun () ->
            let placeholder = cut.Find(".empty-state")
            Assert.Contains("No stories yet", placeholder.TextContent)
        )
```

### F# Idiomatic Mocking Patterns

In F#, prefer fakes and stubs over dynamic mocking frameworks. This approach is simpler and more aligned with functional programming principles.

**Fake functions with mutable capture**:

```fsharp
module UserEmailTests =

    let test_sendWelcomeEmail_includes_time () =
        // Deterministic time
        let fixed = DateTime(2025, 12, 26, 0, 0, 0, DateTimeKind.Utc)
        let clock () = fixed

        // Capture side effects
        let mutable captured : (string * string) option = None
        let sendEmail toAddr body = captured <- Some(toAddr, body)

        // Execute
        sendWelcomeEmail clock sendEmail "a@b.com"

        // Assert
        match captured with
        | Some(toAddr, body) ->
            Expect.equal toAddr "a@b.com" "Recipient should match"
            Expect.isTrue (body.Contains("2025-12-26")) "Should include date"
        | None ->
            failwith "Expected email to be sent"
```

**Inject deterministic time, randomness, and GUIDs**:

Never call `DateTime.UtcNow`, `Guid.NewGuid()`, or random generators directly in core logic. Instead, inject them as function parameters:

```fsharp
// Define capability types
type Clock = unit -> DateTime
type NewGuid = unit -> Guid
type NextInt = int -> int

// Production implementations
let realClock () = DateTime.UtcNow
let realGuid () = Guid.NewGuid()

// Test implementations
let fixedClock dt = fun () -> dt
let fixedGuid g = fun () -> g
```

**Test with record of functions**:

```fsharp
type UserDeps =
    { Clock: unit -> DateTime
      SendEmail: string -> string -> unit
      LoadUser: int -> string option }

let test_notifyUser_sends_email () =
    let fixed = DateTime(2025, 12, 26, 0, 0, 0, DateTimeKind.Utc)
    let mutable sent = []

    let deps =
        { Clock = fun () -> fixed
          SendEmail = fun toAddr body -> sent <- (toAddr, body) :: sent
          LoadUser = fun _ -> Some "a@b.com" }

    UserWorkflow.notifyUser deps 123

    Expect.isNonEmpty sent "Should have sent email"
```

See: `fsharp#pure-functional` for dependency injection patterns.

### Integration Tests

```fsharp
open Microsoft.AspNetCore.Mvc.Testing
open System.Net.Http.Json

type StoryApiTests(factory: WebApplicationFactory<Program>) =
    interface IClassFixture<WebApplicationFactory<Program>>

    let client = factory.CreateClient()

    [<Fact>]
    member _.``GET /api/stories returns empty list initially``() = task {
        let! response = client.GetAsync("/api/stories")
        response.EnsureSuccessStatusCode() |> ignore

        let! stories = response.Content.ReadFromJsonAsync<Story list>()
        Assert.Empty(stories)
    }

    [<Fact>]
    member _.``POST /api/stories creates new story``() = task {
        let dto = {| Title = "Test Story"; ... |}
        let! response = client.PostAsJsonAsync("/api/stories", dto)

        Assert.Equal(HttpStatusCode.Created, response.StatusCode)

        let! story = response.Content.ReadFromJsonAsync<Story>()
        Assert.Equal("Test Story", story.Title)
    }
```

## Test Organization

```text
tests/
├── CareerStoryBuilder.Domain.Tests/
│   ├── ValidationTests.fs
│   ├── DomainModelTests.fs
│   └── PropertyTests.fs
├── CareerStoryBuilder.Client.Tests/
│   ├── Components/
│   │   ├── StoryFormTests.fs
│   │   └── StoryListTests.fs
│   └── _Imports.razor
└── CareerStoryBuilder.Integration.Tests/
    ├── ApiTests.fs
    └── RepositoryTests.fs
```
