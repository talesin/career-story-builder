# ASP.NET Core Backend Guide

> References: `$REFERENCES/aspnet/`

## Quick Links by Task

| Task | Reference |
|------|-----------|
| Configure DI | `$REFERENCES/aspnet/index.md#dependency-injection` |
| Load configuration | `$REFERENCES/aspnet/index.md#configuration` |
| Set up middleware | `$REFERENCES/aspnet/index.md#middleware` |
| Define routes | `$REFERENCES/aspnet/index.md#routing` |
| Create minimal APIs | `$REFERENCES/aspnet/index.md#minimal-apis` |
| Add authentication | `$REFERENCES/aspnet/index.md#authentication` |
| Add authorization | `$REFERENCES/aspnet/index.md#authorization` |
| Optimize performance | `$REFERENCES/aspnet/index.md#performance` |
| Write tests | `$REFERENCES/aspnet/index.md#testing` |

## Key Patterns for Career Story Builder

ASP.NET Core hosts the Bolero application and provides:
- API endpoints for story CRUD operations
- Authentication and user management
- Configuration and dependency injection
- Middleware pipeline for cross-cutting concerns

## Primary References

### Minimal APIs
- **Route Handlers**: `$REFERENCES/aspnet/index.md#minimal-apis`
  - Parameter binding
  - Response types
  - Route groups

### Dependency Injection
- **Service Lifetimes**: `$REFERENCES/aspnet/index.md#dependency-injection`
  - Singleton, Scoped, Transient
  - Keyed services

### Security
- **Authentication/Authorization**: `$REFERENCES/aspnet/index.md#security`
  - JWT Bearer tokens
  - Cookie authentication
  - Policy-based authorization

## Domain Examples

### Story API Endpoints

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
    stories.MapPost("/", fun (dto: CreateStoryDto) (service: IStoryService) -> task {
        let! result = service.Create(dto)
        return
            match result with
            | Ok story -> Results.Created($"/api/stories/{story.Id}", story)
            | Error errors -> Results.BadRequest(errors)
    }) |> ignore

    // PUT /api/stories/{id}
    stories.MapPut("/{id:guid}", fun (id: Guid) (dto: UpdateStoryDto) (service: IStoryService) -> task {
        let! result = service.Update(StoryId id, dto)
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

### Authentication Setup

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

// Usage in service
type StoryService(
    repository: IStoryRepository,
    dbOptions: IOptions<DatabaseOptions>,
    logger: ILogger<StoryService>) =

    member _.GetAll() = task {
        logger.LogInformation("Fetching all stories")
        return! repository.GetAll()
    }
```

## Middleware Order Reference

See: `$REFERENCES/aspnet/index.md#middleware`

```
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
