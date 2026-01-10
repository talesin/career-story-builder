namespace CareerStoryBuilder.Dto

open System.Threading.Tasks
open CareerStoryBuilder.Domain

/// Request payload for creating a new story.
type CreateStoryRequest = {
    Title: string
    Situation: string
    Action: string
    Result: string
}

module CreateStoryRequest =
    let tryToStory (request: CreateStoryRequest) : Result<Story, ValidationError list> =
        match StoryTitle.tryCreate request.Title with
        | Error e -> Error [ e ]
        | Ok title ->
            Ok {
                Title = title
                Situation = StorySituation request.Situation
                Action = StoryAction request.Action
                Result = StoryResult request.Result
            }

/// Request payload for updating an existing story.
type UpdateStoryRequest = {
    Title: string option
    Situation: string option
    Action: string option
    Result: string option
}

module UpdateStoryRequest =
    let tryApplyTo (request: UpdateStoryRequest) (story: Story) : Result<Story, ValidationError list> =
        let titleResult =
            request.Title
            |> Option.map StoryTitle.tryCreate
            |> Option.defaultValue (Ok story.Title)

        match titleResult with
        | Error e -> Error [ e ]
        | Ok title ->
            Ok {
                Title = title
                Situation = request.Situation |> Option.map StorySituation |> Option.defaultValue story.Situation
                Action = request.Action |> Option.map StoryAction |> Option.defaultValue story.Action
                Result = request.Result |> Option.map StoryResult |> Option.defaultValue story.Result
            }

/// Service interface for story operations.
type IStoryService =
    abstract member GetAll: unit -> Task<Story list>
    abstract member GetById: StoryId -> Task<Story option>
    abstract member TryCreate: CreateStoryRequest -> Task<Result<Story, StoryError>>
    abstract member TryUpdate: StoryId -> UpdateStoryRequest -> Task<Result<Story, StoryError>>
    abstract member TryDelete: StoryId -> Task<Result<unit, StoryError>>
