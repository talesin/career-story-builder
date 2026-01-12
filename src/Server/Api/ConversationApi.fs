module CareerStoryBuilder.Server.Api.ConversationApi

open CareerStoryBuilder.Domain
open CareerStoryBuilder.Dto

/// Stub implementation for clarification endpoint.
/// Returns mock clarifying questions (AI integration in Phase 1A.4).
let clarify (request: ClarifyRequest) : ClarifyResponse =
    let userMsg =
        ChatMessage.create User request.UserMessage
        |> Message.fromDomain

    let assistantMsg =
        ChatMessage.create Assistant
            ("Thanks for sharing! To help craft your SAR story:\n\n" +
             "1. What was the specific challenge or problem you faced?\n" +
             "2. What actions did you personally take?\n" +
             "3. What measurable results did you achieve?")
        |> Message.fromDomain

    let updatedConversation = {
        request.Conversation with
            Messages = request.Conversation.Messages @ [ userMsg; assistantMsg ]
            CurrentStep = Clarification
    }

    { AssistantMessage = assistantMsg.Content
      Conversation = updatedConversation }

/// Stub implementation for generation endpoint.
/// Returns mock generated story (AI integration in Phase 1A.6).
let generate (request: GenerateRequest) : GenerateResponse =
    { Story = {
        Title = "Mock Story Title"
        Situation = "Placeholder situation from prototype."
        Action = "Placeholder action from prototype."
        Result = "Placeholder result from prototype."
      }
      Suggestions = Some [ "Add specific metrics"; "Include team size" ] }
