# Test Plan

## Overview

This document defines test scenarios for the Career Story Builder application using BDD (Behavior-Driven Development) Given-When-Then syntax. Scenarios are organized by delivery phase, with Phase 1 containing detailed test cases and subsequent phases containing placeholders for future expansion.

### Format

Each scenario follows the structure:

- **Given**: The initial context or preconditions
- **When**: The action or event that triggers the behavior
- **Then**: The expected outcome or result

---

## Phase 1: Core AI-Assisted Story Creation

### ADD-01: Start New Story

> As a user, I want to initiate a new story creation process so that I can document a career accomplishment.

#### Scenario 1.1: User initiates new story from empty state

```gherkin
Given I am on the main application screen
  And I have no stories in progress
When I click the "New Story" button
Then I should see the story creation wizard
  And the wizard should be on the initial input step
```

#### Scenario 1.2: User initiates new story with existing stories

```gherkin
Given I am on the main application screen
  And I have existing stories in my account
When I click the "New Story" button
Then I should see the story creation wizard
  And my existing stories should remain unchanged
```

#### Scenario 1.3: User cannot start new story without API key configured

```gherkin
Given I am on the main application screen
  And I have not configured an API key
When I click the "New Story" button
Then I should see a prompt to configure my API key
  And the wizard should not open until API key is configured
```

---

### ADD-05: Initial Story Capture

> As a user, I want to write an initial free-form description of my accomplishment so that I can capture the essence of my experience before structuring it.

#### Scenario 5.1: User enters initial story description

```gherkin
Given I am on the initial story capture step of the wizard
When I enter a free-form description of my accomplishment
Then the text should be saved in the input field
  And I should see a character count indicator
```

#### Scenario 5.2: User submits description for AI processing

```gherkin
Given I am on the initial story capture step
  And I have entered a description of at least 50 characters
When I click "Continue" or "Get AI Feedback"
Then the system should send my description to the AI
  And I should see a loading indicator
```

#### Scenario 5.3: User attempts to continue with empty description

```gherkin
Given I am on the initial story capture step
  And the description field is empty
When I click "Continue"
Then I should see a validation message indicating text is required
  And I should remain on the current step
```

#### Scenario 5.4: User enters very short description

```gherkin
Given I am on the initial story capture step
  And I have entered fewer than 20 characters
When I click "Continue"
Then I should see a suggestion to provide more detail
  And I should be able to proceed anyway or add more content
```

---

### ADD-06: AI-Guided Clarification

> As a user, I want the AI to ask me clarifying questions about my initial story so that I can provide sufficient detail for each SAR component.

#### Scenario 6.1: AI identifies missing Situation details

```gherkin
Given I have submitted my initial story description
  And the AI has analyzed my input
  And my description lacks context about the situation
When the AI response is displayed
Then I should see a clarifying question about the situation
  And the question should ask about when, where, or what prompted the situation
```

#### Scenario 6.2: AI identifies missing Action details

```gherkin
Given I have submitted my initial story description
  And the AI has analyzed my input
  And my description lacks specific actions taken
When the AI response is displayed
Then I should see a clarifying question about actions
  And the question should ask what specific steps I took
```

#### Scenario 6.3: AI identifies missing Result details

```gherkin
Given I have submitted my initial story description
  And the AI has analyzed my input
  And my description lacks measurable outcomes
When the AI response is displayed
Then I should see a clarifying question about results
  And the question should ask about outcomes, metrics, or impact
```

#### Scenario 6.5: AI asks multiple clarifying questions

```gherkin
Given I have submitted a brief initial story description
  And the AI has analyzed my input
  And multiple SAR components need clarification
When the AI response is displayed
Then I should see multiple clarifying questions
  And the questions should be prioritized by importance
  And I should be able to answer them one at a time or all at once
```

#### Scenario 6.6: User provides answer to clarifying question

```gherkin
Given the AI has asked me a clarifying question
  And I see an input field to respond
When I type my answer and submit
Then the AI should incorporate my answer into the story context
  And the AI should acknowledge my response
  And the AI should ask the next clarifying question if needed
```

#### Scenario 6.7: AI handles API error gracefully

```gherkin
Given I have submitted my initial story description
  And the AI service is unavailable or returns an error
When the system attempts to get AI clarifications
Then I should see a friendly error message
  And I should be able to retry the request
  And my original input should be preserved
```

---

### ADD-08: Iterative Refinement

> As a user, I want to engage in a back-and-forth dialogue with the AI so that I can progressively develop my story until it meets quality thresholds.

#### Scenario 8.1: User continues dialogue after first response

```gherkin
Given I have answered the AI's initial clarifying questions
  And the AI has processed my responses
When the AI analyzes the updated story content
Then the AI should identify any remaining gaps
  And the AI should ask follow-up questions if needed
  Or the AI should indicate the story is ready for generation
```

#### Scenario 8.2: User provides additional context unprompted

```gherkin
Given I am in the refinement dialogue with the AI
  And I want to add information the AI hasn't asked about
When I enter additional context in the response field
Then the AI should incorporate this information
  And the AI should acknowledge the additional detail
```

#### Scenario 8.3: Story reaches sufficient detail threshold

```gherkin
Given I have been refining my story through dialogue
  And all SAR components have sufficient detail
When the AI analyzes the current story state
Then the AI should indicate the story is ready for generation
  And I should see a "Generate Story" or similar call-to-action
```

#### Scenario 8.4: User can view conversation history

```gherkin
Given I have had multiple exchanges with the AI
When I scroll through the refinement interface
Then I should see the full conversation history
  And each exchange should be clearly distinguished
  And my inputs should be visually separate from AI responses
```

#### Scenario 8.5: User can edit previous responses

```gherkin
Given I have provided answers to AI questions
  And I realize I want to change a previous answer
When I click to edit a previous response
Then I should be able to modify my answer
  And the AI should re-process the updated information
```

#### Scenario 8.6: Long refinement session maintains context

```gherkin
Given I have had more than 5 exchanges with the AI
When the AI generates its next response
Then the AI should remember all previous context
  And the AI should not repeat questions already answered
  And the story should incorporate all provided details
```

---

### ADD-10: AI-Assisted Story Generation

> As a user, I want the AI to help transform my raw input into a professionally written SAR-format story so that the final output is polished and interview-ready.

#### Scenario 10.1: AI generates complete SAR story

```gherkin
Given I have provided sufficient detail through the refinement process
  And I click "Generate Story"
When the AI processes my inputs
Then I should see a professionally formatted story
  And the story should have distinct Situation, Action, and Result sections
  And each section should incorporate my provided details
```

#### Scenario 10.2: Generated story uses professional language

```gherkin
Given the AI has generated my story
When I review the generated content
Then the language should be professional and interview-appropriate
  And action verbs should be strong and specific
  And the tone should be confident but not boastful
```

#### Scenario 10.3: User can regenerate story with different style

```gherkin
Given the AI has generated my story
  And I want a different tone or style
When I request a regeneration with style feedback
Then the AI should generate a new version
  And the new version should reflect my style preferences
  And my original input details should be preserved
```

#### Scenario 10.4: Generation handles API timeout gracefully

```gherkin
Given I have clicked "Generate Story"
  And the AI service takes longer than expected
When the request times out
Then I should see a timeout message
  And I should be able to retry the generation
  And my conversation history should be preserved
```

---

### ADD-15: Save Draft

> As a user, I want to save my story at any point during the wizard process so that I can return and continue later without losing progress.

#### Scenario 15.1: User saves draft during initial capture

```gherkin
Given I am on the initial story capture step
  And I have entered some text
When I click "Save Draft"
Then my progress should be saved
  And I should see a confirmation message
  And I should be able to continue working or exit
```

#### Scenario 15.2: User saves draft during AI refinement

```gherkin
Given I am in the AI refinement dialogue
  And I have had several exchanges with the AI
When I click "Save Draft"
Then my conversation history should be saved
  And all my inputs should be preserved
  And I should see a confirmation message
```

#### Scenario 15.3: User saves draft after story generation

```gherkin
Given the AI has generated my story
  And I have not yet finalized it
When I click "Save Draft"
Then the generated story should be saved
  And the draft status should be "Generated - Not Reviewed"
  And I should see a confirmation message
```

#### Scenario 15.4: Auto-save triggers periodically

```gherkin
Given I am working in the story wizard
  And I have made changes since the last save
When 30 seconds have passed without manual save
Then the system should auto-save my progress
  And I should see a subtle auto-save indicator
  And I should not be interrupted from my work
```

---

### SET-02: API Key Configuration

> As a user, I want to enter and securely store my own AI provider API key so that the application can use AI features on my behalf.

#### Scenario 2.1: User enters API key for first time

```gherkin
Given I am on the settings page
  And I have not configured an API key
When I enter my API key in the configuration field
  And I click "Save"
Then the API key should be stored securely
  And I should see a confirmation message
  And the key should be masked in the display
```

#### Scenario 2.2: User updates existing API key

```gherkin
Given I am on the settings page
  And I have a previously configured API key
When I enter a new API key
  And I click "Save"
Then the new API key should replace the old one
  And I should see a confirmation of the update
```

#### Scenario 2.3: User clears API key

```gherkin
Given I am on the settings page
  And I have a configured API key
When I clear the API key field
  And I click "Save"
Then the API key should be removed
  And AI features should be disabled until reconfigured
  And I should see a warning about disabled features
```

---

### SET-03: API Key Validation

> As a user, I want the application to validate my API key when I enter it so that I know immediately if it's configured correctly.

#### Scenario 3.1: Valid API key is accepted

```gherkin
Given I am on the settings page
  And I enter a valid API key
When I click "Validate" or "Save"
Then the system should test the API key with a simple request
  And I should see a success indicator
  And the key should be saved
```

#### Scenario 3.2: Invalid API key is rejected

```gherkin
Given I am on the settings page
  And I enter an invalid API key
When I click "Validate" or "Save"
Then the system should test the API key
  And I should see an error message indicating the key is invalid
  And the key should not be saved
```

#### Scenario 3.3: Expired API key is detected

```gherkin
Given I am on the settings page
  And I enter an expired API key
When I click "Validate"
Then the system should detect the expiration
  And I should see a message indicating the key has expired
  And I should be prompted to enter a new key
```

#### Scenario 3.4: Validation handles network errors

```gherkin
Given I am on the settings page
  And I enter an API key
  And the network is unavailable
When I click "Validate"
Then I should see a network error message
  And I should be given the option to save without validation
  Or I should be able to retry when connected
```

---

## Phase 2: Authentication & API

> Covers: AUTH-01, AUTH-02, AUTH-03, AUTH-04, AUTH-05

*Test scenarios to be defined when Phase 2 development begins.*

**User Stories:**

- AUTH-01 (M): LinkedIn Sign-In
- AUTH-02 (S): Profile Import
- AUTH-03 (S): Session Management
- AUTH-04 (C): Account Linking
- AUTH-05 (M): Sign Out

---

## Phase 3: Complete Wizard & Draft Management

> Covers: ADD-07, ADD-09, ADD-11, ADD-12, ADD-13, ADD-14, ADD-17, ADD-18

*Test scenarios to be defined when Phase 3 development begins.*

**User Stories:**

- ADD-07 (S): AI Suggestions
- ADD-09 (S): Content Sufficiency Indicator
- ADD-11 (S): Story Review Request
- ADD-12 (C): Review-Based Questions
- ADD-13 (S): Final Scoring
- ADD-14 (C): Scoring Criteria Visibility
- ADD-17 (S): Draft Status Indicator
- ADD-18 (S): Resume Draft

---

## Phase 4: Dashboard & Story List

> Covers: DASH-01, DASH-02

*Test scenarios to be defined when Phase 4 development begins.*

**User Stories:**

- DASH-01 (M): View Story List
- DASH-02 (S): Story Summary Display

---

## Phase 5: Search, Filter & Enhanced Editing

> Covers: DASH-03, DASH-04, DASH-05, DASH-09, UPD-01, UPD-02, UPD-03, UPD-04, DEL-01, DEL-02

*Test scenarios to be defined when Phase 5 development begins.*

**User Stories:**

- DASH-03 (S): Search Stories
- DASH-04 (S): Filter Stories
- DASH-05 (C): Sort Stories
- DASH-09 (C): View Drafts
- UPD-01 (M): Edit Existing Story
- UPD-02 (C): Re-enter Wizard
- UPD-03 (S): Direct Section Editing
- UPD-04 (S): Request Re-Review
- DEL-01 (M): Delete Single Story
- DEL-02 (M): Delete Confirmation

---

## Phase 6: Metadata & Tags

> Covers: ADD-02, ADD-03, ADD-04, ADD-16, UPD-06, UPD-07, UPD-08

*Test scenarios to be defined when Phase 6 development begins.*

**User Stories:**

- ADD-02 (C): Associate Story with Role
- ADD-03 (C): Associate Story with Project
- ADD-04 (C): Set Story Time Period
- ADD-16 (C): Add Tags to Story
- UPD-06 (C): Change Role Association
- UPD-07 (C): Change Project Association
- UPD-08 (C): Manage Story Tags

---

## Phase 7: Employment History

> Covers: EMP-01 through EMP-08

*Test scenarios to be defined when Phase 7 development begins.*

**User Stories:**

- EMP-01 (C): View Employment History
- EMP-02 (C): Add Role
- EMP-03 (W): Edit Role
- EMP-04 (W): Delete Role
- EMP-05 (C): View Stories by Role
- EMP-06 (W): Add Project to Role
- EMP-07 (W): Edit Project
- EMP-08 (W): Delete Project

---

## Phase 8: Advanced Settings & Polish

> Covers: SET-01, SET-04, SET-05, SET-06, SET-07, SET-08, SET-09, SET-10, DASH-06, DASH-07, DASH-08, UPD-05, DEL-03

*Test scenarios to be defined when Phase 8 development begins.*

**User Stories:**

- SET-01 (S): Profile Management
- SET-04 (C): API Provider Selection
- SET-05 (C): Content Length Preferences
- SET-06 (W): Scoring Criteria Customization
- SET-07 (W): AI Interaction Preferences
- SET-08 (W): Export Preferences
- SET-09 (W): Notification Settings
- SET-10 (W): Manage Tags
- DASH-06 (W): Filter by Role
- DASH-07 (W): Filter by Project
- DASH-08 (W): Filter by Tag
- UPD-05 (W): View Edit History
- DEL-03 (C): Bulk Delete
