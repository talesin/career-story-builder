# Bolero MVU Development Guide

## Key Patterns for Career Story Builder

Bolero provides an F#-first approach to Blazor using the MVU (Model-View-Update) pattern:

- **Model**: Immutable state representing the current application state
- **View**: Pure function rendering the UI from the model
- **Update**: Pure function handling messages and producing new state

## Domain Examples

### Story Editor Model

Reference: `bolero#mvu-architecture`

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

// Story editor form state (matches CreateStoryRequest structure)
// See data-types.md for authoritative type definitions
type StoryEditorModel = {
    Title: string
    Situation: string
    Action: string
    Result: string
    IsSaving: bool
    Errors: string list
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

    // Form updates (one per SAR field)
    | SetTitle of string
    | SetSituation of string
    | SetAction of string
    | SetResult of string

    // Save
    | SaveStory
    | StorySaved of Story
    | SaveFailed of string list

    // Error handling
    | ClearError
```

### Update Function

```fsharp
/// Extract string value from change event (handlers receive 'obj')
let inline inputValue (e: ChangeEventArgs) = unbox<string> e.Value

// Helper to update editor if present
let updateEditor f model =
    match model.CurrentStory with
    | Some editor -> { model with CurrentStory = Some (f editor) }, Cmd.none
    | None -> model, Cmd.none

let update (service: IStoryService) (message: Message) (model: Model) =
    match message with
    | SetPage page ->
        { model with Page = page }, Cmd.none

    | LoadStories ->
        { model with IsLoading = true },
        Cmd.OfAsync.either service.GetAll () StoriesLoaded LoadStoriesFailed

    | StoriesLoaded stories ->
        { model with Stories = stories; IsLoading = false }, Cmd.none

    | StartNewStory ->
        let empty = { Title = ""; Situation = ""; Action = ""; Result = ""; IsSaving = false; Errors = [] }
        { model with CurrentStory = Some empty }, Cmd.none

    // Form field updates - helper to update current story
    | SetTitle v -> model |> updateEditor (fun e -> { e with Title = v })
    | SetSituation v -> model |> updateEditor (fun e -> { e with Situation = v })
    | SetAction v -> model |> updateEditor (fun e -> { e with Action = v })
    | SetResult v -> model |> updateEditor (fun e -> { e with Result = v })

    | SaveStory ->
        match model.CurrentStory with
        | Some editor ->
            let dto = { Title = editor.Title; Situation = editor.Situation
                        Action = editor.Action; Result = editor.Result }
            { model with CurrentStory = Some { editor with IsSaving = true } },
            Cmd.OfAsync.either service.Create dto StorySaved SaveFailed
        | None -> model, Cmd.none

    | StorySaved story ->
        { model with CurrentStory = None; Stories = story :: model.Stories },
        Cmd.ofMsg (SetPage StoryList)

    | SaveFailed errors ->
        model |> updateEditor (fun e -> { e with Errors = errors; IsSaving = false })

    | ClearError ->
        { model with Error = None }, Cmd.none

    | _ -> model, Cmd.none
```

### View Functions

Reference: `bolero#html-dsl`

```fsharp
open Bolero.Html

// Error display view - shows application errors from Model.Error
let errorView (error: string option) dispatch =
    cond error <| function
        | Some msg ->
            div {
                attr.``class`` "alert alert-danger"
                text msg
                button {
                    attr.``class`` "close"
                    on.click (fun _ -> dispatch ClearError)
                    "×"
                }
            }
        | None -> empty ()

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
                on.change (fun e -> dispatch (SetTitle (inputValue e)))
            }
        }

        // Situation
        fieldset {
            legend { "Situation" }
            textarea {
                attr.placeholder "Describe the context and background..."
                attr.value editor.Situation
                on.change (fun e -> dispatch (SetSituation (inputValue e)))
            }
        }

        // Action
        fieldset {
            legend { "Action" }
            textarea {
                attr.placeholder "What steps did you take?"
                attr.value editor.Action
                on.change (fun e -> dispatch (SetAction (inputValue e)))
            }
        }

        // Result
        fieldset {
            legend { "Result" }
            textarea {
                attr.placeholder "What was the outcome and impact?"
                attr.value editor.Result
                on.change (fun e -> dispatch (SetResult (inputValue e)))
            }
        }

        // Validation errors
        cond (not (List.isEmpty editor.Errors)) <| function
            | true ->
                ul {
                    attr.``class`` "errors"
                    forEach editor.Errors <| fun error ->
                        li { text error }
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

Reference: `bolero#routing`

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

Reference: `bolero#remoting`

```fsharp
// Service interface defined in data-types.md (abstract member style)
// See data-types.md for the authoritative IStoryService definition

// Client usage with commands
let loadStoriesCmd (service: IStoryService) =
    Cmd.OfAsync.either (fun () -> service.GetAll() |> Async.AwaitTask) () StoriesLoaded LoadStoriesFailed

let saveStoryCmd (service: IStoryService) (request: CreateStoryRequest) =
    Cmd.OfAsync.either (fun () -> service.Create(request) |> Async.AwaitTask) () StorySaved SaveFailed
```

## Component Organization

```text
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
    ├── Domain.fs         # SAR story types
    └── Services.fs       # IStoryService interface
```

## Testing

For component testing patterns with bUnit, see [Testing Guide](testing-guide.md).

## See Also

- `bolero#getting-started` - examples TBD
- `bolero#templating` - examples TBD
- `bolero#components` - examples TBD
- `bolero#server` - examples TBD
- `bolero#patterns` - examples TBD
