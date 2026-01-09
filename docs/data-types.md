# Core Data Types (Preliminary)

Minimal F# types for the Phase 1 prototype. These will evolve as features are added in later phases.

## ID Types

All entity IDs use ordered UUIDs (UUIDv7) for database-friendly sequential ordering. Each has a `.Value` member for convenient extraction:

```fsharp
type StoryId = StoryId of Guid with
    member this.Value = match this with StoryId id -> id

type UserId = UserId of Guid with
    member this.Value = match this with UserId id -> id

type RoleId = RoleId of Guid with
    member this.Value = match this with RoleId id -> id
```

## SAR Components

Single-case discriminated unions with `.Value` members for type safety:

```fsharp
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
```

Usage:

```fsharp
let story : Story = {
    Title = StoryTitle "Led migration project"
    Situation = StorySituation "Legacy system needed modernization"
    Action = StoryAction "Designed migration strategy with rollback plan"
    Result = StoryResult "Zero downtime, 40% performance improvement"
}

// Access values via .Value member
let title = story.Title.Value  // "Led migration project"
```

## Conversation State

Types for the AI-assisted story building workflow:

```fsharp
type MessageRole =
    | User
    | Assistant

type ChatMessage = {
    Role: MessageRole
    Content: string
    Timestamp: DateTimeOffset
    Error: string option        // AI API error message, if any
}

type WorkflowStep =
    | InitialCapture      // Free-form story input
    | Clarification       // AI asking follow-up questions
    | Refinement          // Iterating on sections
    | Generation          // AI generating final story

type ConversationState = {
    Messages: ChatMessage list
    CurrentStep: WorkflowStep
    DraftStory: Story option
    LastError: string option    // Most recent error for display
    IsProcessing: bool          // AI request in progress
}
```

## Service Interfaces

```fsharp
type IStoryService =
    abstract member GetAll: unit -> Task<Story list>
    abstract member GetById: StoryId -> Task<Story option>
    abstract member Create: CreateStoryDto -> Task<Result<Story, string>>
    abstract member Update: StoryId -> UpdateStoryDto -> Task<Result<Story, string>>
    abstract member Delete: StoryId -> Task<Result<unit, string>>
```

## DTOs

Data transfer objects for API communication:

```fsharp
type CreateStoryDto = {
    Title: string
    Situation: string
    Action: string
    Result: string
}

type UpdateStoryDto = {
    Title: string option
    Situation: string option
    Action: string option
    Result: string option
}
```

## Test Fixtures

Example valid story for use in tests:

```fsharp
let validStory : Story = {
    Title = StoryTitle "Led migration project"
    Situation = StorySituation "Legacy system needed modernization due to performance issues"
    Action = StoryAction "Designed migration strategy with rollback plan and led team of 4"
    Result = StoryResult "Zero downtime, 40% performance improvement, completed 2 weeks early"
}
```

---

## Future Phases

Types to be added as the application evolves:

### Phase 3 (Persistence & Drafts)

- `StoryStatus` - Draft | InReview | Complete
- `QualityScore` - Per-section and overall scores (0-100)
- `DraftContent` - Partial story for save/resume

### Phase 6 (Metadata)

- `Role` - Employment position details
- `Tags` - Free-form categorization

### Phase 9 (Scoring)

- `ScoringCriteria` - Criteria definitions and weights
- `SectionScore` - Per-section quality assessment
