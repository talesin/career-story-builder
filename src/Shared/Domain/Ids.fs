namespace CareerStoryBuilder.Domain

open System

/// Unique identifier for a Story entity.
/// Uses UUIDv7 for database-friendly sequential ordering.
type StoryId = StoryId of Guid with
    member this.Value = match this with StoryId id -> id

/// Unique identifier for a User entity.
type UserId = UserId of Guid with
    member this.Value = match this with UserId id -> id

/// Unique identifier for a Role entity.
type RoleId = RoleId of Guid with
    member this.Value = match this with RoleId id -> id

module StoryId =
    let create () = StoryId(Guid.CreateVersion7())
    let createWithTimestamp ts = StoryId(Guid.CreateVersion7(ts))

module UserId =
    let create () = UserId(Guid.CreateVersion7())
    let createWithTimestamp ts = UserId(Guid.CreateVersion7(ts))

module RoleId =
    let create () = RoleId(Guid.CreateVersion7())
    let createWithTimestamp ts = RoleId(Guid.CreateVersion7(ts))
