# Data Access Guide (Dapper + Dapper.FSharp)

## Quick Links by Task

### Dapper.FSharp (Primary)
| Task                    | Topic                            |
| ----------------------- | -------------------------------- |
| Setup and configuration | dapper-fsharp#setup              |
| Table mapping           | dapper-fsharp#table-mapping      |
| SELECT queries          | dapper-fsharp#select-queries     |
| INSERT operations       | dapper-fsharp#insert-operations  |
| UPDATE operations       | dapper-fsharp#update-operations  |
| DELETE operations       | dapper-fsharp#delete-operations  |
| JOINs                   | dapper-fsharp#joins              |
| Aggregations            | dapper-fsharp#aggregations       |
| Transactions            | dapper-fsharp#patterns           |

### Plain Dapper (Fallback)
| Task                  | Topic                    |
| --------------------- | ------------------------ |
| Core operations       | dapper#core-operations   |
| Parameters            | dapper#parameters        |
| Multi-mapping (joins) | dapper#multi-mapping     |
| Async operations      | dapper#async-operations  |
| Type handlers         | dapper#type-handlers     |

## Key Patterns for Career Story Builder

Use **Dapper.FSharp** for:
- Type-safe CRUD operations on story entities
- Simple joins between stories and related tables
- Aggregations (story counts by tag, etc.)

Use **plain Dapper** for:
- Complex queries that Dapper.FSharp can't express
- Raw SQL when performance is critical
- Custom type handling

## Primary References

### Setup
- **Option Types**: `dapper-fsharp#setup`
- **Table Definition**: `dapper-fsharp#table-mapping`

### Query Building
- **Computation Expressions**: `dapper-fsharp#select-queries`
- **WHERE conditions**: `dapper-fsharp#where-conditions`

## Domain Examples

### Story Table Mapping

```fsharp
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL  // or MSSQL, MySQL, SQLite

// IMPORTANT: Register F# Option types on startup
OptionTypes.register()

// Database record types (match database schema)
type StoryRecord = {
    Id: Guid
    UserId: Guid
    Title: string
    SituationContext: string
    SituationWhen: DateOnly option
    SituationWhere: string option
    TaskChallenge: string
    TaskResponsibility: string
    TaskStakeholders: string option
    ResultOutcome: string
    ResultImpact: string option
    ResultMetrics: string option
    CreatedAt: DateTimeOffset
    UpdatedAt: DateTimeOffset
}

type ActionRecord = {
    Id: Guid
    StoryId: Guid
    Step: int
    Description: string
    Skills: string option
}

type TagRecord = {
    Id: Guid
    Name: string
}

type StoryTagRecord = {
    StoryId: Guid
    TagId: Guid
}

// Table definitions
let storiesTable = table<StoryRecord>
let actionsTable = table<ActionRecord>
let tagsTable = table<TagRecord>
let storyTagsTable = table<StoryTagRecord>
```

### Story Repository (Dapper.FSharp)

```fsharp
open Dapper.FSharp.PostgreSQL

type StoryRepository(conn: IDbConnection) =

    member _.GetAll(userId: Guid) = task {
        let! stories =
            select {
                for s in storiesTable do
                where (s.UserId = userId)
                orderByDescending s.UpdatedAt
            } |> conn.SelectAsync<StoryRecord>

        return stories |> Seq.toList
    }

    member _.GetById(id: Guid) = task {
        let! stories =
            select {
                for s in storiesTable do
                where (s.Id = id)
            } |> conn.SelectAsync<StoryRecord>

        return stories |> Seq.tryHead
    }

    member _.Create(story: StoryRecord) = task {
        do! insert {
            into storiesTable
            value story
        } |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore

        return story
    }

    member _.Update(story: StoryRecord) = task {
        do! update {
            for s in storiesTable do
            set story
            where (s.Id = story.Id)
        } |> conn.UpdateAsync |> Async.AwaitTask |> Async.Ignore

        return story
    }

    member _.Delete(id: Guid) = task {
        do! delete {
            for s in storiesTable do
            where (s.Id = id)
        } |> conn.DeleteAsync |> Async.AwaitTask |> Async.Ignore
    }
```

### Actions CRUD

```fsharp
member _.GetActionsByStoryId(storyId: Guid) = task {
    let! actions =
        select {
            for a in actionsTable do
            where (a.StoryId = storyId)
            orderBy a.Step
        } |> conn.SelectAsync<ActionRecord>

    return actions |> Seq.toList
}

member _.SaveActions(storyId: Guid, actions: ActionRecord list) = task {
    // Delete existing actions
    do! delete {
        for a in actionsTable do
        where (a.StoryId = storyId)
    } |> conn.DeleteAsync |> Async.AwaitTask |> Async.Ignore

    // Insert new actions
    if not (List.isEmpty actions) then
        do! insert {
            into actionsTable
            values actions
        } |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore
}
```

### Joins (Story with Actions)

```fsharp
member _.GetStoryWithActions(id: Guid) = task {
    // Dapper.FSharp join
    let! results =
        select {
            for s in storiesTable do
            leftJoin a in actionsTable on (s.Id = a.StoryId)
            where (s.Id = id)
            orderBy a.Step
        } |> conn.SelectAsync<StoryRecord, ActionRecord option>

    // Group results
    let storyOpt = results |> Seq.tryHead |> Option.map fst
    let actions =
        results
        |> Seq.choose (fun (_, a) -> a)
        |> Seq.toList

    return storyOpt |> Option.map (fun s -> s, actions)
}
```

### Aggregations

```fsharp
member _.GetStoryCountByUser(userId: Guid) = task {
    let! result =
        select {
            for s in storiesTable do
            where (s.UserId = userId)
            count "*" "StoryCount"
        } |> conn.SelectAsync<{| StoryCount: int |}>

    return result |> Seq.head |> _.StoryCount
}

member _.GetStoriesPerMonth(userId: Guid) = task {
    let! results =
        select {
            for s in storiesTable do
            where (s.UserId = userId)
            groupBy (s.CreatedAt.Month)
            orderBy (s.CreatedAt.Month)
            count "*" "Count"
        } |> conn.SelectAsync<{| Month: int; Count: int |}>

    return results |> Seq.toList
}
```

### Complex Query (Plain Dapper Fallback)

```fsharp
// For queries too complex for Dapper.FSharp
member _.SearchStories(userId: Guid, searchTerm: string, tags: string list) = task {
    let sql = """
        SELECT DISTINCT s.*
        FROM stories s
        LEFT JOIN story_tags st ON s.id = st.story_id
        LEFT JOIN tags t ON st.tag_id = t.id
        WHERE s.user_id = @UserId
          AND (
            s.title ILIKE @SearchTerm
            OR s.situation_context ILIKE @SearchTerm
            OR s.task_challenge ILIKE @SearchTerm
            OR s.result_outcome ILIKE @SearchTerm
          )
          AND (@Tags IS NULL OR t.name = ANY(@Tags))
        ORDER BY s.updated_at DESC
    """

    let! results = conn.QueryAsync<StoryRecord>(
        sql,
        {| UserId = userId
           SearchTerm = $"%%{searchTerm}%%"
           Tags = if List.isEmpty tags then null else tags |> List.toArray |})

    return results |> Seq.toList
}
```

### Transactions

```fsharp
member _.CreateStoryWithActions(story: StoryRecord, actions: ActionRecord list) = task {
    use transaction = conn.BeginTransaction()

    try
        // Insert story
        do! insert {
            into storiesTable
            value story
        } |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore

        // Insert actions
        if not (List.isEmpty actions) then
            do! insert {
                into actionsTable
                values actions
            } |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore

        transaction.Commit()
        return Ok story
    with ex ->
        transaction.Rollback()
        return Error ex.Message
}
```

### Type Handler for StoryId

```fsharp
// Register custom type handlers for domain types
open Dapper

type StoryIdHandler() =
    inherit SqlMapper.TypeHandler<StoryId>()

    override _.SetValue(parameter, value) =
        let (StoryId id) = value
        parameter.Value <- id

    override _.Parse(value) =
        StoryId(value :?> Guid)

// Register on startup
SqlMapper.AddTypeHandler(StoryIdHandler())
```

## When to Use Which

| Scenario                      | Use           |
| ----------------------------- | ------------- |
| Simple CRUD                   | Dapper.FSharp |
| Type-safe queries             | Dapper.FSharp |
| Simple joins (up to 5 tables) | Dapper.FSharp |
| Full-text search              | Plain Dapper  |
| Window functions              | Plain Dapper  |
| CTEs                          | Plain Dapper  |
| Database-specific features    | Plain Dapper  |
