# Core Data Types (Preliminary)

Minimal F# types for the Phase 1 prototype. These will evolve as features are added in later phases.

## STAR Components

Simple string wrappers for type safety:

```fsharp
type Situation = Situation of string
type StoryTask = StoryTask of string
type Action = Action of string
type Result = Result of string
```

## Story

Minimal record for a complete STAR story:

```fsharp
type Story = {
    Title: string
    Situation: Situation
    Task: StoryTask
    Action: Action
    Result: Result
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
