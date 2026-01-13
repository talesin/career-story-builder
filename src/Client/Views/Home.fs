module CareerStoryBuilder.Client.Views.Home

open Bolero.Html
open CareerStoryBuilder.Client

/// Home page view with New Story button.
let view dispatch =
    div {
        attr.``class`` "container"

        h1 { "Career Story Builder" }
        p { "AI-assisted SAR story creation for career interviews." }

        div {
            attr.``class`` "hero-actions"
            button {
                attr.``class`` "btn-primary btn-large"
                attr.aria "label" "Start creating a new career story"
                on.click (fun _ -> dispatch StartNewStory)
                "New Story"
            }
        }
    }
