namespace CareerStoryBuilder.Domain

open System

/// Unique identifier for a Story entity.
/// Uses UUIDv7 for database-friendly sequential ordering.
type StoryId = StoryId of Guid

/// Unique identifier for a User entity.
type UserId = UserId of Guid

/// Unique identifier for a Role entity.
type RoleId = RoleId of Guid

module StoryId =
    let create () = StoryId(Guid.NewGuid())
    let value (StoryId id) = id

module UserId =
    let create () = UserId(Guid.NewGuid())
    let value (UserId id) = id

module RoleId =
    let create () = RoleId(Guid.NewGuid())
    let value (RoleId id) = id
