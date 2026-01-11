# ASP.NET Core Backend Guide

## Key Patterns for Career Story Builder

ASP.NET Core hosts the Bolero application and provides:

- API endpoints for story CRUD operations
- Authentication and user management
- Configuration and dependency injection
- Middleware pipeline for cross-cutting concerns

## Domain Examples

### Story API Endpoints

Reference: `aspnet#minimal-apis`, `aspnet#routing`

```fsharp
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http

let configureStoryEndpoints (app: WebApplication) =
    let stories = app.MapGroup("/api/stories")
                     .RequireAuthorization()

    // GET /api/stories
    stories.MapGet("/", fun (service: IStoryService) -> task {
        let! stories = service.GetAll()
        return Results.Ok(stories)
    }) |> ignore

    // GET /api/stories/{id}
    stories.MapGet("/{id:guid}", fun (id: Guid) (service: IStoryService) -> task {
        let! story = service.GetById(StoryId id)
        return
            match story with
            | Some s -> Results.Ok(s)
            | None -> Results.NotFound()
    }) |> ignore

    // POST /api/stories
    stories.MapPost("/", fun (request: CreateStoryRequest) (service: IStoryService) -> task {
        let! result = service.Create(request)
        return
            match result with
            | Ok story -> Results.Created($"/api/stories/{story.Id}", story)
            | Error errors -> Results.BadRequest(errors)
    }) |> ignore

    // PUT /api/stories/{id}
    stories.MapPut("/{id:guid}", fun (id: Guid) (request: UpdateStoryRequest) (service: IStoryService) -> task {
        let! result = service.Update(StoryId id, request)
        return
            match result with
            | Ok story -> Results.Ok(story)
            | Error errors -> Results.BadRequest(errors)
    }) |> ignore

    // DELETE /api/stories/{id}
    stories.MapDelete("/{id:guid}", fun (id: Guid) (service: IStoryService) -> task {
        do! service.Delete(StoryId id)
        return Results.NoContent()
    }) |> ignore

    app
```

### Service Registration

Reference: `aspnet#dependency-injection`

```fsharp
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) (config: IConfiguration) =
    // Configuration
    services.Configure<DatabaseOptions>(config.GetSection("Database")) |> ignore
    services.Configure<AuthOptions>(config.GetSection("Auth")) |> ignore

    // Repositories (Scoped - one per request)
    services.AddScoped<IStoryRepository, StoryRepository>() |> ignore
    services.AddScoped<IUserRepository, UserRepository>() |> ignore

    // Services (Scoped)
    services.AddScoped<IStoryService, StoryService>() |> ignore
    services.AddScoped<IStoryValidator, StoryValidator>() |> ignore

    // Database connection (Scoped)
    services.AddScoped<IDbConnection>(fun sp ->
        let options = sp.GetRequiredService<IOptions<DatabaseOptions>>()
        new NpgsqlConnection(options.Value.ConnectionString) :> IDbConnection
    ) |> ignore

    services
```

### Wrapping Functional Code for Framework DI

When ASP.NET Core requires constructor injection (controllers, hosted services), wrap functional core in thin classes at the interop boundary.

```fsharp
// Functional core remains pure
module Billing =
    type BillingDeps =
        { ChargeCard: string -> decimal -> unit
          Log: string -> unit }

    let charge (deps: BillingDeps) (customerId: string) (amount: decimal) =
        deps.Log $"Charging {customerId} amount {amount}"
        deps.ChargeCard customerId amount

// Thin wrapper for framework DI
type BillingService(deps: Billing.BillingDeps) =
    member _.Charge(customerId: string, amount: decimal) =
        Billing.charge deps customerId amount

// Register in composition root
let configureBillingServices (services: IServiceCollection) =
    services.AddSingleton<Billing.BillingDeps>(fun _ ->
        { ChargeCard = PaymentGateway.charge
          Log = fun msg -> printfn "%s" msg }
    ) |> ignore
    services.AddScoped<BillingService>() |> ignore
    services
```

**Guidelines**:

- Only add wrappers at the edges (controllers, hosted services)
- Never embed core business logic in the wrapper
- Wrapper methods should be one-liners that delegate to functional code
- Keep the functional core testable without the wrapper

See: [F# Style Guide - Split Module/Class Pattern](fsharp-style-guide.md#split-moduleclass-pattern-for-framework-interop) for detailed guidance and [Design Patterns Guide](design-patterns-guide.md#functional-dependency-injection) for DI pattern details.

### Authentication Setup

Reference: `aspnet#authentication`, `aspnet#security`

```fsharp
open Microsoft.AspNetCore.Authentication.JwtBearer

let configureAuth (builder: WebApplicationBuilder) =
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(fun options ->
            options.TokenValidationParameters <- TokenValidationParameters(
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            )
        ) |> ignore

    builder.Services.AddAuthorization() |> ignore
    builder
```

### Middleware Pipeline

Reference: `aspnet#middleware`

```fsharp
let configureMiddleware (app: WebApplication) =
    // 1. Exception handling (first to catch all)
    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore
    else
        app.UseExceptionHandler("/Error") |> ignore
        app.UseHsts() |> ignore

    // 2. HTTPS redirection
    app.UseHttpsRedirection() |> ignore

    // 3. Static files (for Blazor WASM)
    app.UseStaticFiles() |> ignore

    // 4. Routing
    app.UseRouting() |> ignore

    // 5. CORS (if needed for separate frontend)
    app.UseCors("AllowBlazor") |> ignore

    // 6. Authentication
    app.UseAuthentication() |> ignore

    // 7. Authorization
    app.UseAuthorization() |> ignore

    // 8. Blazor endpoints
    app.UseBlazorFrameworkFiles() |> ignore

    app
```

### Error Handling Middleware

Convert domain errors to appropriate HTTP responses:

```fsharp
open Microsoft.AspNetCore.Diagnostics

// Domain error types
type DomainError =
    | NotFound of string
    | ValidationFailed of string list
    | Unauthorized of string
    | Conflict of string

// Middleware to convert domain errors to HTTP responses
let configureDomainErrorHandler (app: WebApplication) =
    app.Use(fun context next -> task {
        try
            do! next.Invoke()
        with
        | :? DomainException as ex ->
            context.Response.ContentType <- "application/json"
            let (statusCode, body) =
                match ex.Error with
                | NotFound msg ->
                    (StatusCodes.Status404NotFound,
                     {| error = "NotFound"; message = msg |})
                | ValidationFailed errors ->
                    (StatusCodes.Status400BadRequest,
                     {| error = "ValidationFailed"; messages = errors |})
                | Unauthorized msg ->
                    (StatusCodes.Status401Unauthorized,
                     {| error = "Unauthorized"; message = msg |})
                | Conflict msg ->
                    (StatusCodes.Status409Conflict,
                     {| error = "Conflict"; message = msg |})
            context.Response.StatusCode <- statusCode
            do! context.Response.WriteAsJsonAsync(body)
    }) |> ignore
    app

// Alternative: Result-based error handling in endpoints
let handleResult (result: Result<'T, DomainError>) =
    match result with
    | Ok value -> Results.Ok(value)
    | Error (NotFound msg) -> Results.NotFound({| message = msg |})
    | Error (ValidationFailed errors) -> Results.BadRequest({| errors = errors |})
    | Error (Unauthorized msg) -> Results.Unauthorized()
    | Error (Conflict msg) -> Results.Conflict({| message = msg |})
```

### Program.fs Setup

```fsharp
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    // Configure services
    configureServices builder.Services builder.Configuration |> ignore
    configureAuth builder |> ignore

    // Add Bolero
    builder.Services.AddBoleroHost() |> ignore
    builder.Services.AddBoleroRemoting<IStoryService, StoryRemoteHandler>() |> ignore

    let app = builder.Build()

    // Configure middleware
    configureMiddleware app |> ignore

    // Configure endpoints
    configureStoryEndpoints app |> ignore
    app.MapBoleroRemoting<IStoryService>() |> ignore
    app.MapFallbackToFile("index.html") |> ignore

    app.Run()
    0
```

### Options Pattern for Configuration

Reference: `aspnet#configuration`

```fsharp
// Configuration types
type DatabaseOptions() =
    member val ConnectionString = "" with get, set
    member val MaxRetryCount = 3 with get, set

type AuthOptions() =
    member val Issuer = "" with get, set
    member val Audience = "" with get, set
    member val Key = "" with get, set
    member val ExpirationMinutes = 60 with get, set

// Module: Contains service logic with explicit dependencies
module StoryOperations =
    let getAll (log: string -> unit) (getAllStories: unit -> Task<Story list>) = task {
        log "Fetching all stories"
        return! getAllStories()
    }

    let getById (log: string -> unit) (getStory: StoryId -> Task<Story option>) (id: StoryId) = task {
        log $"Fetching story {id}"
        return! getStory id
    }

// Thin wrapper: Adapts module functions for DI
type StoryService(
    repository: IStoryRepository,
    logger: ILogger<StoryService>) =

    member _.GetAll() =
        StoryOperations.getAll
            (fun msg -> logger.LogInformation(msg))
            repository.GetAll

    member _.GetById(id) =
        StoryOperations.getById
            (fun msg -> logger.LogInformation(msg))
            repository.GetById
            id
```

Corresponding appsettings.json structure:

```json
{
  "Database": {
    "ConnectionString": "Host=localhost;Database=career_stories;Username=app;Password=secret",
    "MaxRetryCount": 3
  },
  "Auth": {
    "Issuer": "career-story-builder",
    "Audience": "career-story-builder-client",
    "Key": "your-256-bit-secret-key-here",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Middleware Order Reference

See: `aspnet#middleware`

```text
1. Exception handling (first)
2. HTTPS redirection
3. Static files
4. Routing
5. CORS
6. Authentication
7. Authorization
8. Custom middleware
9. Endpoints (last)
```

## Testing

For API endpoint and integration testing patterns, see [Testing Guide](testing-guide.md).

## See Also

- `aspnet#authorization` - examples TBD
- `aspnet#performance` - examples TBD
- `aspnet#testing` - examples TBD
