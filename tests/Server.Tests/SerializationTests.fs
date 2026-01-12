module CareerStoryBuilder.Tests.SerializationTests

open System
open System.Text.Json
open Expecto
open CareerStoryBuilder.Domain
open CareerStoryBuilder.Json
open CareerStoryBuilder.Dto

let private roundTrip<'T> (value: 'T) : 'T =
    let json = JsonSerializer.Serialize(value, options)
    match JsonSerializer.Deserialize<'T>(json, options) with
    | null -> failwith "Deserialization returned null"
    | result -> result

let messageTests =
    testList "Message" [
        test "serializes and deserializes correctly" {
            let original: Message = {
                Role = User
                Content = "Test message"
                Timestamp = DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero)
                Error = None
            }

            let result = roundTrip original

            Expect.equal result.Role original.Role "Role should match"
            Expect.equal result.Content original.Content "Content should match"
            Expect.equal result.Timestamp original.Timestamp "Timestamp should match"
            Expect.equal result.Error original.Error "Error should match"
        }

        test "serializes Role as camelCase string" {
            let msg: Message = {
                Role = User
                Content = "Test"
                Timestamp = DateTimeOffset.UtcNow
                Error = None
            }

            let json = JsonSerializer.Serialize(msg, options)

            Expect.stringContains json "\"role\":\"user\"" "User role should serialize as camelCase"
        }

        test "serializes Assistant role as camelCase string" {
            let msg: Message = {
                Role = Assistant
                Content = "Response"
                Timestamp = DateTimeOffset.UtcNow
                Error = None
            }

            let json = JsonSerializer.Serialize(msg, options)

            Expect.stringContains json "\"role\":\"assistant\"" "Assistant role should serialize as camelCase"
        }

        test "serializes with error" {
            let original: Message = {
                Role = Assistant
                Content = "Error occurred"
                Timestamp = DateTimeOffset.UtcNow
                Error = Some "AI service unavailable"
            }

            let result = roundTrip original

            Expect.equal result.Error (Some "AI service unavailable") "Error should match"
        }
    ]

let conversationSnapshotTests =
    testList "ConversationSnapshot" [
        test "serializes empty conversation" {
            let original: ConversationSnapshot = {
                Messages = []
                CurrentStep = InitialCapture
                DraftTitle = None
            }

            let result = roundTrip original

            Expect.isEmpty result.Messages "Messages should be empty"
            Expect.equal result.CurrentStep InitialCapture "CurrentStep should match"
            Expect.isNone result.DraftTitle "DraftTitle should be None"
        }

        test "serializes WorkflowStep as camelCase string" {
            let snapshot: ConversationSnapshot = {
                Messages = []
                CurrentStep = Clarification
                DraftTitle = None
            }

            let json = JsonSerializer.Serialize(snapshot, options)

            Expect.stringContains json "\"currentStep\":\"clarification\"" "Clarification should serialize as camelCase"
        }

        test "serializes all WorkflowStep values correctly" {
            let steps = [ InitialCapture; Clarification; Refinement; Generation ]
            let expected = [ "initialCapture"; "clarification"; "refinement"; "generation" ]

            for (step, expectedStr) in List.zip steps expected do
                let snapshot: ConversationSnapshot = {
                    Messages = []
                    CurrentStep = step
                    DraftTitle = None
                }
                let json = JsonSerializer.Serialize(snapshot, options)
                Expect.stringContains json $"\"currentStep\":\"{expectedStr}\"" $"{step} should serialize as {expectedStr}"
        }

        test "serializes conversation with messages" {
            let original: ConversationSnapshot = {
                Messages = [
                    { Role = User; Content = "Hello"; Timestamp = DateTimeOffset.UtcNow; Error = None }
                    { Role = Assistant; Content = "Hi there"; Timestamp = DateTimeOffset.UtcNow; Error = None }
                ]
                CurrentStep = Clarification
                DraftTitle = Some "My Story"
            }

            let result = roundTrip original

            Expect.hasLength result.Messages 2 "Should have 2 messages"
            Expect.equal result.CurrentStep Clarification "CurrentStep should match"
            Expect.equal result.DraftTitle (Some "My Story") "DraftTitle should match"
        }
    ]

let storyOutputTests =
    testList "StoryOutput" [
        test "serializes and deserializes correctly" {
            let original: StoryOutput = {
                Title = "Led Migration Project"
                Situation = "Legacy system needed modernization"
                Action = "Designed migration strategy"
                Result = "40% performance improvement"
            }

            let result = roundTrip original

            Expect.equal result.Title original.Title "Title should match"
            Expect.equal result.Situation original.Situation "Situation should match"
            Expect.equal result.Action original.Action "Action should match"
            Expect.equal result.Result original.Result "Result should match"
        }
    ]

let clarifyRequestResponseTests =
    testList "ClarifyRequest/Response" [
        test "ClarifyRequest round-trip" {
            let original: ClarifyRequest = {
                Conversation = {
                    Messages = []
                    CurrentStep = InitialCapture
                    DraftTitle = None
                }
                UserMessage = "I led a database migration"
            }

            let result = roundTrip original

            Expect.equal result.UserMessage original.UserMessage "UserMessage should match"
            Expect.equal result.Conversation.CurrentStep InitialCapture "CurrentStep should match"
        }

        test "ClarifyResponse round-trip" {
            let original: ClarifyResponse = {
                AssistantMessage = "What was the outcome?"
                Conversation = {
                    Messages = [
                        { Role = User; Content = "Test"; Timestamp = DateTimeOffset.UtcNow; Error = None }
                    ]
                    CurrentStep = Clarification
                    DraftTitle = None
                }
            }

            let result = roundTrip original

            Expect.equal result.AssistantMessage original.AssistantMessage "AssistantMessage should match"
            Expect.hasLength result.Conversation.Messages 1 "Should have 1 message"
        }
    ]

let generateRequestResponseTests =
    testList "GenerateRequest/Response" [
        test "GenerateRequest round-trip" {
            let original: GenerateRequest = {
                Conversation = {
                    Messages = [
                        { Role = User; Content = "Initial story"; Timestamp = DateTimeOffset.UtcNow; Error = None }
                        { Role = Assistant; Content = "Questions"; Timestamp = DateTimeOffset.UtcNow; Error = None }
                    ]
                    CurrentStep = Generation
                    DraftTitle = Some "My Story"
                }
            }

            let result = roundTrip original

            Expect.hasLength result.Conversation.Messages 2 "Should have 2 messages"
            Expect.equal result.Conversation.CurrentStep Generation "CurrentStep should match"
        }

        test "GenerateResponse round-trip" {
            let original: GenerateResponse = {
                Story = {
                    Title = "Test Story"
                    Situation = "Test situation"
                    Action = "Test action"
                    Result = "Test result"
                }
                Suggestions = Some [ "Add metrics"; "Include team size" ]
            }

            let result = roundTrip original

            Expect.equal result.Story.Title "Test Story" "Story title should match"
            Expect.equal result.Suggestions (Some [ "Add metrics"; "Include team size" ]) "Suggestions should match"
        }

        test "GenerateResponse without suggestions" {
            let original: GenerateResponse = {
                Story = {
                    Title = "Test Story"
                    Situation = "Situation"
                    Action = "Action"
                    Result = "Result"
                }
                Suggestions = None
            }

            let result = roundTrip original

            Expect.isNone result.Suggestions "Suggestions should be None"
        }
    ]

[<Tests>]
let allTests =
    testList "Serialization" [
        messageTests
        conversationSnapshotTests
        storyOutputTests
        clarifyRequestResponseTests
        generateRequestResponseTests
    ]
