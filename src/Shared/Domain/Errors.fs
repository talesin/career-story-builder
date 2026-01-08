namespace CareerStoryBuilder.Domain

/// Validation errors for story content.
type ValidationError =
    | TitleRequired

/// Service-level errors for story operations.
type StoryError =
    | ValidationFailed of ValidationError list
    | NotFound of StoryId
    | Unauthorized
    | DatabaseError of message: string

/// Errors from AI conversation operations.
type ConversationError =
    | AiServiceUnavailable
    | RateLimited
    | InvalidResponse of reason: string
    | NetworkError of message: string
