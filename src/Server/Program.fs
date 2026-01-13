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
            style {
                """
                body {
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
                    max-width: 800px;
                    margin: 0 auto;
                    padding: 2rem;
                }
                h1 { color: #333; }
                .container { padding: 1rem; }
                .btn-primary {
                    background-color: #2563eb;
                    color: white;
                    border: none;
                    padding: 0.75rem 1.5rem;
                    border-radius: 0.5rem;
                    font-size: 1rem;
                    cursor: pointer;
                    transition: background-color 0.2s;
                }
                .btn-primary:hover { background-color: #1d4ed8; }
                .btn-large { padding: 1rem 2rem; font-size: 1.125rem; }
                .hero-actions { margin-top: 2rem; }
                .wizard-nav { margin-bottom: 1rem; }
                .back-link { color: #6b7280; text-decoration: none; }
                .back-link:hover { color: #374151; }
                .workflow-steps { display: flex; gap: 1rem; margin-bottom: 1.5rem; }
                .step { padding: 0.5rem 1rem; background: #f3f4f6; border-radius: 0.25rem; color: #6b7280; }
                .step.active { background: #2563eb; color: white; }
                .wizard-content { padding: 1.5rem; background: #fafafa; border: 1px solid #e5e7eb; border-radius: 0.5rem; }
                """
            }
        }
        body {
            div {
                attr.id "main"
                text "Loading..."
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
    // Disable prerendering to avoid NullReferenceException with router (NavigationManager not available during SSR)
    builder.Services.AddBoleroHost(prerendered = false, devToggle = true) |> ignore
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
