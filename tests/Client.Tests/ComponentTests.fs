module CareerStoryBuilder.Tests.ComponentTests

open Xunit
open Bunit
open CareerStoryBuilder.Client.Main

type AppComponentTests() =
    inherit TestContext()

    [<Fact>]
    member this.``App renders Career Story Builder heading``() =
        // Arrange & Act
        let cut = this.RenderComponent<App>()

        // Assert
        let heading = cut.Find("h1")
        Assert.Contains("Career Story Builder", heading.TextContent)

    [<Fact>]
    member this.``App renders description paragraph``() =
        // Arrange & Act
        let cut = this.RenderComponent<App>()

        // Assert
        let paragraph = cut.Find("p")
        Assert.Contains("SAR story", paragraph.TextContent)

    [<Fact>]
    member this.``App renders with container class``() =
        // Arrange & Act
        let cut = this.RenderComponent<App>()

        // Assert - verify the DOM structure
        let container = cut.Find(".container")
        Assert.NotNull(container)

    [<Fact>]
    member this.``App has expected DOM structure``() =
        // Arrange & Act
        let cut = this.RenderComponent<App>()

        // Assert - verify h1 is inside container
        let container = cut.Find(".container")
        let heading = container.QuerySelector("h1")
        Assert.NotNull(heading)
