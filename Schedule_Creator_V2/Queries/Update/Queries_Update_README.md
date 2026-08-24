# Update Queries

This folder contains the SQL `UPDATE` statements used by Schedule Creator V2 to modify existing database records.

The current Update query set is focused entirely on the `UWGB.Staff` table. The SQL files are registered through `Queries.resx`, exposed as properties on the generated `Queries` class, and executed by:

```text
Services/
└── Database/
    └── DatabaseUpdate.cs
```

The normal update flow is:

```text
View / Application Logic
        |
        v
DatabaseUpdate
        |
        v
Queries.<UpdateQuery>
        |
        v
SQL file in Queries/Update
        |
        v
Database.ExecuteNonQuery
        |
        v
Existing UWGB row updated
```

The C# service determines the values that should be written. The SQL query determines which columns are changed and which row is targeted.

---

## Folder Overview

| File | Query Resource | Called By | Target Table | Purpose |
| --- | --- | --- | --- | --- |
| `UpdateStaff.sql` | `Queries.UpdateStaff` | `DatabaseUpdate.UpdateStaff(...)` | `UWGB.Staff` | Updates core staff identity/contact fields. |
| `UpdateBelayCert.sql` | `Queries.UpdateBelayCert` | `DatabaseUpdate.UpdateBelayCert(...)` | `UWGB.Staff` | Updates belay-certification status and dates. |

Both current queries identify the staff record using:

```sql
WHERE id = @id
```

Because `UWGB.Staff.id` is the table's primary key, each update is intended to affect at most one staff row.

---

# How Update Queries Are Executed

The SQL files are registered in `Queries.resx`.

That allows the service layer to reference them as:

```csharp
Queries.UpdateStaff
Queries.UpdateBelayCert
```

The service then supplies `SqlParameter` values and executes the query using:

```csharp
ExecuteNonQuery(...)
```

For example:

```csharp
ExecuteNonQuery(
    Queries.UpdateStaff,
    new SqlParameter("@id", id),
    new SqlParameter("@fName", fName),
    new SqlParameter("@mName", mName),
    new SqlParameter("@lName", lName),
    new SqlParameter(
        "@position",
        position.ToString()),
    new SqlParameter(
        "@email",
        studentEmail));
```

This keeps application values separate from SQL syntax and avoids constructing SQL strings directly from user-entered data.

---

# `UpdateStaff.sql`

## Purpose

Updates the main editable identity and contact fields for an existing staff member.

## SQL

```sql
UPDATE
    [UWGB].[Staff]
SET
    fName = @fName,
    mName = @mName,
    lName = @lName,
    position = @position,
    email = @email
WHERE
    id = @id
```

---

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Existing staff ID used to identify the row. |
| `@fName` | Updated first name. |
| `@mName` | Updated middle name. |
| `@lName` | Updated last name. |
| `@position` | Updated staff position stored as text. |
| `@email` | Updated staff email address. |

---

## Called By

```csharp
DatabaseUpdate.UpdateStaff(
    int id,
    string fName,
    string mName,
    string lName,
    Positions position,
    string studentEmail)
```

The service converts the `Positions` enum before sending it to SQL Server:

```csharp
new SqlParameter(
    "@position",
    position.ToString())
```

Therefore, a C# enum such as:

```csharp
Positions.Shift_Lead
```

is persisted using its string representation.

---

## Columns Updated

This query modifies:

```text
fName
mName
lName
position
email
```

It intentionally does **not** modify:

```text
id
belayCert
certifiedOn
expiresOn
```

Certification fields are maintained separately through `UpdateBelayCert.sql`.

That separation produces two logical update paths:

```text
Staff profile information
        |
        v
UpdateStaff.sql


Belay certification
        |
        v
UpdateBelayCert.sql
```

---

## Row Selection

The query uses:

```sql
WHERE id = @id
```

The current Staff schema defines `id` as:

```sql
INT IDENTITY(1,1)
PRIMARY KEY
```

so the query should update either:

```text
one staff row
```

or:

```text
zero rows if the ID does not exist
```

`Database.ExecuteNonQuery()` currently does not return the number of affected rows to `DatabaseUpdate`, so the service does not currently distinguish those two outcomes.

---

## Middle Name Behavior

The Staff schema defines:

```sql
[mName] NVARCHAR(50) NULL
```

so the database permits a SQL `NULL` middle name.

However, the current service method accepts:

```csharp
string mName
```

and passes it directly into:

```csharp
new SqlParameter(
    "@mName",
    mName)
```

There is currently no explicit conversion such as:

```csharp
string.IsNullOrWhiteSpace(mName)
    ? DBNull.Value
    : mName
```

Therefore, blank/empty values and SQL `NULL` are not treated as the same thing by the current update method.

This should be considered if nullable middle-name behavior is standardized later.

---

# `UpdateBelayCert.sql`

## Purpose

Updates a staff member's belay-certification status and associated dates.

## SQL

```sql
UPDATE
    [UWGB].[Staff]
SET
    belayCert = @belayCert,
    certifiedOn = @certifiedOn,
    expiresOn = @expiresOn
WHERE
    id = @id
```

---

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff ID identifying the record to update. |
| `@belayCert` | Certification status. |
| `@certifiedOn` | Date certification was obtained, or SQL `NULL`. |
| `@expiresOn` | Certification expiration date, or SQL `NULL`. |

---

## Called By

```csharp
DatabaseUpdate.UpdateBelayCert(
    int id,
    bool isBelayCertified,
    DateOnly? certifiedOn = null,
    DateOnly? expiresOn = null)
```

The optional date parameters allow the method to represent both:

```text
certified staff
```

and:

```text
staff without certification dates
```

using the same SQL statement.

---

# Belay Certification Status Conversion

The application uses:

```csharp
bool
```

for the certification state.

The current database schema stores `belayCert` as:

```sql
NVARCHAR(50)
```

rather than SQL:

```sql
BIT
```

Therefore, `DatabaseUpdate` converts the Boolean to text:

```csharp
isBelayCertified.ToString()
```

Typical stored values are:

```text
True
False
```

The corresponding read services later reconstruct the Boolean using:

```csharp
bool.Parse(
    (string)reader["belayCert"])
```

The complete round trip is:

```text
C# bool
   |
   | .ToString()
   v
"True" / "False"
   |
   v
NVARCHAR belayCert
   |
   | bool.Parse(...)
   v
C# bool
```

Because the reader uses `bool.Parse`, values stored in this column should remain valid Boolean strings.

---

# Certification Date Conversion

The application models certification dates using nullable `DateOnly` values:

```csharp
DateOnly? certifiedOn
DateOnly? expiresOn
```

SQL Server stores both columns as:

```sql
DATE
```

---

## Non-Null Date

When a C# date exists, the service converts it to a `DateTime`:

```csharp
certifiedOn.Value.ToDateTime(
    TimeOnly.MinValue)
```

`TimeOnly.MinValue` represents:

```text
00:00:00
```

so a value such as:

```text
2026-08-25
```

becomes conceptually:

```text
2026-08-25 00:00:00
```

before being sent to SQL Server.

Because the destination column is SQL `DATE`, only the calendar date is stored.

---

## Null Date

When the nullable `DateOnly` has no value, the service sends:

```csharp
DBNull.Value
```

Example:

```csharp
new SqlParameter(
    "@certifiedOn",
    certifiedOn.HasValue
        ? (object)certifiedOn.Value
            .ToDateTime(TimeOnly.MinValue)
        : DBNull.Value)
```

The same logic is used for `expiresOn`.

The conversion flow is:

```text
DateOnly?
   |
   +--> has value
   |       |
   |       v
   |   DateTime at midnight
   |       |
   |       v
   |   SQL DATE
   |
   +--> null
           |
           v
       DBNull.Value
           |
           v
       SQL NULL
```

---

# Certification State Combinations

The current method does not impose a business rule connecting:

```text
belayCert
certifiedOn
expiresOn
```

That means the service can technically write combinations such as:

```text
belayCert = False
certifiedOn = 2026-01-10
expiresOn = 2027-01-10
```

or:

```text
belayCert = True
certifiedOn = NULL
expiresOn = NULL
```

Whether those combinations are valid is determined by the UI/application logic that calls `UpdateBelayCert`, not by the SQL statement itself.

If stricter certification rules are added later, they should be enforced consistently before this query is executed or through appropriate database constraints.

---

# Why Staff Updates Are Split

The current architecture separates general staff information from certification information.

```text
UWGB.Staff
   |
   +--> identity/contact information
   |       |
   |       v
   |   UpdateStaff.sql
   |
   +--> certification information
           |
           v
       UpdateBelayCert.sql
```

This is useful because certification may be changed independently of:

```text
name
position
email
```

and normal profile edits do not need to rewrite certification dates.

---

# Parameterization

Both current update queries use SQL parameters.

For example:

```sql
SET
    email = @email
WHERE
    id = @id
```

with C# supplying:

```csharp
new SqlParameter(
    "@email",
    studentEmail),

new SqlParameter(
    "@id",
    id)
```

This should remain the standard for future updates.

Avoid constructing SQL by interpolating user-entered values directly into the command text.

Parameterized queries:

- keep values separate from SQL syntax
- reduce SQL injection risk
- avoid manual quoting/escaping
- make null handling clearer
- keep the service/query boundary consistent

---

# Relationship to `DatabaseUpdate`

The SQL query is responsible for the database operation itself.

`DatabaseUpdate` is responsible for adapting application values to the database.

For example:

```text
Positions enum
      |
      | .ToString()
      v
SqlParameter
      |
      v
UpdateStaff.sql
      |
      v
NVARCHAR position
```

and:

```text
DateOnly?
      |
      +--> DateTime
      |
      +--> DBNull.Value
              |
              v
       UpdateBelayCert.sql
              |
              v
           SQL DATE
```

This conversion logic belongs in the service layer rather than in the view.

---

# Current Update Coverage

The current Update folder only contains operations for:

```text
UWGB.Staff
```

There are currently no dedicated Update query resources for:

```text
UWGB.Availability
UWGB.DaysOff
UWGB.JobSettings
UWGB.Schedule
```

That means the current query architecture should not be assumed to support direct updates to every table.

If direct modification of those records is added later, new update resources and matching `DatabaseUpdate` methods should be introduced.

---

# Adding a New Update Query

When adding an update operation, follow the same resource/service pattern.

## 1. Create the SQL File

Example:

```text
Queries/
└── Update/
    └── UpdateScheduleStaff.sql
```

Example SQL:

```sql
UPDATE
    [UWGB].[Schedule]
SET
    staffID = @staffID
WHERE
    scheduleName = @scheduleName
    AND dayOfWeek = @dayOfWeek
    AND startTime = @startTime
```

The exact identifying columns should be chosen based on the actual schema and uniqueness rules.

---

## 2. Register It in `Queries.resx`

Register the `.sql` file so Visual Studio generates:

```csharp
Queries.UpdateScheduleStaff
```

A SQL file placed in this folder is not automatically usable through the generated `Queries` class unless it is registered as a resource.

---

## 3. Add the Service Method

Add the application-facing method to:

```text
Services/
└── Database/
    └── DatabaseUpdate.cs
```

Example:

```csharp
public static void UpdateExample(
    int id,
    string value)
{
    ExecuteNonQuery(
        Queries.UpdateExample,
        new SqlParameter(
            "@id",
            id),
        new SqlParameter(
            "@value",
            value));
}
```

---

## 4. Keep Parameter Names Synchronized

SQL:

```sql
@value
```

must match C#:

```csharp
new SqlParameter(
    "@value",
    value)
```

Changing one without the other causes a runtime SQL error.

---

## 5. Make the `WHERE` Clause Specific

Every update should explicitly identify the intended row or rows.

Before adding an UPDATE, determine whether the operation should affect:

```text
one row
several related rows
all rows matching a category
```

Then make the `WHERE` clause match that scope.

An update without an appropriate `WHERE` clause can unintentionally modify every row in the table.

---

# Update Query Checklist

When adding or modifying an update query, verify:

- [ ] The correct `[UWGB]` table is targeted.
- [ ] Only the intended columns appear in the `SET` clause.
- [ ] The `WHERE` clause identifies the correct row or rows.
- [ ] SQL parameter names match the corresponding `SqlParameter` names.
- [ ] Nullable values are converted to `DBNull.Value` when appropriate.
- [ ] Enum values use the same string representation expected by readers.
- [ ] Date values use the expected SQL/C# conversion.
- [ ] The SQL file is registered in `Queries.resx`.
- [ ] A corresponding `DatabaseUpdate` method exists.
- [ ] Callers understand what happens when the target ID does not exist.
- [ ] Business-rule validation occurs before the SQL operation when needed.

---

# Current Maintenance Notes

The following details reflect the current repository implementation:

- `UpdateStaff.sql` and `UpdateBelayCert.sql` are the only current Update queries.
- Both queries target `UWGB.Staff`.
- Both queries identify staff using the primary-key `id`.
- `UpdateStaff.sql` updates name, position, and email fields but leaves certification data unchanged.
- `UpdateBelayCert.sql` updates certification data but leaves identity/contact fields unchanged.
- `Positions` values are stored using `position.ToString()`.
- `belayCert` is currently stored as `NVARCHAR(50)` using `bool.ToString()`, rather than as SQL `BIT`.
- The read layer later relies on `bool.Parse()` to reconstruct `belayCert`.
- `certifiedOn` and `expiresOn` are nullable SQL `DATE` columns.
- Nullable `DateOnly` values are converted to either a midnight `DateTime` or `DBNull.Value`.
- `Staff.mName` is nullable in the database, but `UpdateStaff` currently accepts a non-nullable `string` and does not explicitly convert blank values to SQL `NULL`.
- `Database.ExecuteNonQuery()` currently returns `void`, so `DatabaseUpdate` does not inspect the affected-row count to determine whether the requested staff ID actually existed.
- There are currently no direct Update query resources for Availability, DaysOff, JobSettings, or Schedule.

Update this README whenever the Staff schema, update parameters, resource registration, business rules, or `DatabaseUpdate` methods change.
