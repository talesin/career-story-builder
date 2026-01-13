module CareerStoryBuilder.Client.Update

open Elmish
open CareerStoryBuilder.Domain

/// Update function - handles messages and returns new state with commands.
let update message model =
    match message with
    | SetPage Home ->
        // Clear conversation when returning home
        { model with Page = Home; Conversation = None }, Cmd.none

    | SetPage StoryWizard ->
        // Preserve existing conversation or initialize new one
        let conv = model.Conversation |> Option.defaultValue ConversationState.initial
        { model with Page = StoryWizard; Conversation = Some conv }, Cmd.none

    | StartNewStory ->
        // Always start fresh conversation
        { model with Page = StoryWizard; Conversation = Some ConversationState.initial }, Cmd.none
