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

// Phase 1A.3: Initial Capture Tests

[<Fact>]
let ``SetInitialCaptureContent updates model content`` () =
    // Arrange
    let model = Model.initial

    // Act
    let newModel, _ = Update.update (SetInitialCaptureContent "My story content") model

    // Assert
    Assert.Equal("My story content", newModel.InitialCaptureContent)

[<Fact>]
let ``SetInitialCaptureContent preserves other model state`` () =
    // Arrange
    let model = { Model.initial with Conversation = Some ConversationState.initial }

    // Act
    let newModel, _ = Update.update (SetInitialCaptureContent "content") model

    // Assert
    Assert.True(newModel.Conversation.IsSome)

[<Fact>]
let ``SubmitInitialCapture with empty content does nothing`` () =
    // Arrange
    let model = { Model.initial with InitialCaptureContent = "" }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.Equal("", newModel.InitialCaptureContent)
    Assert.True(newModel.Conversation.IsNone)

[<Fact>]
let ``SubmitInitialCapture with whitespace only does nothing`` () =
    // Arrange
    let model = { Model.initial with InitialCaptureContent = "   \n\t  " }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.True(newModel.Conversation.IsNone)

[<Fact>]
let ``SubmitInitialCapture adds user message to conversation`` () =
    // Arrange
    let model = {
        Model.initial with
            InitialCaptureContent = "My accomplishment story"
            Conversation = Some ConversationState.initial
    }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.True(newModel.Conversation.IsSome)
    let messages = newModel.Conversation.Value.Messages
    Assert.Single(messages) |> ignore
    Assert.Equal(User, messages.Head.Role)
    Assert.Equal("My accomplishment story", messages.Head.Content)

[<Fact>]
let ``SubmitInitialCapture advances to Clarification step`` () =
    // Arrange
    let model = {
        Model.initial with
            InitialCaptureContent = "My story"
            Conversation = Some ConversationState.initial
    }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.Equal(Clarification, newModel.Conversation.Value.CurrentStep)

[<Fact>]
let ``SubmitInitialCapture clears InitialCaptureContent`` () =
    // Arrange
    let model = {
        Model.initial with
            InitialCaptureContent = "My story"
            Conversation = Some ConversationState.initial
    }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.Equal("", newModel.InitialCaptureContent)

[<Fact>]
let ``SubmitInitialCapture initializes conversation if none exists`` () =
    // Arrange
    let model = {
        Model.initial with
            InitialCaptureContent = "My story"
            Conversation = None
    }

    // Act
    let newModel, _ = Update.update SubmitInitialCapture model

    // Assert
    Assert.True(newModel.Conversation.IsSome)
    Assert.Equal(Clarification, newModel.Conversation.Value.CurrentStep)

[<Fact>]
let ``StartNewStory clears InitialCaptureContent`` () =
    // Arrange
    let model = { Model.initial with InitialCaptureContent = "old content" }

    // Act
    let newModel, _ = Update.update StartNewStory model

    // Assert
    Assert.Equal("", newModel.InitialCaptureContent)

[<Fact>]
let ``SetPage Home clears InitialCaptureContent`` () =
    // Arrange
    let model = {
        Model.initial with
            Page = StoryWizard
            InitialCaptureContent = "content"
    }

    // Act
    let newModel, _ = Update.update (SetPage Home) model

    // Assert
    Assert.Equal("", newModel.InitialCaptureContent)
