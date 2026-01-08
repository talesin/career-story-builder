namespace CareerStoryBuilder.Domain

open System

/// Story title with validation.
type StoryTitle =
    private
    | StoryTitle of string

    member this.Value = match this with StoryTitle s -> s

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

    member this.Value = match this with StorySituation s -> s

/// The specific steps you took to address the situation.
type StoryAction =
    | StoryAction of string

    member this.Value = match this with StoryAction a -> a
/// The outcome and impact of your actions.
type StoryResult =
    | StoryResult of string

    member this.Value = match this with StoryResult r -> r

/// A complete SAR story for career interviews.
type Story =
    { Title: StoryTitle
      Situation: StorySituation
      Action: StoryAction
      Result: StoryResult }

module Story =
    let tryCreate title situation action result =
        StoryTitle.tryCreate title
        |> Result.map (fun t ->
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
