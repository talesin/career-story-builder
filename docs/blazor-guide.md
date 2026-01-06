# Blazor Component Patterns Guide

## Quick Links by Task

| Task                 | Topic                      |
| -------------------- | -------------------------- |
| Blazor intro         | blazor#introduction        |
| Razor syntax         | blazor#razor-syntax        |
| Component lifecycle  | blazor#component-lifecycle |
| Component parameters | blazor#components          |
| Two-way binding      | blazor#two-way-binding     |
| Forms and validation | blazor#forms-validation    |
| Dependency injection | blazor#di-patterns         |
| JavaScript interop   | blazor#js-interop          |
| Authentication       | blazor#security            |

## Key Patterns for Career Story Builder

While Bolero wraps Blazor with F#-friendly APIs, understanding Blazor patterns helps when:

- Working with component lifecycle methods
- Understanding render modes (Server vs WebAssembly)
- Integrating with Blazor component libraries
- Handling forms and validation

## Primary References

### Component Lifecycle

- **Lifecycle Methods**: `blazor#component-lifecycle`
  - `OnInitialized` / `OnInitializedAsync`
  - `OnParametersSet` / `OnParametersSetAsync`
  - `OnAfterRender` / `OnAfterRenderAsync`

### Forms and Validation

- **EditForm**: `blazor#forms-validation`
  - Input components
  - DataAnnotations validation
  - Custom validation

### Render Modes

- **Choosing Modes**: `blazor#render-modes`
  - Server-side (SignalR)
  - WebAssembly (client-side)
  - Auto mode

## Domain Examples

### Component Lifecycle in Bolero

Following the [split module/class pattern](fsharp-style-guide.md#split-moduleclass-pattern-for-framework-interop), extract async operations to a module for testability:

```fsharp
// Module: Contains async loading logic
// Testable without Blazor infrastructure
module StoryEditorOps =
    type LoadResult =
        | Loading
        | Loaded of Story
        | NotFound

    let loadStory (getStory: Guid -> Task<Story option>) (storyId: Guid) = task {
        let! story = getStory storyId
        return
            match story with
            | Some s -> Loaded s
            | None -> NotFound
    }

// Bolero component: Thin wrapper calling module functions
type StoryEditorComponent() =
    inherit Component()

    [<Parameter>]
    member val StoryId: Guid = Guid.Empty with get, set

    [<Inject>]
    member val StoryService: IStoryService = Unchecked.defaultof<_> with get, set

    member val LoadState: StoryEditorOps.LoadResult = StoryEditorOps.Loading with get, set

    override this.OnInitializedAsync() = task {
        let! result = StoryEditorOps.loadStory this.StoryService.GetById this.StoryId
        this.LoadState <- result
    }

    override this.Render() =
        match this.LoadState with
        | StoryEditorOps.Loading -> div { "Loading story..." }
        | StoryEditorOps.Loaded story -> storyView story
        | StoryEditorOps.NotFound -> div { "Story not found" }
```

**Why extract component logic:**

- **Testability**: `StoryEditorOps.loadStory` can be tested without Blazor runtime
- **State clarity**: `LoadResult` DU makes states explicit vs boolean flags
- **Reusability**: Loading logic can be reused across components

### Validation Attributes for Story Forms

```fsharp
open System.ComponentModel.DataAnnotations

// Story form model with validation
type StoryFormModel() =
    [<Required(ErrorMessage = "Title is required")>]
    [<StringLength(200, MinimumLength = 5,
        ErrorMessage = "Title must be 5-200 characters")>]
    member val Title = "" with get, set

    [<Required(ErrorMessage = "Situation context is required")>]
    [<MinLength(20, ErrorMessage = "Please provide more context (min 20 chars)")>]
    member val SituationContext = "" with get, set

    [<Required(ErrorMessage = "Task description is required")>]
    member val TaskChallenge = "" with get, set

    [<Required(ErrorMessage = "At least one action is required")>]
    member val Actions: ActionFormModel list = [] with get, set

    [<Required(ErrorMessage = "Result/outcome is required")>]
    member val ResultOutcome = "" with get, set

and ActionFormModel() =
    [<Required>]
    member val Description = "" with get, set
    member val Skills = "" with get, set
```

### Form Handling in Bolero

```fsharp
// Bolero form with validation feedback
let storyFormWithValidation (model: StoryEditorModel) (errors: Map<string, string list>) dispatch =
    div {
        attr.``class`` "story-form"

        // Title with validation
        div {
            attr.``class`` "form-group"
            label { attr.``for`` "title"; "Story Title" }
            input {
                attr.id "title"
                attr.``type`` "text"
                attr.value model.Title
                attr.``class`` (if errors.ContainsKey "Title" then "invalid" else "")
                on.change (fun e -> dispatch (SetTitle (unbox e.Value)))
            }
            cond (errors.TryFind "Title") <| function
                | Some errs ->
                    ul {
                        attr.``class`` "validation-errors"
                        forEach errs <| fun err -> li { err }
                    }
                | None -> empty ()
        }

        // ... other fields
    }
```

### Service Lifetime Considerations

```fsharp
// In Program.fs or Startup
// For Blazor Server: use Scoped (one per SignalR circuit)
// For Blazor WASM: Scoped behaves like Singleton

builder.Services.AddScoped<IStoryService, StoryService>()
builder.Services.AddScoped<IStoryRepository, StoryRepository>()

// Singleton for shared state (careful in Server mode!)
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>()

// Transient for stateless utilities
builder.Services.AddTransient<IStoryValidator, StoryValidator>()
```

### JavaScript Interop for Rich Text

```fsharp
// For rich text editing of story descriptions
[<Inject>]
member val JS: IJSRuntime = Unchecked.defaultof<_> with get, set

member this.InitRichTextEditor(elementId: string) = task {
    do! this.JS.InvokeVoidAsync("initQuillEditor", elementId)
}

member this.GetRichTextContent(elementId: string) = task {
    let! content = this.JS.InvokeAsync<string>("getQuillContent", elementId)
    return content
}
```

## Blazor Anti-Patterns to Avoid

| Anti-Pattern                | Problem                  | Solution                      |
| --------------------------- | ------------------------ | ----------------------------- |
| Static state in Server mode | Shared across users      | Use Scoped services           |
| Missing `StateHasChanged`   | UI doesn't update        | Call after async updates      |
| `firstRender` not checked   | JS runs on every render  | Check in `OnAfterRenderAsync` |
| Singleton user state        | Data leaks between users | Use Scoped for user data      |

See: `blazor#anti-patterns`

## Render Mode Decision

For Career Story Builder:

| Consideration      | Recommendation       |
| ------------------ | -------------------- |
| SEO needed?        | Server or Pre-render |
| Offline support?   | WebAssembly          |
| Real-time collab?  | Server (SignalR)     |
| Simple deployment? | Server               |
| Client-side perf?  | WebAssembly          |

See: `blazor#render-modes`
