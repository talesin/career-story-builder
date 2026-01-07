module CareerStoryBuilder.Tests.DomainTests

open Expecto
open CareerStoryBuilder.Domain

/// Extract string value from Star.Situation
let situationValue (Star.Situation s) = s

/// Extract string value from Star.Task
let taskValue (Star.Task s) = s

/// Extract string value from Star.Action
let actionValue (Star.Action s) = s

/// Extract string value from Star.Result
let resultValue (Star.Result s) = s

let storyTests =
    testList "Story" [
        test "Story.empty creates empty story" {
            let story = Story.empty
            Expect.equal story.Title "" "Title should be empty"
            Expect.equal (situationValue story.Situation) "" "Situation should be empty"
            Expect.equal (taskValue story.Task) "" "Task should be empty"
            Expect.equal (actionValue story.Action) "" "Action should be empty"
            Expect.equal (resultValue story.Result) "" "Result should be empty"
        }

        test "Story.create creates story with values" {
            let story =
                Story.create
                    "Led migration project"
                    (Star.Situation "Legacy system needed modernization")
                    (Star.Task "Migrate 500k records to new platform")
                    (Star.Action "Designed migration strategy with rollback plan")
                    (Star.Result "Zero downtime, 40% performance improvement")

            Expect.equal story.Title "Led migration project" "Title should match"
            Expect.equal (situationValue story.Situation) "Legacy system needed modernization" "Situation should match"
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
            let guid = StoryId.value id
            Expect.isNotNull (box guid) "Guid should not be null"
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

        test "ConversationState.addMessage appends message" {
            let state = ConversationState.initial
            let message = ChatMessage.create User "Hello"
            let newState = ConversationState.addMessage message state

            Expect.hasLength newState.Messages 1 "Should have one message"
            Expect.equal newState.Messages[0].Content "Hello" "Message content should match"
        }

        test "ConversationState.setStep changes workflow step" {
            let state = ConversationState.initial
            let newState = ConversationState.setStep Clarification state

            Expect.equal newState.CurrentStep Clarification "Step should be Clarification"
        }
    ]

[<Tests>]
let allTests =
    testList "Domain" [
        storyTests
        idTests
        conversationTests
    ]
