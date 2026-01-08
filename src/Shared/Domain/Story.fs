namespace CareerStoryBuilder.Domain

open System
open FSharpPlus

/// Story title with validation.
type StoryTitle =
    private
    | StoryTitle of string

    static member Extract(StoryTitle s) = s

module StoryTitle =
    let tryCreate (title: string) =
        let trimmed = title.Trim()

        if String.IsNullOrWhiteSpace trimmed then
            Error TitleRequired
        else
            Ok(StoryTitle trimmed)


/// The context and background of the story.
type StorySituation =
    | StorySituation of string

    static member Extract(StorySituation s) = s

/// The specific steps you took to address the situation.
type StoryAction =
    | StoryAction of string

    static member Extract(StoryAction a) = a

/// The outcome and impact of your actions.
type StoryResult =
    | StoryResult of string

    static member Extract(StoryResult r) = r

/// A complete SAR story for career interviews.
type Story =
    { Title: StoryTitle
      Situation: StorySituation
      Action: StoryAction
      Result: StoryResult }

module Story =
    let tryCreate title situation action result =
        StoryTitle.tryCreate title
        |> map (fun t ->
            { Title = t
              Situation = situation
              Action = action
              Result = result })

    /// Empty story for form initialization. Validation happens on save.
    let empty =
        { Title = StoryTitle "" // Bypass validation for form initialization
          Situation = StorySituation ""
          Action = StoryAction ""
          Result = StoryResult "" }
