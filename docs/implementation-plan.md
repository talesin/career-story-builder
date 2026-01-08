# Implementation Plan: Career Story Builder

## Overview

This plan details the implementation approach for the Career Story Builder application. Phase 0 and 1 are detailed with specific dot releases; subsequent phases provide high-level guidance referencing the delivery plan.

## Progress Checklist

### Phase 0: Infrastructure Setup
- [x] 0.1 - Project Scaffolding
- [ ] 0.2 - Docker Configuration
- [ ] 0.3 - Static Placeholder Site
- [ ] 0.4 - Test Infrastructure

### Phase 1: Core AI Workflow (Prototype)
- [ ] 1.1 - Domain Types & API Contract
- [ ] 1.2 - Start New Story (ADD-01)
- [ ] 1.3 - Initial Story Capture (ADD-05)
- [ ] 1.4 - AI-Guided Clarification (ADD-06)
- [ ] 1.5 - Iterative Refinement (ADD-08)
- [ ] 1.6 - AI-Assisted Story Generation (ADD-10)

### Phase 2: Authentication & API Setup
- [ ] LinkedIn OAuth integration
- [ ] User session management
- [ ] Secure API key storage
- [ ] API key validation UI

### Phase 3: Complete Wizard & Draft Management
- [ ] PostgreSQL database integration
- [ ] Draft save/resume functionality
- [ ] Quality scoring system
- [ ] Review workflow

### Phase 4: Dashboard & Story List
- [ ] Story list view with cards/table
- [ ] Story detail view
- [ ] Edit and delete operations

### Phase 5: Search, Filter & Enhanced Editing
- [ ] Full-text search
- [ ] Filter/sort controls
- [ ] Direct section editing

### Phase 6: Metadata & Tags
- [ ] Role/project associations
- [ ] Tagging system
- [ ] Scoring criteria display

### Phase 7: Employment History
- [ ] Employment history management
- [ ] Role/project CRUD
- [ ] Story-to-role linking

### Phase 8: Advanced Settings & Polish
- [ ] Profile management
- [ ] AI provider selection
- [ ] Content preferences
- [ ] Bulk operations

---

## Phase 0: Infrastructure Setup

### 0.1 - Project Scaffolding

Create the F# solution structure with shared library pattern.

**Deliverables:**
- `CareerStoryBuilder.sln` - Solution file
- `src/Shared/Shared.fsproj` - Shared library (compiles to both server and WASM)
- `src/Server/Server.fsproj` - ASP.NET Core backend
- `src/Client/Client.fsproj` - Bolero/Blazor frontend
- `tests/Server.Tests/Server.Tests.fsproj` - Expecto tests
- `tests/Client.Tests/Client.Tests.fsproj` - bUnit tests

**Tasks:**
1. Create solution file and directory structure
2. Create Shared project with Phase 1 domain types from `docs/data-types.md`
3. Create Server project referencing Shared
4. Create Client project (Bolero) referencing Shared
5. Create test projects with Expecto and bUnit
6. Add `global.json` to pin .NET 10 SDK
7. Verify `dotnet build` succeeds

### 0.2 - Docker Configuration

Containerized development environment with hot reload.

**Deliverables:**
- `Dockerfile` - Multi-stage build
- `docker-compose.yml` - Development configuration
- `docker-compose.prod.yml` - Production configuration
- `scripts/build.sh` - Build helper
- `scripts/run.sh` - Run helper

**Tasks:**
1. Create Dockerfile with SDK and runtime stages
2. Create docker-compose.yml with app service (no DB for Phase 1)
3. Create shell scripts for common operations
4. Configure volume mounts for hot reload
5. Verify `docker compose up` builds and runs

### 0.3 - Static Placeholder Site

Minimal Bolero app to verify end-to-end pipeline.

**Deliverables:**
- Working Bolero page with "Career Story Builder" heading
- Server serving client assets
- Health check endpoint

**Tasks:**
1. Implement minimal Bolero `Main.fs` with static content
2. Configure Server to serve Blazor client
3. Add `/health` endpoint
4. Write smoke test verifying page loads
5. Verify hot reload works in Docker

### 0.4 - Test Infrastructure

Test-first setup with Expecto and bUnit.

**Deliverables:**
- Test project configurations
- Sample tests demonstrating patterns
- Test runner integration

**Tasks:**
1. Configure Expecto in Server.Tests with sample test
2. Configure bUnit in Client.Tests with sample component test
3. Add `dotnet test` to build scripts
4. Verify tests run in Docker environment

---

## Phase 1: Core AI Workflow (Prototype)

**Goal:** Validate the core AI-assisted story creation workflow.

**Technical approach:** Single-page UI, API key via environment variable, in-memory state, no database or authentication.

### 1.1 - Domain Types & API Contract

Implement shared types and API endpoints.

**User Stories:** Foundation for ADD-01, ADD-05, ADD-06, ADD-08, ADD-10

**Deliverables:**
- `Star` module types (Situation, Action, Result)
- `Story`, `ChatMessage`, `WorkflowStep`, `ConversationState` types
- API endpoint definitions
- DTOs for client-server communication

**Tasks:**
1. Implement types from `docs/data-types.md` in Shared project
2. Define API request/response DTOs
3. Create API endpoint stubs
4. Write unit tests for type serialization

### 1.2 - Start New Story (ADD-01)

User can begin creating a new story.

**Deliverables:**
- "New Story" button on main page
- Initializes conversation state
- Routes to story wizard view

**Tasks:**
1. Create `NewStoryButton` component with tests
2. Implement client-side routing
3. Create story wizard shell view
4. Add Bolero MVU model for conversation state
5. Write component tests

### 1.3 - Initial Story Capture (ADD-05)

Free-form text input for initial story content.

**Deliverables:**
- Text input area for story description
- Submit action to advance workflow
- Display of captured content

**Tasks:**
1. Create `InitialCaptureView` component
2. Implement form handling with validation
3. Update MVU model on submission
4. Write component tests for input handling

### 1.4 - AI-Guided Clarification (ADD-06)

AI asks follow-up questions to enhance story.

**Deliverables:**
- Chat-style UI showing AI questions
- User response input
- Conversation history display

**Tasks:**
1. Create `ChatView` component with message rendering
2. Implement `ClarificationApi` endpoint calling OpenAI
3. Design prompt for extracting clarifying questions
4. Handle streaming responses (optional, can batch)
5. Write tests with mocked AI responses

### 1.5 - Iterative Refinement (ADD-08)

User refines story sections based on AI feedback.

**Deliverables:**
- Section-by-section editing view
- AI suggestions per section
- Save section updates

**Tasks:**
1. Create `RefinementView` with SAR section editors
2. Implement refinement prompts for each section
3. Add inline suggestion display
4. Update conversation state with edits
5. Write component and integration tests

### 1.6 - AI-Assisted Story Generation (ADD-10)

AI generates complete SAR story from conversation.

**Deliverables:**
- "Generate Story" action
- Final story preview
- Copy/export functionality

**Tasks:**
1. Implement generation prompt with conversation context
2. Create `GenerationApi` endpoint
3. Build `StoryPreview` component
4. Add copy-to-clipboard functionality
5. Write end-to-end test for full workflow

---

## Phase 2: Authentication & API Setup (High-Level)

**Goal:** Enable user accounts and AI integration.

**Stories:** AUTH-01, AUTH-05, SET-02, SET-03, AUTH-02, AUTH-03

**Key Deliverables:**
- LinkedIn OAuth integration
- User session management
- Secure API key storage
- API key validation UI

**Notes:**
- Use ASP.NET Core Identity with external providers
- Store API keys encrypted in user settings
- Add authentication middleware

---

## Phase 3: Complete Wizard & Draft Management (High-Level)

**Goal:** Full story creation workflow with persistence.

**Stories:** ADD-15, ADD-18, ADD-17, ADD-07, ADD-09, ADD-11, ADD-13

**Key Deliverables:**
- PostgreSQL database integration
- Draft save/resume functionality
- Quality scoring system
- Review workflow

**Notes:**
- Add PostgreSQL to docker-compose
- Implement Dapper repositories
- Add background job for scoring

---

## Phase 4: Dashboard & Story List (High-Level)

**Goal:** View and manage created stories.

**Stories:** DASH-01, DASH-02, UPD-01, DEL-01, DEL-02

**Key Deliverables:**
- Story list view with cards/table
- Story detail view
- Edit and delete operations

---

## Phase 5: Search, Filter & Enhanced Editing (High-Level)

**Goal:** Find and refine stories efficiently.

**Stories:** DASH-03, DASH-04, DASH-05, DASH-09, UPD-03, UPD-04

**Key Deliverables:**
- Full-text search
- Filter/sort controls
- Direct section editing

---

## Phase 6: Metadata & Tags (High-Level)

**Goal:** Add context and organization to stories.

**Stories:** ADD-02, ADD-03, ADD-04, ADD-16, UPD-08, ADD-12, ADD-14

**Key Deliverables:**
- Role/project associations
- Tagging system
- Scoring criteria display

---

## Phase 7: Employment History (High-Level)

**Goal:** Track career context for stories.

**Stories:** EMP-01, EMP-02, EMP-05, UPD-06, UPD-07

**Key Deliverables:**
- Employment history management
- Role/project CRUD
- Story-to-role linking

---

## Phase 8: Advanced Settings & Polish (High-Level)

**Goal:** Customization and remaining features.

**Stories:** SET-01, AUTH-04, SET-04, SET-05, UPD-02, DEL-03

**Key Deliverables:**
- Profile management
- AI provider selection
- Content preferences
- Bulk operations

---

## File Structure (Phase 0-1)

```
CareerStoryBuilder/
├── CareerStoryBuilder.sln
├── global.json
├── Dockerfile
├── docker-compose.yml
├── docker-compose.prod.yml
├── scripts/
│   ├── build.sh
│   └── run.sh
├── src/
│   ├── Shared/
│   │   ├── Shared.fsproj
│   │   ├── Domain/
│   │   │   ├── Story.fs
│   │   │   └── Conversation.fs
│   │   └── Dto/
│   │       └── Api.fs
│   ├── Server/
│   │   ├── Server.fsproj
│   │   ├── Program.fs
│   │   └── Api/
│   │       ├── ClarificationApi.fs
│   │       └── GenerationApi.fs
│   └── Client/
│       ├── Client.fsproj
│       ├── Main.fs
│       ├── Model.fs
│       ├── Update.fs
│       └── Views/
│           ├── InitialCaptureView.fs
│           ├── ChatView.fs
│           ├── RefinementView.fs
│           └── StoryPreview.fs
├── tests/
│   ├── Server.Tests/
│   │   ├── Server.Tests.fsproj
│   │   └── ApiTests.fs
│   └── Client.Tests/
│       ├── Client.Tests.fsproj
│       └── ComponentTests.fs
└── docs/
    └── (existing documentation)
```
