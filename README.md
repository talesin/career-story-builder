# Career Story Builder

An AI supported career story builder, helping professionals write STAR style career stories useful for building resumes, personal talent reviews or promotion documents.

## User Stories

### Authentication & Account Management

**AUTH-01: LinkedIn Sign-In**
As a new or returning user, I want to sign in using my LinkedIn account so that I can authenticate quickly without creating a separate password.

**AUTH-02: Profile Import**
As a first-time user signing in with LinkedIn, I want my basic profile information (name, headline, profile photo) to be imported automatically so that I don't need to enter it manually.

**AUTH-03: Session Management**
As a logged-in user, I want my session to remain active for a reasonable period so that I don't need to repeatedly authenticate during a work session.

**AUTH-04: Account Linking**
As a user, I want my account to remain linked to my LinkedIn identity so that I can seamlessly sign in across devices.

**AUTH-05: Sign Out**
As a user, I want to sign out of my account so that I can secure my session when using shared devices.

---

### Dashboard

**DASH-01: View Story List**
As a user, I want to see a list of all my career stories on a dashboard so that I can quickly review what I've created.

**DASH-02: Story Summary Display**
As a user, I want each story in my list to display a title, creation date, last modified date, and quality score so that I can identify and prioritize stories at a glance.

**DASH-03: Search Stories**
As a user, I want to search my stories by keyword so that I can quickly find specific experiences.

**DASH-04: Filter Stories**
As a user, I want to filter my stories by status (draft, in review, complete) or score range so that I can focus on stories needing attention.

**DASH-05: Sort Stories**
As a user, I want to sort my story list by date, title, or score so that I can organize my view according to my current needs.

**DASH-06: Filter by Role**
As a user, I want to filter my story list by role so that I can focus on stories from a specific position.

**DASH-07: Filter by Project**
As a user, I want to filter my story list by project so that I can view stories related to a specific initiative.

**DASH-08: Filter by Tag**
As a user, I want to filter my story list by tag so that I can find stories matching specific themes or skills.

**DASH-09: View Drafts**
As a user, I want to filter to see only draft stories so that I can prioritize completing unfinished work.

---

### Employment History

**EMP-01: View Employment History**
As a user, I want to view a list of my roles so that I can see my career timeline at a glance.

**EMP-02: Add Role**
As a user, I want to add a role including employer, job title, department, location, and period of employment so that I can build a complete employment history.

**EMP-03: Edit Role**
As a user, I want to edit an existing role's details so that I can correct or update my employment information.

**EMP-04: Delete Role**
As a user, I want to delete a role from my employment history so that I can remove positions that are no longer relevant.

**EMP-05: View Stories by Role**
As a user, I want to see which stories are associated with each role so that I can ensure adequate coverage of my experiences at each position.

**EMP-06: Add Project to Role**
As a user, I want to add projects within a role including name and time period so that I can organize stories around specific initiatives.

**EMP-07: Edit Project**
As a user, I want to edit project details so that my project information stays accurate.

**EMP-08: Delete Project**
As a user, I want to delete a project so that I can remove outdated or irrelevant project groupings.

---

### Add Story (Wizard Workflow)

**ADD-01: Start New Story**
As a user, I want to initiate a new story creation process so that I can document a career accomplishment.

**ADD-02: Associate Story with Role**
As a user, I want to select a role when creating a story so that each story is linked to a specific position in my employment history.

**ADD-03: Associate Story with Project**
As a user, I want to optionally associate a story with a project within my selected role so that I can group related accomplishments.

**ADD-04: Set Story Time Period**
As a user, I want to optionally specify a time period for when the story occurred so that I can provide temporal context for my accomplishments.

**ADD-05: Initial Story Capture**
As a user, I want to write an initial free-form description of my accomplishment so that I can capture the essence of my experience before structuring it.

**ADD-06: AI-Guided Clarification**
As a user, I want the AI to ask me clarifying questions about my initial story so that I can provide sufficient detail for each STAR component (Situation, Task, Action, Result).

**ADD-07: AI Suggestions**
As a user, I want the AI to offer suggestions and prompts when my content is thin so that I can recall and articulate important details I may have overlooked.

**ADD-08: Iterative Refinement**
As a user, I want to engage in a back-and-forth dialogue with the AI so that I can progressively develop my story until it meets quality thresholds.

**ADD-09: Content Sufficiency Indicator**
As a user, I want to see an indicator showing whether each STAR section has enough content (configurable, typically one paragraph per section) so that I know when my story is ready for review.

**ADD-10: AI-Assisted Story Generation**
As a user, I want the AI to help transform my raw input into a professionally written STAR-format story so that the final output is polished and interview-ready.

**ADD-11: Story Review Request**
As a user, I want to request an AI review of my completed story so that I can receive structured feedback before finalizing.

**ADD-12: Review-Based Questions**
As a user, I want the AI review to generate targeted follow-up questions so that I can further strengthen weak areas of my story.

**ADD-13: Final Scoring**
As a user, I want each STAR section to receive an individual score and my story to receive an overall score based on defined criteria so that I can objectively assess story quality.

**ADD-14: Scoring Criteria Visibility**
As a user, I want to understand the criteria used for scoring so that I know how to improve my stories.

**ADD-15: Save Draft**
As a user, I want to save my story at any point during the wizard process so that I can return and continue later without losing progress.

**ADD-16: Add Tags to Story**
As a user, I want to add free-form tags to my story with auto-complete suggestions from existing tags so that I can categorize stories consistently.

**ADD-17: Draft Status Indicator**
As a user, I want to clearly see when a story is in draft mode so that I know which stories need further work.

**ADD-18: Resume Draft**
As a user, I want to resume working on a draft story from where I left off so that I can continue the wizard workflow seamlessly.

---

### Update Story

**UPD-01: Edit Existing Story**
As a user, I want to open an existing story for editing so that I can make improvements or corrections.

**UPD-02: Re-enter Wizard**
As a user, I want to re-enter the guided wizard workflow for an existing story so that I can use AI assistance to enhance previously created content.

**UPD-03: Direct Section Editing**
As a user, I want to edit individual STAR sections directly without going through the full wizard so that I can make quick targeted updates.

**UPD-04: Request Re-Review**
As a user, I want to request a new AI review after making changes so that I can see updated scores and feedback.

**UPD-05: View Edit History**
As a user, I want to see a history of changes made to a story so that I can track how it has evolved over time.

**UPD-06: Change Role Association**
As a user, I want to change which role a story is associated with so that I can correct misassigned stories.

**UPD-07: Change Project Association**
As a user, I want to change or remove a story's project association so that I can reorganize my stories.

**UPD-08: Manage Story Tags**
As a user, I want to add or remove tags from an existing story so that I can refine how stories are categorized.

---

### Delete Story

**DEL-01: Delete Single Story**
As a user, I want to delete a story I no longer need so that I can keep my dashboard organized.

**DEL-02: Delete Confirmation**
As a user, I want to confirm before permanently deleting a story so that I don't accidentally lose important content.

**DEL-03: Bulk Delete**
As a user, I want to select and delete multiple stories at once so that I can efficiently clean up my story list.

---

### Settings & Configuration

**SET-01: Profile Management**
As a user, I want to update my profile information so that my account reflects current details.

**SET-02: API Key Configuration**
As a user, I want to enter and securely store my own AI provider API key so that the application can use AI features on my behalf.

**SET-03: API Key Validation**
As a user, I want the application to validate my API key when I enter it so that I know immediately if it's configured correctly.

**SET-04: API Provider Selection**
As a user, I want to select which AI provider to use (initially OpenAI, with more options in the future) so that I can choose the service that best fits my needs.

**SET-05: Content Length Preferences**
As a user, I want to configure the minimum content length for each STAR section so that the AI guidance aligns with my personal standards or target use case.

**SET-06: Scoring Criteria Customization**
As a user, I want to view and potentially adjust the weighting of scoring criteria so that the evaluation reflects what matters most for my career goals.

**SET-07: AI Interaction Preferences**
As a user, I want to configure how the AI interacts with me (e.g., question frequency, suggestion verbosity) so that the experience matches my preferred workflow.

**SET-08: Export Preferences**
As a user, I want to set default export formats and templates so that I can quickly generate outputs for resumes or reviews.

**SET-09: Notification Settings**
As a user, I want to control email or in-app notifications so that I'm informed about relevant updates without being overwhelmed.

**SET-10: Manage Tags**
As a user, I want to view and delete existing tags so that I can maintain a clean tagging vocabulary across my stories.

---

## Technology

### Operations

We will use docker and containers for both local development (running a local server in watch mode) and to build a production ready container that holds the web site and API.

#### Docker Strategy

##### Development Container

- Mount source code as a volume to enable live editing
- Run `dotnet watch` inside the container for hot reload
- Expose ports for both the application and any debugging tools
- Use a development-specific Dockerfile or compose profile that includes SDK tooling
- Environment variables configure connection strings, API keys, and debug settings

##### Production Container

- Multi-stage build: first stage compiles with the .NET SDK, second stage runs with the slim ASP.NET runtime
- Publish as a self-contained or framework-dependent app depending on size/startup tradeoffs
- Final image based on `mcr.microsoft.com/dotnet/aspnet` for minimal footprint
- No SDK, source code, or development dependencies in the final image
- Health checks configured for orchestrator readiness/liveness probes

##### Compose Configuration

- `docker-compose.yml` defines services for the app, database, and any supporting infrastructure (Redis, message queues)
- Separate `docker-compose.override.yml` or profiles for development vs production settings
- Named volumes for database persistence across container restarts
- Network isolation between services

##### Typical Workflow

1. `docker compose up` starts the full local environment
2. Code changes trigger hot reload via mounted volumes and `dotnet watch`
3. `docker compose build` creates production images
4. Images tagged and pushed to a container registry for deployment

### F# Backend + Frontend using Blazor (Tech Stack Overview)

#### Core platform

- ASP.NET Core  
  Hosting layer for HTTP APIs, SignalR hubs, authentication, middleware, and deployment. Supports both Blazor Server and Blazor WebAssembly hosting models.

- Blazor  
  Component-based web UI framework running on .NET. Supports WebAssembly (client-side) and Server (remote UI over SignalR).

#### F# frontend options

- Bolero  
  F#-first framework built on top of Blazor. Provides MVU-style architecture, typed HTML helpers, and F#-friendly abstractions.

- Raw Blazor Components in F#  
  Writing standard Blazor components directly in F# without Bolero. Lower abstraction and framework risk, but more verbose and less ergonomic.

#### Application models

- Bolero MVU  
  Elmish-style Model–View–Update loop layered over Blazor rendering. Centralized state, explicit messages, predictable updates.

- Blazor Component Model  
  Stateful components with lifecycle methods and dependency injection. Familiar to Blazor users, less opinionated than MVU.

#### Backend communication

- HTTP APIs (JSON)  
  Standard request-response APIs using HttpClient from Blazor WASM. Shared F# DTOs compile to both server and client.

- SignalR  
  Real-time messaging abstraction over WebSockets, SSE, or long polling. First-class support in Blazor. Suitable for live updates and push-based UI.

#### Shared code

- Shared F# Class Library  
  Domain types, DTOs, validation logic, and contracts shared between server and client (WASM).

- System.Text.Json  
  Built-in JSON serializer with good WebAssembly support and predictable performance.

#### State and effects

- Bolero Commands  
  Explicit side-effect handling in MVU-style applications, similar to Elmish commands.

- Dependency Injection (DI)  
  Unified DI model across ASP.NET Core and Blazor for services, clients, and infrastructure concerns.

#### Authentication and security

- ASP.NET Core Authentication  
  Cookie-based auth, JWT, or external identity providers (OIDC, Azure AD, Auth0).

- Blazor Authorization Components  
  Declarative authorization in UI using roles and policies.

#### Hosting models

- Blazor WebAssembly (Hosted)  
  Client runs in the browser; backend serves APIs and static assets. Clear frontend/backend boundary.

- Blazor Server  
  UI executes on the server and communicates over SignalR. No frontend deployment, but higher latency sensitivity and scalability considerations.

#### Persistence and infrastructure

- Entity Framework Core (F#-friendly usage)  
  ORM for relational databases, often wrapped to reduce OO friction in F#.

- Dapper / SQL-first access  
  Lightweight data access with explicit SQL and strong performance characteristics.

#### Tooling and build

- .NET SDK tooling  
  Single build and dependency system for backend and frontend. No Node.js required.

- Blazor Hot Reload  
  Supported, but reliability varies in complex F# projects.

#### Testing

- xUnit / Expecto  
  Unit and integration testing for shared domain logic and backend services.

- bUnit  
  Component testing framework for Blazor UI.

#### Operational considerations

- Payload size  
  Blazor WebAssembly apps ship the .NET runtime, leading to larger initial downloads than JS SPAs.

- Startup time  
  Slower cold-start compared to Fable or TypeScript SPAs; acceptable for authenticated or internal apps.