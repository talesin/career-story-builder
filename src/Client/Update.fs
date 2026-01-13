module CareerStoryBuilder.Client.Update

open System
open Elmish
open CareerStoryBuilder.Domain

/// Update function - handles messages and returns new state with commands.
let update message model =
    match message with
    | SetPage Home ->
        // Clear conversation and capture content when returning home
        { model with Page = Home; Conversation = None; InitialCaptureContent = "" }, Cmd.none

    | SetPage StoryWizard ->
        // Preserve existing conversation or initialize new one
        let conv = model.Conversation |> Option.defaultValue ConversationState.initial
        { model with Page = StoryWizard; Conversation = Some conv }, Cmd.none

    | StartNewStory ->
        // Always start fresh conversation and clear capture content
        { model with
            Page = StoryWizard
            Conversation = Some ConversationState.initial
            InitialCaptureContent = "" }, Cmd.none

    | SetInitialCaptureContent content ->
        // Update textarea content
        { model with InitialCaptureContent = content }, Cmd.none

    | SubmitInitialCapture ->
        // Validate content - do nothing if empty/whitespace
        if String.IsNullOrWhiteSpace model.InitialCaptureContent then
            model, Cmd.none
        else
            // Add user message to conversation and advance to Clarification
            let userMessage = ChatMessage.create User model.InitialCaptureContent
            let updatedConv =
                model.Conversation
                |> Option.defaultValue ConversationState.initial
                |> ConversationState.addMessage userMessage
                |> ConversationState.setStep Clarification
            { model with
                Conversation = Some updatedConv
                InitialCaptureContent = "" }, Cmd.none
