module CareerStoryBuilder.Tests.DomainTests

open Expecto
open CareerStoryBuilder.Domain

let storyTitleTests =
    testList "StoryTitle" [
        test "tryCreate rejects empty title" {
            let result = StoryTitle.tryCreate ""
            Expect.isError result "Empty title should fail"
        }

        test "tryCreate rejects whitespace title" {
            let result = StoryTitle.tryCreate "   "
            Expect.isError result "Whitespace title should fail"
        }

        test "tryCreate accepts valid title" {
            let result = StoryTitle.tryCreate "My Career Story"
            Expect.isOk result "Valid title should succeed"
        }

        test "tryCreate trims whitespace" {
            let result = StoryTitle.tryCreate "  My Story  "
            match result with
            | Ok title -> Expect.equal title.Value "My Story" "Should trim whitespace"
            | Error _ -> failtest "Expected Ok"
        }
    ]


let storyTests =
    testList "Story" [
        test "Story.empty creates empty story for forms" {
            let story = Story.empty
            Expect.equal story.Title.Value "" "Title should be empty"
            Expect.equal story.Situation.Value "" "Situation should be empty"
            Expect.equal story.Action.Value "" "Action should be empty"
            Expect.equal story.Result.Value "" "Result should be empty"
        }

        test "Story.tryCreate validates title" {
            let result =
                Story.tryCreate
                    ""  // Invalid empty title
                    (StorySituation "Context")
                    (StoryAction "Steps taken")
                    (StoryResult "Outcome")

            Expect.isError result "Should fail with empty title"
        }

        test "Story.tryCreate succeeds with valid title" {
            let result =
                Story.tryCreate
                    "Led migration project"
                    (StorySituation "Legacy system needed modernization")
                    (StoryAction "Designed migration strategy with rollback plan")
                    (StoryResult "Zero downtime, 40% performance improvement")

            Expect.isOk result "Should succeed with valid title"
            match result with
            | Ok story ->
                Expect.equal story.Title.Value "Led migration project" "Title should match"
                Expect.equal story.Situation.Value "Legacy system needed modernization" "Situation should match"
            | Error _ -> failtest "Expected Ok"
        }
    ]

let idTests =
    testList "IDs" [
        test "StoryId.create generates unique IDs" {
            let id1 = StoryId.create()
            let id2 = StoryId.create()
            Expect.notEqual id1 id2 "IDs should be unique"
        }

        test "StoryId.value extracts the Guid" {
            let id = StoryId.create()
            let guid = id.Value
            Expect.isNotNull (box guid) "Guid should not be null"
        }

        test "StoryId uses UUIDv7 (version 7)" {
            let id = StoryId.create()
            let guid = id.Value
            Expect.equal (guid.Version) 7 "Should be UUIDv7"
        }
    ]

let conversationTests =
    testList "Conversation" [
        test "ConversationState.initial starts in InitialCapture" {
            let state = ConversationState.initial
            Expect.equal state.CurrentStep InitialCapture "Should start in InitialCapture"
            Expect.isEmpty state.Messages "Messages should be empty"
            Expect.isNone state.DraftStory "DraftStory should be None"
            Expect.isFalse state.IsProcessing "Should not be processing"
        }

        test "ConversationState.addMessage prepends message" {
            let state = ConversationState.initial
            let message1 = ChatMessage.create User "First"
            let message2 = ChatMessage.create Assistant "Second"

            let newState =
                state
                |> ConversationState.addMessage message1
                |> ConversationState.addMessage message2

            Expect.hasLength newState.Messages 2 "Should have two messages"
            // Messages are stored in reverse order (newest first)
            Expect.equal newState.Messages[0].Content "Second" "First in list is newest"
            Expect.equal newState.Messages[1].Content "First" "Second in list is oldest"
        }

        test "ConversationState.messagesChronological returns chronological order" {
            let state = ConversationState.initial
            let message1 = ChatMessage.create User "First"
            let message2 = ChatMessage.create Assistant "Second"

            let newState =
                state
                |> ConversationState.addMessage message1
                |> ConversationState.addMessage message2

            let chronological = ConversationState.messagesOrdered newState

            Expect.equal chronological[0].Content "First" "First chronologically"
            Expect.equal chronological[1].Content "Second" "Second chronologically"
        }

        test "ConversationState.setStep changes workflow step" {
            let state = ConversationState.initial
            let newState = ConversationState.setStep Clarification state

            Expect.equal newState.CurrentStep Clarification "Step should be Clarification"
        }

        test "ChatMessage.withError sets typed error" {
            let message = ChatMessage.create User "Test"
            let errorMessage = ChatMessage.withError AiServiceUnavailable message

            Expect.equal errorMessage.Error (Some AiServiceUnavailable) "Should have error"
        }
    ]

[<Tests>]
let allTests =
    testList "Domain" [
        storyTitleTests
        storyTests
        idTests
        conversationTests
    ]
