module CareerStoryBuilder.Json

open System.Text.Json
open System.Text.Json.Serialization

/// Shared JSON serialization options configured for F# types.
/// Uses FSharp.SystemTextJson for discriminated union support.
let options =
    let opts =
        JsonFSharpOptions.Default()
            .WithUnionUnwrapFieldlessTags()
            .WithUnionTagNamingPolicy(JsonNamingPolicy.CamelCase)
            .WithSkippableOptionFields()
            .ToJsonSerializerOptions()
    opts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    opts.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
    opts

/// Configure existing JsonSerializerOptions with F# type support.
let configureOptions (opts: JsonSerializerOptions) =
    JsonFSharpOptions.Default()
        .WithUnionUnwrapFieldlessTags()
        .WithUnionTagNamingPolicy(JsonNamingPolicy.CamelCase)
        .WithSkippableOptionFields()
        .AddToJsonSerializerOptions(opts)
    opts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    opts.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
