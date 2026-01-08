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
