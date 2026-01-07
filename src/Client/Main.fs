module CareerStoryBuilder.Client.Main

open Elmish
open Bolero
open Bolero.Html

/// Application model.
type Model = {
    Page: string
}

/// Application messages.
type Message =
    | NoOp

/// Initialize the application model.
let initModel = { Page = "home" }

/// Update function - handles messages and returns new state.
let update message model =
    match message with
    | NoOp -> model

/// View function - renders the UI.
let view model _dispatch =
    div {
        attr.``class`` "container"
        h1 { "Career Story Builder" }
        p { "AI-assisted STAR story creation for career interviews." }
    }

/// Main application component.
type App() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkSimple (fun _ -> initModel) update view
