# Data Access Guide (Dapper + Dapper.FSharp)

## Quick Links by Task

### Dapper.FSharp (Primary)

| Task                    | Topic                           |
| ----------------------- | ------------------------------- |
| Setup and configuration | dapper-fsharp#setup             |
| Table mapping           | dapper-fsharp#table-mapping     |
| SELECT queries          | dapper-fsharp#select-queries    |
| INSERT operations       | dapper-fsharp#insert-operations |
| UPDATE operations       | dapper-fsharp#update-operations |
| DELETE operations       | dapper-fsharp#delete-operations |
| JOINs                   | dapper-fsharp#joins             |
| Aggregations            | dapper-fsharp#aggregations      |
| Transactions            | dapper-fsharp#patterns          |

### Plain Dapper (Fallback)

| Task                  | Topic                   |
| --------------------- | ----------------------- |
| Core operations       | dapper#core-operations  |
| Parameters            | dapper#parameters       |
| Multi-mapping (joins) | dapper#multi-mapping    |
| Async operations      | dapper#async-operations |
| Type handlers         | dapper#type-handlers    |

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
type UserRecord = {
    Id: Guid
    Name: string
    Email: string
    CreatedAt: DateTimeOffset
}

type PostRecord = {
    Id: Guid
    UserId: Guid
    Title: string
    Content: string
    CreatedAt: DateTimeOffset
}

// Table definitions
let usersTable = table<UserRecord>
let postsTable = table<PostRecord>
```

### Basic Repository (Dapper.FSharp)

```fsharp
open Dapper.FSharp.PostgreSQL

type PostRepository(conn: IDbConnection) =

    member _.GetByUser(userId: Guid) = task {
        let! posts =
            select {
                for p in postsTable do
                where (p.UserId = userId)
                orderByDescending p.CreatedAt
            } |> conn.SelectAsync<PostRecord>
        return posts |> Seq.toList
    }

    member _.GetById(id: Guid) = task {
        let! posts =
            select {
                for p in postsTable do
                where (p.Id = id)
            } |> conn.SelectAsync<PostRecord>
        return posts |> Seq.tryHead
    }

    member _.Create(post: PostRecord) = task {
        do! insert {
            into postsTable
            value post
        } |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore
        return post
    }

    member _.Delete(id: Guid) = task {
        do! delete {
            for p in postsTable do
            where (p.Id = id)
        } |> conn.DeleteAsync |> Async.AwaitTask |> Async.Ignore
    }
```

### Joins

```fsharp
member _.GetPostWithUser(postId: Guid) = task {
    let! results =
        select {
            for p in postsTable do
            leftJoin u in usersTable on (p.UserId = u.Id)
            where (p.Id = postId)
        } |> conn.SelectAsync<PostRecord, UserRecord option>

    return results |> Seq.tryHead
}
```

### Aggregations

```fsharp
member _.GetPostCount(userId: Guid) = task {
    let! result =
        select {
            for p in postsTable do
            where (p.UserId = userId)
            count "*" "Total"
        } |> conn.SelectAsync<{| Total: int |}>

    return result |> Seq.head |> _.Total
}
```

### Complex Query (Plain Dapper Fallback)

```fsharp
// For queries too complex for Dapper.FSharp
member _.Search(userId: Guid, searchTerm: string) = task {
    let sql = """
        SELECT * FROM posts
        WHERE user_id = @UserId
          AND (title ILIKE @Term OR content ILIKE @Term)
        ORDER BY created_at DESC
    """
    let! results = conn.QueryAsync<PostRecord>(
        sql, {| UserId = userId; Term = $"%%{searchTerm}%%" |})
    return results |> Seq.toList
}
```

### Transactions

```fsharp
member _.CreateWithUser(user: UserRecord, post: PostRecord) = task {
    use transaction = conn.BeginTransaction()
    try
        do! insert { into usersTable; value user }
            |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore
        do! insert { into postsTable; value post }
            |> conn.InsertAsync |> Async.AwaitTask |> Async.Ignore
        transaction.Commit()
        return Ok post
    with ex ->
        transaction.Rollback()
        return Error ex.Message
}
```

### Custom Type Handler

```fsharp
// Register type handlers for domain types (e.g., single-case DUs)
type UserIdHandler() =
    inherit SqlMapper.TypeHandler<UserId>()
    override _.SetValue(p, v) = let (UserId id) = v in p.Value <- id
    override _.Parse(v) = UserId(v :?> Guid)

SqlMapper.AddTypeHandler(UserIdHandler())
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
