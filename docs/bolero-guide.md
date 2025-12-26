# Bolero MVU Development Guide

> References: `$REFERENCES/bolero/`

## Quick Links by Task

| Task                    | Reference                                      |
| ----------------------- | ---------------------------------------------- |
| Set up Bolero project   | `$REFERENCES/bolero/index.md#getting-started`  |
| Write HTML with DSL     | `$REFERENCES/bolero/index.md#html-dsl`         |
| Set up MVU architecture | `$REFERENCES/bolero/index.md#mvu-architecture` |
| Define routes           | `$REFERENCES/bolero/index.md#routing`          |
| Client-server remoting  | `$REFERENCES/bolero/index.md#remoting`         |
| Use HTML templates      | `$REFERENCES/bolero/index.md#templating`       |
| Create components       | `$REFERENCES/bolero/index.md#components`       |
| Configure hosting       | `$REFERENCES/bolero/index.md#server`           |
| Best practices          | `$REFERENCES/bolero/index.md#patterns`         |

## Key Patterns for Career Story Builder

Bolero provides an F#-first approach to Blazor using the MVU (Model-View-Update) pattern:
- **Model**: Immutable state representing the current application state
- **View**: Pure function rendering the UI from the model
- **Update**: Pure function handling messages and producing new state

## Primary References

### MVU Architecture
- **Elmish Integration**: `$REFERENCES/bolero/index.md#mvu-architecture`
  - ProgramComponent base class
  - Model and Messages definition
  - Update function patterns
  - Commands for side effects

### HTML DSL
- **Writing Views**: `$REFERENCES/bolero/index.md#html-dsl`
  - Element builders (`div { }`, `button { }`)
  - Attributes (`attr.*`)
  - Event handlers (`on.*`)
  - Conditional rendering (`cond`)
  - Lists (`forEach`)

### Routing
- **Page Navigation**: `$REFERENCES/bolero/index.md#routing`
  - Route definitions with DUs
  - Path and query parameters
  - Navigation links

## Domain Examples

### Story Editor Model

```fsharp
open Elmish
open Bolero

// Application pages
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/stories">] StoryList
    | [<EndPoint "/stories/{id}">] StoryDetail of id: Guid
    | [<EndPoint "/stories/new">] NewStory
    | [<EndPoint "/stories/{id}/edit">] EditStory of id: Guid

// Story editor state
type StoryEditorModel = {
    Title: string
    Situation: SituationForm
    Task: TaskForm
    Actions: ActionForm list
    Result: ResultForm
    IsSaving: bool
    Errors: ValidationError list
}

and SituationForm = {
    Context: string
    When: string
    Where: string
}

and TaskForm = {
    Challenge: string
    Responsibility: string
    Stakeholders: string
}

and ActionForm = {
    Step: int
    Description: string
    Skills: string
}

and ResultForm = {
    Outcome: string
    Impact: string
    Metrics: string
}

// Application model
type Model = {
    Page: Page
    Stories: Story list
    CurrentStory: StoryEditorModel option
    IsLoading: bool
    Error: string option
}
```

### Messages

```fsharp
type Message =
    // Navigation
    | SetPage of Page

    // Story list
    | LoadStories
    | StoriesLoaded of Story list
    | LoadStoriesFailed of exn

    // Story editor
    | StartNewStory
    | EditStory of StoryId
    | StoryLoaded of Story

    // Form updates
    | SetTitle of string
    | SetSituationContext of string
    | SetSituationWhen of string
    | SetSituationWhere of string
    | SetTaskChallenge of string
    | SetTaskResponsibility of string
    | SetTaskStakeholders of string
    | AddAction
    | RemoveAction of int
    | SetActionDescription of step: int * description: string
    | SetActionSkills of step: int * skills: string
    | SetResultOutcome of string
    | SetResultImpact of string
    | SetResultMetrics of string

    // Save
    | SaveStory
    | StorySaved of Story
    | SaveFailed of ValidationError list
```

### Update Function

```fsharp
let update (message: Message) (model: Model) =
    match message with
    | SetPage page ->
        { model with Page = page }, Cmd.none

    | LoadStories ->
        { model with IsLoading = true },
        Cmd.OfTask.either storyService.GetAll () StoriesLoaded LoadStoriesFailed

    | StoriesLoaded stories ->
        { model with Stories = stories; IsLoading = false }, Cmd.none

    | SetTitle title ->
        match model.CurrentStory with
        | Some editor ->
            { model with CurrentStory = Some { editor with Title = title } }, Cmd.none
        | None -> model, Cmd.none

    | SaveStory ->
        match model.CurrentStory with
        | Some editor ->
            { model with CurrentStory = Some { editor with IsSaving = true } },
            Cmd.OfTask.either storyService.Save (toStory editor) StorySaved SaveFailed
        | None -> model, Cmd.none

    | StorySaved story ->
        { model with
            CurrentStory = None
            Stories = story :: model.Stories },
        Cmd.ofMsg (SetPage StoryList)

    | _ -> model, Cmd.none
```

### View Functions

```fsharp
open Bolero.Html

let storyFormView (editor: StoryEditorModel) dispatch =
    div {
        attr.``class`` "story-form"

        // Title
        div {
            attr.``class`` "form-group"
            label { "Story Title" }
            input {
                attr.``type`` "text"
                attr.value editor.Title
                attr.placeholder "e.g., Led cross-functional team to deliver..."
                on.change (fun e -> dispatch (SetTitle (unbox e.Value)))
            }
        }

        // Situation section
        fieldset {
            legend { "Situation" }
            textarea {
                attr.placeholder "Describe the context and background..."
                attr.value editor.Situation.Context
                on.change (fun e -> dispatch (SetSituationContext (unbox e.Value)))
            }
        }

        // Task section
        fieldset {
            legend { "Task" }
            textarea {
                attr.placeholder "What was the challenge or responsibility?"
                attr.value editor.Task.Challenge
                on.change (fun e -> dispatch (SetTaskChallenge (unbox e.Value)))
            }
        }

        // Actions section
        fieldset {
            legend { "Actions" }
            forEach editor.Actions <| fun action ->
                div {
                    attr.key (string action.Step)
                    input {
                        attr.``type`` "text"
                        attr.value action.Description
                        on.change (fun e ->
                            dispatch (SetActionDescription (action.Step, unbox e.Value)))
                    }
                    button {
                        on.click (fun _ -> dispatch (RemoveAction action.Step))
                        "Remove"
                    }
                }
            button {
                on.click (fun _ -> dispatch AddAction)
                "Add Action"
            }
        }

        // Result section
        fieldset {
            legend { "Result" }
            textarea {
                attr.placeholder "What was the outcome and impact?"
                attr.value editor.Result.Outcome
                on.change (fun e -> dispatch (SetResultOutcome (unbox e.Value)))
            }
        }

        // Validation errors
        cond (not (List.isEmpty editor.Errors)) <| function
            | true ->
                ul {
                    attr.``class`` "errors"
                    forEach editor.Errors <| fun error ->
                        li { text (formatError error) }
                }
            | false -> empty ()

        // Submit
        button {
            attr.``type`` "submit"
            attr.disabled editor.IsSaving
            on.click (fun _ -> dispatch SaveStory)
            cond editor.IsSaving <| function
                | true -> text "Saving..."
                | false -> text "Save Story"
        }
    }
```

### Routing Setup

```fsharp
let router = Router.infer SetPage (fun m -> m.Page)

type App() =
    inherit ProgramComponent<Model, Message>()

    override this.Program =
        let storyService = this.Services.GetService<IStoryService>()
        Program.mkProgram
            (fun _ -> initModel, Cmd.ofMsg LoadStories)
            (update storyService)
            (view router)
        |> Program.withRouter router
```

### Remoting Service

```fsharp
// Shared service interface
type IStoryService =
    { getAll: unit -> Async<Story list>
      getById: Guid -> Async<Story option>
      save: Story -> Async<Result<Story, ValidationError list>>
      delete: Guid -> Async<unit> }
    interface IRemoteService with
        member _.BasePath = "/api/stories"

// Client usage with commands
let loadStoriesCmd (service: IStoryService) =
    Cmd.OfAsync.either service.getAll () StoriesLoaded LoadStoriesFailed
```

## Component Organization

```
src/
├── Client/
│   ├── Main.fs           # ProgramComponent, router
│   ├── Model.fs          # Model, Message types
│   ├── Update.fs         # Update function
│   └── Views/
│       ├── StoryList.fs  # Story list view
│       ├── StoryForm.fs  # Story editor view
│       └── Components/   # Reusable view components
├── Server/
│   └── StoryService.fs   # Remote service implementation
└── Shared/
    ├── Domain.fs         # STAR story types
    └── Services.fs       # IStoryService interface
```
