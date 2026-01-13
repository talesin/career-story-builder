module CareerStoryBuilder.Client.Views.Wizard.InitialCapture

open System
open Bolero.Html
open CareerStoryBuilder.Client

/// Minimum recommended character count for guidance (not blocking).
let recommendedMinLength = 100

/// Count words in text.
let wordCount (text: string) =
    if String.IsNullOrWhiteSpace text then 0
    else text.Split([| ' '; '\n'; '\r'; '\t' |], StringSplitOptions.RemoveEmptyEntries).Length

/// Count characters in text.
let charCount (text: string) =
    text.Length

/// Validate content is not empty/whitespace only.
let isValidContent (content: string) =
    not (String.IsNullOrWhiteSpace content)

/// Initial capture view - textarea for free-form story description.
let view (content: string) (isProcessing: bool) dispatch =
    let chars = charCount content
    let words = wordCount content
    let isValid = isValidContent content
    let hasMinContent = chars >= recommendedMinLength

    div {
        attr.``class`` "initial-capture"

        // Guidance section
        div {
            attr.``class`` "guidance"
            h2 { "Capture Your Story" }
            p {
                "Start by describing your accomplishment in your own words. "
                "Don't worry about structure yet - just tell us what happened, "
                "what you did, and what the outcome was."
            }
            p {
                attr.``class`` "guidance-tips"
                strong { "Tips: " }
                "Include specific details, numbers, and outcomes when possible. "
                "Think about the challenge you faced, the actions you took, and the results you achieved."
            }
        }

        // Textarea for story input
        div {
            attr.``class`` "form-group"
            label {
                attr.``for`` "initial-content"
                "Your Story"
            }
            textarea {
                attr.id "initial-content"
                attr.``class`` "story-textarea"
                attr.placeholder "Describe your accomplishment... For example: 'I led a project to migrate our legacy system to the cloud, which reduced costs by 40% and improved performance...'"
                attr.rows 10
                bind.input.string content (SetInitialCaptureContent >> dispatch)
                attr.disabled isProcessing
            }
        }

        // Character/word count and guidance indicator
        div {
            attr.``class`` "content-stats"
            span {
                attr.``class`` "char-count"
                $"{chars} characters"
            }
            span {
                attr.``class`` "word-count"
                $"{words} words"
            }
            cond hasMinContent <| function
                | true ->
                    span {
                        attr.``class`` "sufficiency-indicator sufficient"
                        "Good length!"
                    }
                | false ->
                    span {
                        attr.``class`` "sufficiency-indicator insufficient"
                        $"Aim for at least {recommendedMinLength} characters"
                    }
        }

        // Submit button
        div {
            attr.``class`` "form-actions"
            button {
                attr.``type`` "button"
                attr.``class`` "btn-primary"
                attr.disabled (not isValid || isProcessing)
                on.click (fun _ -> dispatch SubmitInitialCapture)
                cond isProcessing <| function
                    | true -> text "Processing..."
                    | false -> text "Continue"
            }
        }
    }
