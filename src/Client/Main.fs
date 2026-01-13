module CareerStoryBuilder.Client.Main

open Elmish
open Bolero
open Bolero.Html
open CareerStoryBuilder.Client
open CareerStoryBuilder.Client.Views
open CareerStoryBuilder.Client.Views.Wizard
open CareerStoryBuilder.Domain

/// Router for client-side navigation.
let router = Router.infer SetPage _.Page

/// Convert workflow step to display name.
let workflowStepName = function
    | InitialCapture -> "Capture"
    | Clarification -> "Clarify"
    | Refinement -> "Refine"
    | Generation -> "Generate"

/// All workflow steps in order.
let allWorkflowSteps = [ InitialCapture; Clarification; Refinement; Generation ]

/// Render the appropriate wizard step content based on current step.
let wizardStepContent model dispatch =
    match model.Conversation |> Option.map _.CurrentStep with
    | Some InitialCapture ->
        let isProcessing =
            model.Conversation
            |> Option.map _.IsProcessing
            |> Option.defaultValue false
        InitialCapture.view model.InitialCaptureContent isProcessing dispatch
    | Some Clarification ->
        p { "AI clarification coming in Phase 1A.4..." }
    | Some Refinement ->
        p { "Story refinement coming in Phase 1A.5..." }
    | Some Generation ->
        p { "Story generation coming in Phase 1A.6..." }
    | None ->
        p { "No conversation state." }

/// Story wizard shell view with workflow step indicator.
let wizardShellView model dispatch =
    div {
        attr.``class`` "container wizard-shell"

        // Navigation back to home
        nav {
            attr.``class`` "wizard-nav"
            a {
                router.HRef Home
                attr.``class`` "back-link"
                "< Back"
            }
        }

        h1 { "Create Your Story" }

        // Workflow step indicator
        div {
            attr.``class`` "workflow-steps"
            for step in allWorkflowSteps do
                let isActive =
                    model.Conversation |> Option.exists (fun c -> c.CurrentStep = step)

                span {
                    attr.``class`` (if isActive then "step active" else "step")
                    workflowStepName step
                }
        }

        // Wizard step content
        div {
            attr.``class`` "wizard-content"
            wizardStepContent model dispatch
        }
    }

/// Main view function - routes to appropriate page view.
let view model dispatch =
    match model.Page with
    | Home -> Home.view dispatch
    | StoryWizard -> wizardShellView model dispatch

/// Main application component.
type App() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkProgram (fun _ -> Model.initial, Cmd.none) Update.update view
        |> Program.withRouter router
