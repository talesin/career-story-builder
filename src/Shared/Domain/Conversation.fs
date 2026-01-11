namespace CareerStoryBuilder.Domain

open System

/// Role of a message in the conversation.
type MessageRole =
    | User
    | Assistant

/// A single message in the AI-assisted conversation.
type ChatMessage = {
    Role: MessageRole
    Content: string
    Timestamp: DateTimeOffset
    Error: ConversationError option
}

module ChatMessage =
    let create role content = {
        Role = role
        Content = content
        Timestamp = DateTimeOffset.UtcNow
        Error = None
    }

    let withError error message = { message with Error = Some error }

/// Steps in the story creation workflow.
type WorkflowStep =
    | InitialCapture
    | Clarification
    | Refinement
    | Generation

/// State of the AI-assisted story building conversation.
/// Messages are stored in reverse chronological order (newest first) for O(1) append.
type ConversationState = {
    Messages: ChatMessage list
    CurrentStep: WorkflowStep
    DraftStory: Story option
    LastError: ConversationError option
    IsProcessing: bool
}

module ConversationState =
    let initial = {
        Messages = []
        CurrentStep = InitialCapture
        DraftStory = None
        LastError = None
        IsProcessing = false
    }

    /// Add a message. Messages stored in reverse chronological order (O(1) prepend).
    let addMessage message state = {
        state with Messages = message :: state.Messages
    }

    /// Get messages in chronological order for display.
    let messagesOrdered state = state.Messages |> List.rev

    let setStep step state = { state with CurrentStep = step }

    let setDraft story state = { state with DraftStory = Some story }

    let setProcessing processing state = { state with IsProcessing = processing }

    let setError error state = { state with LastError = Some error }

    let clearError state = { state with LastError = None }
