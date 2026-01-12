namespace CareerStoryBuilder.Dto

open System
open CareerStoryBuilder.Domain

/// A message in the conversation (embedded in ConversationSnapshot).
type Message = {
    Role: MessageRole
    Content: string
    Timestamp: DateTimeOffset
    Error: string option
}

module Message =
    let fromDomain (msg: ChatMessage) : Message = {
        Role = msg.Role
        Content = msg.Content
        Timestamp = msg.Timestamp
        Error = msg.Error |> Option.map (function
            | AiServiceUnavailable -> "AI service unavailable"
            | RateLimited -> "Rate limited"
            | InvalidResponse reason -> $"Invalid response: {reason}"
            | NetworkError message -> $"Network error: {message}")
    }

    let toDomain (msg: Message) : ChatMessage = {
        Role = msg.Role
        Content = msg.Content
        Timestamp = msg.Timestamp
        Error = None
    }

/// Snapshot of conversation state (passed between client and server).
type ConversationSnapshot = {
    Messages: Message list
    CurrentStep: WorkflowStep
    DraftTitle: string option
}

module ConversationSnapshot =
    let fromDomain (state: ConversationState) : ConversationSnapshot = {
        Messages = state.Messages |> List.map Message.fromDomain |> List.rev
        CurrentStep = state.CurrentStep
        DraftTitle = state.DraftStory |> Option.map _.Title.Value
    }

    let toDomain (snapshot: ConversationSnapshot) : ConversationState = {
        Messages = snapshot.Messages |> List.map Message.toDomain |> List.rev
        CurrentStep = snapshot.CurrentStep
        DraftStory = None
        LastError = None
        IsProcessing = false
    }

/// Generated SAR story output.
type StoryOutput = {
    Title: string
    Situation: string
    Action: string
    Result: string
}

module StoryOutput =
    let fromDomain (story: Story) : StoryOutput = {
        Title = story.Title.Value
        Situation = story.Situation.Value
        Action = story.Action.Value
        Result = story.Result.Value
    }

/// Request to get clarifying questions from AI.
type ClarifyRequest = {
    Conversation: ConversationSnapshot
    UserMessage: string
}

/// Response with AI clarifying questions.
type ClarifyResponse = {
    AssistantMessage: string
    Conversation: ConversationSnapshot
}

/// Request to generate the final story.
type GenerateRequest = {
    Conversation: ConversationSnapshot
}

/// Response with generated SAR story.
type GenerateResponse = {
    Story: StoryOutput
    Suggestions: string list option
}
