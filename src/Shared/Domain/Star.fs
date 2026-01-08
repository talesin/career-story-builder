namespace CareerStoryBuilder.Domain

open System

/// Story title with validation.
type StoryTitle = private StoryTitle of string with
    static member Extract(StoryTitle s) = s

module StoryTitle =
    let tryCreate (title: string) =
        let trimmed = title.Trim()
        if String.IsNullOrWhiteSpace trimmed then
            Error TitleRequired
        else
            Ok (StoryTitle trimmed)

/// STAR framework components for behavioral interview stories.
/// Wrapped in a module to avoid collisions with System.Threading.Tasks.Task
/// and F#'s Result<'T,'E> type.
module Star =
    /// The context and background of the story.
    type Situation = Situation of string with
        static member Extract(Situation s) = s

    /// The challenge or responsibility you faced.
    type Task = Task of string with
        static member Extract(Task t) = t

    /// The specific steps you took to address the task.
    type Action = Action of string with
        static member Extract(Action a) = a

    /// The outcome and impact of your actions.
    type Result = Result of string with
        static member Extract(Result r) = r

/// A complete STAR story for career interviews.
type Story = {
    Title: StoryTitle
    Situation: Star.Situation
    Task: Star.Task
    Action: Star.Action
    Result: Star.Result
}

module Story =
    let tryCreate title situation task action result =
        match StoryTitle.tryCreate title with
        | Error e -> Error [ e ]
        | Ok t ->
            Ok {
                Title = t
                Situation = situation
                Task = task
                Action = action
                Result = result
            }

    /// Empty story for form initialization. Validation happens on save.
    let empty = {
        Title = StoryTitle ""  // Bypass validation for form initialization
        Situation = Star.Situation ""
        Task = Star.Task ""
        Action = Star.Action ""
        Result = Star.Result ""
    }
