module CareerStoryBuilder.Tests.ComponentTests

open Xunit
open CareerStoryBuilder.Client
open CareerStoryBuilder.Client.Views
open CareerStoryBuilder.Domain

/// Tests for view functions (rendering without full App/router setup)
module ViewTests =

    [<Fact>]
    let ``Home view can be called with dispatch``() =
        // The Home view function generates proper structure
        // We verify the view function exists and can be called
        let mutable buttonClicked = false
        let dispatch = function
            | StartNewStory -> buttonClicked <- true
            | _ -> ()

        // Verify the view function can be called without error
        let result = Home.view dispatch
        Assert.NotNull(box result)

    [<Fact>]
    let ``Model.initial starts at Home page``() =
        Assert.Equal(Home, Model.initial.Page)

    [<Fact>]
    let ``Model.initial has no conversation``() =
        Assert.True(Model.initial.Conversation.IsNone)

/// Tests for the Update function behavior
module UpdateBehaviorTests =

    [<Fact>]
    let ``Update returns Cmd.none for SetPage Home``() =
        let _, cmd = Update.update (SetPage Home) Model.initial
        // Cmd.none is an empty list
        Assert.True(cmd.IsEmpty)

    [<Fact>]
    let ``Update returns Cmd.none for StartNewStory``() =
        let _, cmd = Update.update StartNewStory Model.initial
        Assert.True(cmd.IsEmpty)

    [<Fact>]
    let ``Conversation is initialized with empty message list``() =
        let model, _ = Update.update StartNewStory Model.initial
        Assert.Empty(model.Conversation.Value.Messages)

    [<Fact>]
    let ``Conversation is not processing initially``() =
        let model, _ = Update.update StartNewStory Model.initial
        Assert.False(model.Conversation.Value.IsProcessing)

    [<Fact>]
    let ``Conversation has no draft story initially``() =
        let model, _ = Update.update StartNewStory Model.initial
        Assert.True(model.Conversation.Value.DraftStory.IsNone)

    [<Fact>]
    let ``Conversation has no error initially``() =
        let model, _ = Update.update StartNewStory Model.initial
        Assert.True(model.Conversation.Value.LastError.IsNone)
