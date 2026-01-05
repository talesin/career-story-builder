# Delivery Plan

## MOSCOW Prioritization Key

- **(M)** Must Have - Core functionality required for viable product
- **(S)** Should Have - Important features that significantly enhance value
- **(C)** Could Have - Desirable features that can be deferred
- **(W)** Won't Have - Out of scope for initial releases

## Phase 1: Prototype / POC

**Goal:** Validate the core AI-assisted story creation workflow.

Minimal stories to test the iterative AI conversation for building STAR-format stories:

- ADD-01: Start New Story
- ADD-05: Initial Story Capture
- ADD-06: AI-Guided Clarification
- ADD-08: Iterative Refinement
- ADD-10: AI-Assisted Story Generation

**Technical approach:** Simple single-page UI, API key via config, in-memory state, no database or authentication.

## Phase 2: Authentication & API Setup

**Goal:** Enable user accounts and AI integration.

- AUTH-01: LinkedIn Sign-In
- AUTH-05: Sign Out
- SET-02: API Key Configuration
- SET-03: API Key Validation
- AUTH-02: Profile Import
- AUTH-03: Session Management

**Note:** Sessions use cookie/in-memory storage until database is introduced in Phase 3.

## Phase 3: Complete Wizard & Draft Management

**Goal:** Full story creation workflow with persistence.

- ADD-15: Save Draft
- ADD-18: Resume Draft
- ADD-17: Draft Status Indicator
- ADD-07: AI Suggestions
- ADD-11: Story Review Request

## Phase 4: Dashboard & Story List

**Goal:** View and manage created stories.

- DASH-01: View Story List
- DASH-02: Story Summary Display
- UPD-01: Edit Existing Story
- DEL-01: Delete Single Story
- DEL-02: Delete Confirmation

## Phase 5: Search, Filter & Enhanced Editing

**Goal:** Find and refine stories efficiently.

- DASH-03: Search Stories
- DASH-04: Filter Stories
- DASH-05: Sort Stories
- DASH-09: View Drafts
- UPD-03: Direct Section Editing

## Phase 6: Metadata & Tags

**Goal:** Add context and organization to stories.

- EMP-02: Add Role
- ADD-02: Associate Story with Role
- ADD-04: Set Story Time Period
- ADD-16: Add Tags to Story
- UPD-08: Manage Story Tags
- ADD-12: Review-Based Questions

## Phase 7: Employment History

**Goal:** Track career context for stories.

- EMP-01: View Employment History
- EMP-05: View Stories by Role
- UPD-06: Change Role Association

## Phase 8: Advanced Settings & Polish

**Goal:** Customization and remaining features.

- SET-01: Profile Management
- AUTH-04: Account Linking
- SET-04: API Provider Selection
- SET-05: Content Length Preferences
- UPD-02: Re-enter Wizard
- DEL-03: Bulk Delete

## Phase 9: Scoring & Quality

**Goal:** Add quality scoring and content sufficiency features.

- ADD-09: Content Sufficiency Indicator
- ADD-13: Final Scoring
- ADD-14: Scoring Criteria Visibility
- UPD-04: Request Re-Review

## Future Considerations (Won't Have - Initial Release)

- ADD-03: Associate Story with Project
- UPD-07: Change Project Association
- DASH-06: Filter by Role
- DASH-07: Filter by Project
- DASH-08: Filter by Tag
- EMP-03: Edit Role
- EMP-04: Delete Role
- EMP-06: Add Project to Role
- EMP-07: Edit Project
- EMP-08: Delete Project
- UPD-05: View Edit History
- SET-06: Scoring Criteria Customization
- SET-07: AI Interaction Preferences
- SET-08: Export Preferences
- SET-09: Notification Settings
- SET-10: Manage Tags
