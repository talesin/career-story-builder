module CareerStoryBuilder.Server.Program

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Bolero.Server
open Bolero.Html
open Bolero.Server.Html
open CareerStoryBuilder.Json
open CareerStoryBuilder.Dto
open CareerStoryBuilder.Server.Api

/// Static HTML page that hosts the Blazor client.
let indexPage =
    doctypeHtml {
        head {
            meta { attr.charset "UTF-8" }
            meta {
                attr.name "viewport"
                attr.content "width=device-width, initial-scale=1.0"
            }
            title { "Career Story Builder" }
            ``base`` { attr.href "/" }
        }
        body {
            div {
                attr.id "main"
                comp<CareerStoryBuilder.Client.Main.App>
            }
            boleroScript
        }
    }

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    // Add MVC services (required for Bolero.Server.Html rendering)
    builder.Services.AddControllersWithViews() |> ignore

    // Add Bolero hosting
    builder.Services.AddBoleroHost(prerendered = true, devToggle = true) |> ignore
    builder.Services.AddServerSideBlazor() |> ignore

    // Configure JSON serialization for F# types (discriminated unions, options, etc.)
    builder.Services.ConfigureHttpJsonOptions(fun opts ->
        configureOptions opts.SerializerOptions |> ignore
    ) |> ignore

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    app.UseStaticFiles() |> ignore
    app.UseRouting() |> ignore
    app.UseBlazorFrameworkFiles() |> ignore

    // Health check endpoint
    app.MapGet("/health", Func<string>(fun () -> "OK")) |> ignore

    // Conversation API endpoints
    app.MapPost("/api/conversation/clarify", Func<ClarifyRequest, ClarifyResponse>(ConversationApi.clarify)) |> ignore
    app.MapPost("/api/conversation/generate", Func<GenerateRequest, GenerateResponse>(ConversationApi.generate)) |> ignore

    // Blazor Hub for server-side interactivity
    app.MapBlazorHub() |> ignore

    // Fallback to Bolero page
    app.MapFallbackToBolero(indexPage) |> ignore

    app.Run()
    0
