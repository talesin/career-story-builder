module CareerStoryBuilder.Tests.UpdateTests

open Xunit
open CareerStoryBuilder.Client
open CareerStoryBuilder.Domain

[<Fact>]
let ``StartNewStory initializes conversation at InitialCapture`` () =
    // Arrange
    let model = Model.initial

    // Act
    let newModel, _ = Update.update StartNewStory model

    // Assert
    Assert.Equal(StoryWizard, newModel.Page)
    Assert.True(newModel.Conversation.IsSome)
    Assert.Equal(InitialCapture, newModel.Conversation.Value.CurrentStep)

[<Fact>]
let ``StartNewStory creates fresh conversation even if one exists`` () =
    // Arrange
    let existingConv = ConversationState.initial |> ConversationState.setStep Clarification
    let model = { Model.initial with Conversation = Some existingConv }

    // Act
    let newModel, _ = Update.update StartNewStory model

    // Assert
    Assert.Equal(InitialCapture, newModel.Conversation.Value.CurrentStep)

[<Fact>]
let ``SetPage Home clears conversation`` () =
    // Arrange
    let model = {
        Model.initial with
            Page = StoryWizard
            Conversation = Some ConversationState.initial
    }

    // Act
    let newModel, _ = Update.update (SetPage Home) model

    // Assert
    Assert.Equal(Home, newModel.Page)
    Assert.True(newModel.Conversation.IsNone)

[<Fact>]
let ``SetPage StoryWizard preserves existing conversation`` () =
    // Arrange
    let existingConv = ConversationState.initial |> ConversationState.setStep Clarification
    let model = { Model.initial with Conversation = Some existingConv }

    // Act
    let newModel, _ = Update.update (SetPage StoryWizard) model

    // Assert
    Assert.Equal(StoryWizard, newModel.Page)
    Assert.Equal(Clarification, newModel.Conversation.Value.CurrentStep)

[<Fact>]
let ``SetPage StoryWizard initializes conversation if none exists`` () =
    // Arrange
    let model = Model.initial

    // Act
    let newModel, _ = Update.update (SetPage StoryWizard) model

    // Assert
    Assert.Equal(StoryWizard, newModel.Page)
    Assert.True(newModel.Conversation.IsSome)
    Assert.Equal(InitialCapture, newModel.Conversation.Value.CurrentStep)
