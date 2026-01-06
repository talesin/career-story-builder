# User Stories

## Authentication & Account Management

**AUTH-01 (M): LinkedIn Sign-In**
As a new or returning user, I want to sign in using my LinkedIn account so that I can authenticate quickly without creating a separate password.

**AUTH-02 (S): Profile Import**
As a first-time user signing in with LinkedIn, I want my basic profile information (name, headline, profile photo) to be imported automatically so that I don't need to enter it manually.

**AUTH-03 (S): Session Management**
As a logged-in user, I want my session to remain active for a reasonable period so that I don't need to repeatedly authenticate during a work session.

**AUTH-04 (C): Account Linking**
As a user, I want my account to remain linked to my LinkedIn identity so that I can seamlessly sign in across devices.

**AUTH-05 (M): Sign Out**
As a user, I want to sign out of my account so that I can secure my session when using shared devices.

---

## Dashboard

**DASH-01 (M): View Story List**
As a user, I want to see a list of all my career stories on a dashboard so that I can quickly review what I've created.

**DASH-02 (S): Story Summary Display**
As a user, I want each story in my list to display a title, creation date, last modified date, and quality score so that I can identify and prioritize stories at a glance.

**DASH-03 (S): Search Stories**
As a user, I want to search my stories by keyword so that I can quickly find specific experiences.

**DASH-04 (S): Filter Stories**
As a user, I want to filter my stories by status (draft, in review, complete) or score range so that I can focus on stories needing attention.

**DASH-05 (C): Sort Stories**
As a user, I want to sort my story list by date, title, or score so that I can organize my view according to my current needs.

**DASH-06 (W): Filter by Role**
As a user, I want to filter my story list by role so that I can focus on stories from a specific position.

**DASH-07 (W): Filter by Project**
As a user, I want to filter my story list by project so that I can view stories related to a specific initiative.

**DASH-08 (W): Filter by Tag**
As a user, I want to filter my story list by tag so that I can find stories matching specific themes or skills.

**DASH-09 (C): View Drafts**
As a user, I want to filter to see only draft stories so that I can prioritize completing unfinished work.

---

## Employment History

**EMP-01 (C): View Employment History**
As a user, I want to view a list of my roles so that I can see my career timeline at a glance.

**EMP-02 (C): Add Role**
As a user, I want to add a role including employer, job title, department, location, and period of employment so that I can build a complete employment history.

**EMP-03 (W): Edit Role**
As a user, I want to edit an existing role's details so that I can correct or update my employment information.

**EMP-04 (W): Delete Role**
As a user, I want to delete a role from my employment history so that I can remove positions that are no longer relevant.

**EMP-05 (C): View Stories by Role**
As a user, I want to see which stories are associated with each role so that I can ensure adequate coverage of my experiences at each position.

**EMP-06 (W): Add Project to Role**
As a user, I want to add projects within a role including name and time period so that I can organize stories around specific initiatives.

**EMP-07 (W): Edit Project**
As a user, I want to edit project details so that my project information stays accurate.

**EMP-08 (W): Delete Project**
As a user, I want to delete a project so that I can remove outdated or irrelevant project groupings.

---

## Add Story (Wizard Workflow)

**ADD-01 (M): Start New Story**
As a user, I want to initiate a new story creation process so that I can document a career accomplishment.

**ADD-02 (C): Associate Story with Role**
As a user, I want to select a role when creating a story so that each story is linked to a specific position in my employment history.

**ADD-03 (C): Associate Story with Project**
As a user, I want to optionally associate a story with a project within my selected role so that I can group related accomplishments.

**ADD-04 (C): Set Story Time Period**
As a user, I want to optionally specify a time period for when the story occurred so that I can provide temporal context for my accomplishments.

**ADD-05 (M): Initial Story Capture**
As a user, I want to write an initial free-form description of my accomplishment so that I can capture the essence of my experience before structuring it.

**ADD-06 (M): AI-Guided Clarification**
As a user, I want the AI to ask me clarifying questions about my initial story so that I can provide sufficient detail for each STAR component (Situation, Task, Action, Result).

**ADD-07 (S): AI Suggestions**
As a user, I want the AI to offer suggestions and prompts when my content is thin so that I can recall and articulate important details I may have overlooked.

**ADD-08 (M): Iterative Refinement**
As a user, I want to engage in a back-and-forth dialogue with the AI so that I can progressively develop my story until it meets quality thresholds.

**ADD-09 (S): Content Sufficiency Indicator**
As a user, I want to see an indicator showing whether each STAR section has enough content (configurable, typically one paragraph per section) so that I know when my story is ready for review.

**ADD-10 (M): AI-Assisted Story Generation**
As a user, I want the AI to help transform my raw input into a professionally written STAR-format story so that the final output is polished and interview-ready.

**ADD-11 (S): Story Review Request**
As a user, I want to request an AI review of my completed story so that I can receive structured feedback before finalizing.

**ADD-12 (C): Review-Based Questions**
As a user, I want the AI review to generate targeted follow-up questions so that I can further strengthen weak areas of my story.

**ADD-13 (S): Final Scoring**
As a user, I want each STAR section to receive an individual score and my story to receive an overall score based on defined criteria so that I can objectively assess story quality.

**ADD-14 (C): Scoring Criteria Visibility**
As a user, I want to understand the criteria used for scoring so that I know how to improve my stories.

**ADD-15 (M): Save Draft**
As a user, I want to save my story at any point during the wizard process so that I can return and continue later without losing progress.

**ADD-16 (C): Add Tags to Story**
As a user, I want to add free-form tags to my story with auto-complete suggestions from existing tags so that I can categorize stories consistently.

**ADD-17 (S): Draft Status Indicator**
As a user, I want to clearly see when a story is in draft mode so that I know which stories need further work.

**ADD-18 (S): Resume Draft**
As a user, I want to resume working on a draft story from where I left off so that I can continue the wizard workflow seamlessly.

---

## Update Story

**UPD-01 (M): Edit Existing Story**
As a user, I want to open an existing story for editing so that I can make improvements or corrections.

**UPD-02 (C): Re-enter Wizard**
As a user, I want to re-enter the guided wizard workflow for an existing story so that I can use AI assistance to enhance previously created content.

**UPD-03 (S): Direct Section Editing**
As a user, I want to edit individual STAR sections directly without going through the full wizard so that I can make quick targeted updates.

**UPD-04 (S): Request Re-Review**
As a user, I want to request a new AI review after making changes so that I can see updated scores and feedback.

**UPD-05 (W): View Edit History**
As a user, I want to see a history of changes made to a story so that I can track how it has evolved over time.

**UPD-06 (C): Change Role Association**
As a user, I want to change which role a story is associated with so that I can correct misassigned stories.

**UPD-07 (C): Change Project Association**
As a user, I want to change or remove a story's project association so that I can reorganize my stories.

**UPD-08 (C): Manage Story Tags**
As a user, I want to add or remove tags from an existing story so that I can refine how stories are categorized.

---

## Delete Story

**DEL-01 (M): Delete Single Story**
As a user, I want to delete a story I no longer need so that I can keep my dashboard organized.

**DEL-02 (M): Delete Confirmation**
As a user, I want to confirm before permanently deleting a story so that I don't accidentally lose important content.

**DEL-03 (C): Bulk Delete**
As a user, I want to select and delete multiple stories at once so that I can efficiently clean up my story list.

---

## Settings & Configuration

**SET-01 (S): Profile Management**
As a user, I want to update my profile information so that my account reflects current details.

**SET-02 (M): API Key Configuration**
As a user, I want to enter and securely store my own AI provider API key so that the application can use AI features on my behalf.

**SET-03 (M): API Key Validation**
As a user, I want the application to validate my API key when I enter it so that I know immediately if it's configured correctly.

**SET-04 (C): API Provider Selection**
As a user, I want to select which AI provider to use (initially OpenAI, with more options in the future) so that I can choose the service that best fits my needs.

**SET-05 (C): Content Length Preferences**
As a user, I want to configure the minimum content length for each STAR section so that the AI guidance aligns with my personal standards or target use case.

**SET-06 (W): Scoring Criteria Customization**
As a user, I want to view and potentially adjust the weighting of scoring criteria so that the evaluation reflects what matters most for my career goals.

**SET-07 (W): AI Interaction Preferences**
As a user, I want to configure how the AI interacts with me (e.g., question frequency, suggestion verbosity) so that the experience matches my preferred workflow.

**SET-08 (W): Export Preferences**
As a user, I want to set default export formats and templates so that I can quickly generate outputs for resumes or reviews.

**SET-09 (W): Notification Settings**
As a user, I want to control email or in-app notifications so that I'm informed about relevant updates without being overwhelmed.

**SET-10 (W): Manage Tags**
As a user, I want to view and delete existing tags so that I can maintain a clean tagging vocabulary across my stories.
