# Core Data Types (Preliminary)

Minimal F# types for the Phase 1 prototype. These will evolve as features are added in later phases.

> **Namespacing**: Star component types are wrapped in `module Star` to avoid collisions with `System.Threading.Tasks.Task` and F#'s `Result<'T,'E>` type. Access via `Star.Task`, `Star.Result`, etc.

## Star Components

Simple string wrappers for type safety:

```fsharp
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
```

Usage:

```fsharp
let story : Story = {
    Title = "Led migration project"
    Situation = Star.Situation "Legacy system needed modernization"
    Task = Star.Task "Migrate 500k records to new platform"
    Action = Star.Action "Designed migration strategy with rollback plan"
    Result = Star.Result "Zero downtime, 40% performance improvement"
}
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
}
```

---

## Future Phases

Types to be added as the application evolves:

### Phase 2 (Authentication)

- `UserId` - User identity from auth provider

### Phase 3 (Persistence & Drafts)

- `StoryId` - Unique story identifier
- `StoryStatus` - Draft | InReview | Complete
- `QualityScore` - Per-section and overall scores (0-100)
- `DraftContent` - Partial story for save/resume

### Phase 6 (Metadata)

- `Tags` - Free-form categorization

### Phase 7 (Employment History)

- `RoleId`, `Role` - Employment positions
- `ProjectId`, `Project` - Projects within roles
