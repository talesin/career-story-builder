namespace CareerStoryBuilder.Domain

/// STAR framework components for behavioral interview stories.
/// Wrapped in a module to avoid collisions with System.Threading.Tasks.Task
/// and F#'s Result<'T,'E> type.
module Star =
    /// The context and background of the story.
    type Situation = Situation of string

    /// The challenge or responsibility you faced.
    type Task = Task of string

    /// The specific steps you took to address the task.
    type Action = Action of string

    /// The outcome and impact of your actions.
    type Result = Result of string

    module Situation =
        let create s = Situation s
        let value (Situation s) = s

    module Task =
        let create s = Task s
        let value (Task s) = s

    module Action =
        let create s = Action s
        let value (Action s) = s

    module Result =
        let create s = Result s
        let value (Result s) = s

/// A complete STAR story for career interviews.
type Story = {
    Title: string
    Situation: Star.Situation
    Task: Star.Task
    Action: Star.Action
    Result: Star.Result
}

module Story =
    let create title situation task action result = {
        Title = title
        Situation = situation
        Task = task
        Action = action
        Result = result
    }

    let empty = {
        Title = ""
        Situation = Star.Situation ""
        Task = Star.Task ""
        Action = Star.Action ""
        Result = Star.Result ""
    }
