namespace CareerStoryBuilder.Client

open Bolero
open CareerStoryBuilder.Domain

/// Application pages with routing endpoints.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/story/new">] StoryWizard

/// Application model.
type Model = {
    Page: Page
    Conversation: ConversationState option
    InitialCaptureContent: string
}

/// Application messages.
type Message =
    | SetPage of Page
    | StartNewStory
    | SetInitialCaptureContent of string
    | SubmitInitialCapture

module Model =
    let initial = { Page = Home; Conversation = None; InitialCaptureContent = "" }
