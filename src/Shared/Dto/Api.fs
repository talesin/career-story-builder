namespace CareerStoryBuilder.Dto

open System.Threading.Tasks
open CareerStoryBuilder.Domain

/// DTO for creating a new story.
type CreateStoryDto = {
    Title: string
    Situation: string
    Task: string
    Action: string
    Result: string
}

module CreateStoryDto =
    let toStory (dto: CreateStoryDto) : Story = {
        Title = dto.Title
        Situation = Star.Situation dto.Situation
        Task = Star.Task dto.Task
        Action = Star.Action dto.Action
        Result = Star.Result dto.Result
    }

/// DTO for updating an existing story.
type UpdateStoryDto = {
    Title: string option
    Situation: string option
    Task: string option
    Action: string option
    Result: string option
}

module UpdateStoryDto =
    let applyTo (dto: UpdateStoryDto) (story: Story) : Story = {
        Title = dto.Title |> Option.defaultValue story.Title
        Situation = dto.Situation |> Option.map Star.Situation |> Option.defaultValue story.Situation
        Task = dto.Task |> Option.map Star.Task |> Option.defaultValue story.Task
        Action = dto.Action |> Option.map Star.Action |> Option.defaultValue story.Action
        Result = dto.Result |> Option.map Star.Result |> Option.defaultValue story.Result
    }

/// Service interface for story operations.
type IStoryService =
    abstract member GetAll: unit -> Task<Story list>
    abstract member GetById: StoryId -> Task<Story option>
    abstract member Create: CreateStoryDto -> Task<Result<Story, string>>
    abstract member Update: StoryId -> UpdateStoryDto -> Task<Result<Story, string>>
    abstract member Delete: StoryId -> Task<Result<unit, string>>
