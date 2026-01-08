namespace CareerStoryBuilder.Dto

open System.Threading.Tasks
open CareerStoryBuilder.Domain

/// DTO for creating a new story.
type CreateStoryDto = {
    Title: string
    Situation: string
    Action: string
    Result: string
}

module CreateStoryDto =
    let tryToStory (dto: CreateStoryDto) : Result<Story, ValidationError list> =
        match StoryTitle.tryCreate dto.Title with
        | Error e -> Error [ e ]
        | Ok title ->
            Ok {
                Title = title
                Situation = StorySituation dto.Situation
                Action = StoryAction dto.Action
                Result = StoryResult dto.Result
            }

/// DTO for updating an existing story.
type UpdateStoryDto = {
    Title: string option
    Situation: string option
    Action: string option
    Result: string option
}

module UpdateStoryDto =
    let tryApplyTo (dto: UpdateStoryDto) (story: Story) : Result<Story, ValidationError list> =
        let titleResult =
            dto.Title
            |> Option.map StoryTitle.tryCreate
            |> Option.defaultValue (Ok story.Title)

        match titleResult with
        | Error e -> Error [ e ]
        | Ok title ->
            Ok {
                Title = title
                Situation = dto.Situation |> Option.map StorySituation |> Option.defaultValue story.Situation
                Action = dto.Action |> Option.map StoryAction |> Option.defaultValue story.Action
                Result = dto.Result |> Option.map StoryResult |> Option.defaultValue story.Result
            }

/// Service interface for story operations.
type IStoryService =
    abstract member GetAll: unit -> Task<Story list>
    abstract member GetById: StoryId -> Task<Story option>
    abstract member TryCreate: CreateStoryDto -> Task<Result<Story, StoryError>>
    abstract member TryUpdate: StoryId -> UpdateStoryDto -> Task<Result<Story, StoryError>>
    abstract member TryDelete: StoryId -> Task<Result<unit, StoryError>>
