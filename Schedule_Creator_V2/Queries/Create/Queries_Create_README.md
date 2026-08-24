# Create Queries

This folder contains the SQL `INSERT` statements used by Schedule Creator V2 to create new application records.

The scripts in this folder are loaded through `Queries.resx`, which exposes each SQL file as a strongly named property on the generated `Queries` class. The database service layer then supplies parameter values and executes the SQL through `Database.ExecuteNonQuery()`.

The normal flow is:

```text
View / Application Logic
        |
        v
DatabaseCreate
        |
        v
Queries.<CreateQuery>
        |
        v
SQL file in Queries/Create
        |
        v
Database.ExecuteNonQuery
        |
        v
UWGB database table
```

This keeps the SQL statements separate from the C# code that prepares application values.

---

## Folder Overview

| File | Query Resource | Called By | Target Table |
| --- | --- | --- | --- |
| `CreateAvailability.sql` | `Queries.CreateAvailability` | `DatabaseCreate.CreateAvailability` | `UWGB.Availability` |
| `CreateDaysOff.sql` | `Queries.CreateDaysOff` | `DatabaseCreate.CreateDaysOff` | `UWGB.DaysOff` |
| `CreateJobSettings.sql` | `Queries.CreateJobSettings` | `DatabaseCreate.CreateJobSettings` | `UWGB.JobSettings` |
| `CreateSchedule.sql` | `Queries.CreateSchedule` | `DatabaseCreate.CreateSchedule` | `UWGB.Schedule` |
| `CreateStaff.sql` | `Queries.CreateStaff` | `DatabaseCreate.CreateStaff` | `UWGB.Staff` |
| `asdf.sql` | None | None | None |

All active create queries use parameterized SQL rather than concatenating application values directly into the command text.

---

# How These Queries Are Loaded

The SQL files are registered in `Queries.resx`.

For example:

```xml
<data name="CreateSchedule" type="System.Resources.ResXFileRef, System.Windows.Forms">
    <value>
        Queries\Create\CreateSchedule.sql;
        System.String, mscorlib;
        utf-8
    </value>
</data>
```

Visual Studio generates the corresponding property:

```csharp
Queries.CreateSchedule
```

The database service can then execute the SQL without reading the `.sql` file manually:

```csharp
ExecuteNonQuery(
    Queries.CreateSchedule,
    parameters);
```

This structure provides a simple separation between:

```text
C# application logic
        |
        | prepares values
        v
SqlParameter objects

SQL resource
        |
        | defines operation
        v
INSERT statement
```

---

# `CreateAvailability.sql`

## Purpose

Creates one availability range for a staff member.

## SQL

```sql
INSERT
    INTO
        [UWGB].[Availability]
    (id, dayOfTheWeek, startTime, endTime)
VALUES
    (@id, @dayOfTheWeek, @startTime, @endTime)
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff member ID. |
| `@dayOfTheWeek` | Day for which the availability applies. |
| `@startTime` | Beginning of the availability window. |
| `@endTime` | End of the availability window. |

## Called By

```csharp
DatabaseCreate.CreateAvailability(
    Availability availability)
```

The service passes values from the `Availability` record:

```csharp
new SqlParameter(
    "@id",
    availability.id),

new SqlParameter(
    "@dayOfTheWeek",
    availability.dayOfTheWeek),

new SqlParameter(
    "@startTime",
    availability.startTime),

new SqlParameter(
    "@endTime",
    availability.endTime)
```

## Target Table

```text
UWGB.Availability
```

The current schema stores:

```text
id
dayOfTheWeek
startTime
endTime
```

One staff member can therefore have multiple rows representing availability on different days.

---

# `CreateDaysOff.sql`

## Purpose

Creates one requested or unavailable date for a staff member.

## SQL

```sql
INSERT INTO
    [UWGB].[DaysOff]
    (id, Date, reason)
VALUES
    (@id, @date, @reason)
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff member ID. |
| `@date` | Date the staff member is unavailable. |
| `@reason` | Reason associated with the day off. |

## Called By

```csharp
DatabaseCreate.CreateDaysOff(
    int id,
    List<DateOnly> dates,
    string reason)
```

Unlike the other create methods, `CreateDaysOff` can receive several dates at once.

The C# service loops over the list:

```text
dates
  |
  +--> date 1 -> CreateDaysOff.sql
  |
  +--> date 2 -> CreateDaysOff.sql
  |
  +--> date 3 -> CreateDaysOff.sql
```

The SQL script itself still inserts only **one row per execution**.

Example:

```csharp
foreach (DateOnly date in dates)
{
    ExecuteNonQuery(
        Queries.CreateDaysOff,
        new SqlParameter("@id", id),
        new SqlParameter("@date", date),
        new SqlParameter("@reason", reason));
}
```

## Target Table

```text
UWGB.DaysOff
```

---

# `CreateJobSettings.sql`

## Purpose

Creates the operating-hours configuration for one day of the week.

## SQL

```sql
INSERT INTO
    [UWGB].[JobSettings]
    (DayOfWeek, OpeningTime, ClosingTime)
VALUES
    (@day, @openingTime, @closingTime)
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@day` | Day of the week. |
| `@openingTime` | Time operations begin. |
| `@closingTime` | Time operations end. |

## Called By

```csharp
DatabaseCreate.CreateJobSettings(
    JobSettings settings)
```

The C# service converts the `DayOfWeek` enum to text:

```csharp
new SqlParameter(
    "@day",
    settings.dayOfWeek.ToString())
```

Opening and closing times are then supplied directly from the `JobSettings` record.

## Target Table

```text
UWGB.JobSettings
```

`DayOfWeek` is currently the primary key for this table, meaning only one job-settings row can exist for a given day.

Attempting to insert another row for a day that already exists will violate that primary-key constraint unless the existing row is first deleted or updated.

---

# `CreateSchedule.sql`

## Purpose

Creates one shift/assignment row in a saved schedule.

## SQL

```sql
INSERT INTO
    [UWGB].[Schedule]
    ([dayOfWeek], staffID, startTime, endTime, scheduleName)
VALUES
    (@dayOfWeek, @staffID, @startTime, @endTime, @scheduleName)
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@dayOfWeek` | Day on which the shift occurs. |
| `@staffID` | Assigned staff member, or SQL `NULL` when unassigned. |
| `@startTime` | Shift start time. |
| `@endTime` | Shift end time. |
| `@scheduleName` | Name used to group rows into one saved schedule. |

## Called By

```csharp
DatabaseCreate.CreateSchedule(
    ScheduleRow row)
```

The service performs two important conversions before executing the query.

### Nullable Staff ID

A schedule row may exist without an assigned staff member.

The C# layer converts a missing `staffID` to:

```csharp
DBNull.Value
```

before sending it to SQL Server.

### `TimeOnly` to SQL `TIME`

The application uses `TimeOnly`, while `SqlParameter` sends the values as `TimeSpan`:

```csharp
row.startTime.ToTimeSpan()
row.endTime.ToTimeSpan()
```

## Schedule Grouping

There is no separate Schedule header table in the current schema.

Instead, multiple rows belong to the same schedule because they share the same `scheduleName`.

For example:

```text
Fall 26 - Final Draft
    |
    +--> Monday / Riley / 2:45 - 8:15
    +--> Monday / Abbie / 2:45 - 6:15
    +--> Tuesday / Riley / 2:45 - 8:15
    +--> ...
```

Each line is stored as an independent row in `UWGB.Schedule`.

## Target Table

```text
UWGB.Schedule
```

---

# `CreateStaff.sql`

## Purpose

Creates a staff-member record.

## SQL

```sql
INSERT INTO
    [UWGB].[Staff]
    (fName, mName, lName, position, email, belayCert)
VALUES
    (@fName, @mName, @lName, @position, @email, @belayCert)
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@fName` | First name. |
| `@mName` | Middle name. |
| `@lName` | Last name. |
| `@position` | Staff position/role. |
| `@email` | Staff email address. |
| `@belayCert` | Belay-certification status. |

## Called By

```csharp
DatabaseCreate.CreateStaff(
    string fName,
    string mName,
    string lName,
    Positions position,
    byte[]? profilePicture,
    string email,
    bool isBelayCertified)
```

The service converts:

```csharp
position.ToString()
```

before storing the position.

It currently also converts the certification Boolean to text:

```csharp
isBelayCertified.ToString()
```

because the database currently stores `belayCert` as an `NVARCHAR` column rather than a SQL `BIT`.

## Identity Column

The query does **not** provide an `id`.

That is intentional because the schema defines:

```sql
[id] INT IDENTITY (1, 1)
```

SQL Server therefore generates the staff ID automatically.

## Current Profile-Picture Behavior

`DatabaseCreate.CreateStaff` currently accepts:

```csharp
byte[]? profilePicture
```

but `CreateStaff.sql` has no profile-picture column or parameter.

That means the supplied profile picture is currently **not persisted** by this insert operation.

## Target Table

```text
UWGB.Staff
```

---

# `asdf.sql`

This file is currently located in the Create query folder, but it is **not a SQL query**.

Its contents are C# boilerplate:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Queries.Create
{
    class asdf
    {
    }
}
```

It is also **not registered in `Queries.resx`**, so there is no:

```csharp
Queries.asdf
```

resource used by the application.

As a result, this file currently has no role in the database query system.

It appears to be an accidental or leftover development file and can likely be removed after confirming nothing external relies on it.

---

# Parameterization

Every active query in this folder uses SQL parameters:

```sql
VALUES
    (@id, @date, @reason)
```

rather than inserting C# values directly into the SQL string.

The corresponding service provides:

```csharp
new SqlParameter("@id", id)
new SqlParameter("@date", date)
new SqlParameter("@reason", reason)
```

This should remain the standard when new create queries are added.

Parameterized queries provide several advantages:

- application values are kept separate from SQL syntax
- SQL Server performs appropriate parameter handling
- strings containing apostrophes or special characters do not require manual escaping
- risk of SQL injection is greatly reduced
- C# methods remain easier to read and maintain

Avoid patterns such as:

```csharp
$"INSERT INTO Staff VALUES ('{name}')"
```

when adding new database operations.

---

# Relationship to `DatabaseCreate`

The SQL files in this folder deliberately contain very little application logic.

For example, `CreateSchedule.sql` only knows how to perform:

```text
parameters
    |
    v
INSERT row
```

It does **not** decide:

- which employee should be scheduled
- whether the employee is available
- whether the shift overlaps another shift
- whether a schedule name already exists
- whether the schedule is valid
- how `TimeOnly` should be converted
- how a nullable staff ID should be handled

Those responsibilities belong to the C# application/service layer.

This separation can be summarized as:

```text
Application Logic
    decides WHAT should happen
             |
             v
DatabaseCreate
    converts C# values to SQL parameters
             |
             v
Create Query
    defines HOW the row is inserted
             |
             v
Database
```

---

# Adding a New Create Query

When a new entity or INSERT operation is added, follow the existing pattern.

## 1. Add the SQL file

Example:

```text
Queries/
└── Create/
    └── CreateExample.sql
```

Example SQL:

```sql
INSERT INTO
    [UWGB].[Example]
    (id, value)
VALUES
    (@id, @value)
```

## 2. Register it in `Queries.resx`

The resource should point to the `.sql` file so Visual Studio generates a property such as:

```csharp
Queries.CreateExample
```

## 3. Add the C# service method

Place the application-facing method in:

```text
Services/
└── Database/
    └── DatabaseCreate.cs
```

Example:

```csharp
public static void CreateExample(
    int id,
    string value)
{
    ExecuteNonQuery(
        Queries.CreateExample,
        new SqlParameter("@id", id),
        new SqlParameter("@value", value));
}
```

## 4. Keep Parameter Names Synchronized

The SQL parameter:

```sql
@value
```

must match the C# parameter:

```csharp
new SqlParameter("@value", value)
```

Changing one without changing the other will cause the command to fail at runtime.

---

# Current Create-Query Conventions

When modifying or extending this folder, preserve these conventions unless the database architecture itself is being changed:

- Use `[UWGB]` as the schema for application tables.
- Use parameterized SQL for all application values.
- Keep one logical create operation per `.sql` file.
- Name files using `Create<Entity>.sql`.
- Register active SQL files in `Queries.resx`.
- Put value conversion and application logic in `DatabaseCreate`, not in the SQL file.
- Allow SQL Server to generate identity values when the table uses an `IDENTITY` column.
- Use `DBNull.Value` from C# when a nullable database value must be inserted.

---

# Current Maintenance Notes

The following details describe the current implementation:

- `CreateAvailability.sql`, `CreateDaysOff.sql`, `CreateJobSettings.sql`, `CreateSchedule.sql`, and `CreateStaff.sql` are active resources in `Queries.resx`.
- `asdf.sql` is not SQL and is not registered as a query resource.
- `CreateStaff.sql` does not currently persist the `profilePicture` argument accepted by `DatabaseCreate.CreateStaff`.
- `belayCert` is currently stored as text rather than a SQL `BIT`.
- `CreateSchedule.sql` allows `staffID` to be `NULL`.
- Saved schedules are grouped by repeated `scheduleName` values rather than by a separate schedule-header ID.
- `UWGB.JobSettings.DayOfWeek` is a primary key, so only one row can exist for each configured day.

Update this README whenever create-query parameters, table schemas, or their corresponding `DatabaseCreate` methods change.
