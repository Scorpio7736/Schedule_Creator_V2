# Delete Queries

This folder contains the SQL `DELETE` statements used by Schedule Creator V2 to remove application data.

Most active scripts in this folder are loaded through `Queries.resx`, exposed as properties on the generated `Queries` class, and executed by methods in `Services/Database/DatabaseDelete.cs`.

The normal flow is:

```text
View / Application Logic
        |
        v
DatabaseDelete
        |
        v
Queries.<DeleteQuery>
        |
        v
SQL file in Queries/Delete
        |
        v
Database.ExecuteNonQuery
        |
        v
Rows removed from UWGB database
```

Because `DELETE` operations permanently remove rows, the scope of each query is important. A missing or incorrect `WHERE` clause can affect more data than intended.

---

## Folder Overview

| File | Query Resource | Called By | Target Table | Scope |
| --- | --- | --- | --- | --- |
| `DeleteAllAvailability.sql` | `Queries.DeleteAllAvailability` | `DatabaseDelete.DeleteAllAvailability` | `UWGB.Availability` | All availability for one staff ID |
| `DeleteDaysOff.sql` | `Queries.DeleteDaysOff` | `DatabaseDelete.DeleteDaysOff` | `UWGB.DaysOff` | One staff/date combination |
| `DeleteJobSettingsOnDay.sql` | `Queries.DeleteJobSettingsOnDay` | `DatabaseDelete.DeleteJobSettingsOnDay` | `UWGB.JobSettings` | One day |
| `DeleteAllJobSettings.sql` | `Queries.DeleteAllJobSettings` | `DatabaseDelete.DeleteAllJobSettings` | `UWGB.JobSettings` | Entire table |
| `DeleteAllByID.sql` | **Not currently registered** | **Not currently used** | Multiple tables | Intended staff cleanup |

> **Important:** `DeleteAllByID.sql` exists in this folder, but the current `DatabaseDelete.DeleteAllByID` method does not use it. That service method currently contains separate inline SQL.

---

# How Delete Queries Are Loaded

Active delete scripts are registered in `Queries.resx`.

For example:

```xml
<data
    name="DeleteDaysOff"
    type="System.Resources.ResXFileRef, System.Windows.Forms">

    <value>
        Queries\Delete\DeleteDaysOff.sql;
        System.String, mscorlib;
        utf-8
    </value>
</data>
```

Visual Studio then exposes the script as:

```csharp
Queries.DeleteDaysOff
```

The database service executes it with parameters:

```csharp
ExecuteNonQuery(
    Queries.DeleteDaysOff,
    new SqlParameter("@id", id),
    new SqlParameter("@date", date));
```

This keeps the destructive SQL itself separate from the application logic that decides when a delete should happen.

---

# `DeleteAllAvailability.sql`

## Purpose

Deletes every availability record belonging to one staff member.

## SQL

```sql
DELETE
    FROM
        [UWGB].[Availability]
WHERE
    id = @id
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff ID whose availability records should be removed. |

## Called By

```csharp
DatabaseDelete.DeleteAllAvailability(
    int id)
```

The service executes:

```csharp
ExecuteNonQuery(
    Queries.DeleteAllAvailability,
    new SqlParameter("@id", id));
```

## Target Table

```text
UWGB.Availability
```

## Behavior

This query can remove multiple rows.

For example:

```text
UWGB.Availability

id    day       start     end
--------------------------------
4     Monday    14:45     20:15
4     Tuesday   14:45     18:00
4     Thursday  16:00     20:15
7     Monday    17:30     20:15
```

Executing:

```text
DeleteAllAvailability(4)
```

removes all three rows where:

```sql
id = 4
```

but leaves staff ID `7` unchanged.

## Typical Use

This is useful when the application wants to completely replace a staff member's availability.

The general pattern is:

```text
Existing availability
        |
        v
DeleteAllAvailability(id)
        |
        v
Create new Availability rows
```

---

# `DeleteDaysOff.sql`

## Purpose

Deletes one specific day-off entry for a staff member.

## SQL

```sql
DELETE FROM
    [UWGB].[DaysOff]
WHERE
    id = @id
    AND Date = @date
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff member ID. |
| `@date` | Specific day-off date to remove. |

## Called By

```csharp
DatabaseDelete.DeleteDaysOff(
    int id,
    List<DateOnly> dates)
```

The SQL file removes one staff/date combination per execution.

The C# method handles deleting multiple dates by looping:

```csharp
foreach (DateOnly date in dates)
{
    ExecuteNonQuery(
        Queries.DeleteDaysOff,
        new SqlParameter("@id", id),
        new SqlParameter("@date", date));
}
```

The resulting flow is:

```text
List<DateOnly>
      |
      +--> date 1 -> DELETE
      |
      +--> date 2 -> DELETE
      |
      +--> date 3 -> DELETE
```

## Why Both Conditions Matter

The query uses:

```sql
id = @id
AND Date = @date
```

rather than deleting by date alone.

This prevents deleting another staff member's day-off entry when multiple staff members requested the same date.

## Target Table

```text
UWGB.DaysOff
```

---

# `DeleteJobSettingsOnDay.sql`

## Purpose

Deletes the job-settings record for one day of the week.

## SQL

```sql
DELETE
    FROM
        [UWGB].[JobSettings]
WHERE
    DayOfWeek = @DayOfWeek
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@DayOfWeek` | Day whose job settings should be removed. |

## Called By

```csharp
DatabaseDelete.DeleteJobSettingsOnDay(
    DayOfWeek dayOfWeek)
```

The enum is converted to text before being supplied to SQL Server:

```csharp
new SqlParameter(
    "@DayOfWeek",
    dayOfWeek.ToString())
```

For example:

```csharp
DatabaseDelete.DeleteJobSettingsOnDay(
    DayOfWeek.Monday);
```

results in a parameter value equivalent to:

```text
Monday
```

## Target Table

```text
UWGB.JobSettings
```

Because `DayOfWeek` is the primary key in the current schema, this query should remove at most one row.

---

# `DeleteAllJobSettings.sql`

## Purpose

Deletes **every row** from the JobSettings table.

## SQL

```sql
DELETE FROM [UWGB].[JobSettings]
```

## Parameters

None.

## Called By

```csharp
DatabaseDelete.DeleteAllJobSettings()
```

which executes:

```csharp
ExecuteNonQuery(
    Queries.DeleteAllJobSettings);
```

## Target Table

```text
UWGB.JobSettings
```

## Destructive Scope

This query intentionally has **no `WHERE` clause**.

That means:

```text
Monday settings
Tuesday settings
Wednesday settings
Thursday settings
...
```

are all removed in one operation.

Use this query only when the application intentionally wants to reset all configured operating days.

It should not be used when removing a single day. Use:

```csharp
DeleteJobSettingsOnDay(...)
```

for that case.

---

# `DeleteAllByID.sql`

## Current Status

This file is currently **not part of the active query-resource pipeline**.

It is present at:

```text
Queries/Delete/DeleteAllByID.sql
```

but it is not currently registered in `Queries.resx`, so the application does not expose an active:

```csharp
Queries.DeleteAllByID
```

resource.

The current C# method:

```csharp
DatabaseDelete.DeleteAllByID(int id)
```

uses an inline raw SQL string instead.

As a result, modifying this `.sql` file currently does **not** change the behavior of `DatabaseDelete.DeleteAllByID`.

---

## Current File Contents

The file currently attempts to perform several deletions:

```sql
DELETE FROM
    [UWGB].[Availability]
WHERE
    id = @id;

DELETE FROM
    [UWGB].[DaysOff]
WHERE
    id = @id;

DELETE FROM
    [UWGB].[Staff]
WHERE
    id = @id;

DELETE FROM
    [UWGB].[Staff]
WHERE
    staffID = @id;
```

The apparent intent is to remove data associated with a staff member before or while deleting the staff record.

---

## Current Problem in the Final Statement

The final statement is:

```sql
DELETE FROM
    [UWGB].[Staff]
WHERE
    staffID = @id;
```

However, the current `UWGB.Staff` schema uses:

```text
id
fName
mName
lName
position
email
belayCert
certifiedOn
expiresOn
```

and does **not** contain a `staffID` column.

The `staffID` column belongs to:

```text
UWGB.Schedule
```

Therefore, the final statement appears to have been intended to target schedule rows:

```sql
DELETE FROM
    [UWGB].[Schedule]
WHERE
    staffID = @id;
```

This README documents the current file as written; it does not assume that correction has been made.

---

# Difference Between `DeleteAllByID.sql` and `DatabaseDelete.DeleteAllByID`

This is an important maintenance distinction.

## Standalone SQL File

`Queries/Delete/DeleteAllByID.sql` currently attempts:

```text
Availability by staff ID
        |
DaysOff by staff ID
        |
Staff by ID
        |
Staff WHERE staffID = ID   <-- inconsistent
```

## Current C# Service Method

`DatabaseDelete.DeleteAllByID` currently executes its own inline SQL instead:

```sql
DELETE FROM [UWGB].[Staff]
WHERE id = @id;

DELETE FROM [UWGB].[Availability]
WHERE id = @id;

DELETE FROM [UWGB].[DaysOff]
WHERE id = @id;

DELETE FROM [UWGB].[JobSettings];
```

This means the active method currently has a different behavior than the `.sql` file.

Most importantly, the inline method executes:

```sql
DELETE FROM [UWGB].[JobSettings];
```

which deletes **all job settings**, even though job settings are not tied to a staff ID.

The method also contains a code comment noting that schedule rows containing the deleted staff ID still need to be handled.

---

# Staff Deletion Logic

Removing a staff member is more complicated than deleting only the `UWGB.Staff` row because other tables can contain data associated with that person.

Conceptually, staff deletion needs to consider:

```text
Staff ID
   |
   +--> UWGB.Availability.id
   |
   +--> UWGB.DaysOff.id
   |
   +--> UWGB.Schedule.staffID
   |
   +--> UWGB.Staff.id
```

A safe cleanup operation needs to decide what should happen to each relationship.

For example, schedule rows could potentially be:

```text
Option A
DELETE schedule rows referencing the staff member

Option B
UPDATE schedule rows
SET staffID = NULL

Option C
Prevent staff deletion while referenced
```

The current database schema does not define foreign-key constraints for these relationships, so this behavior is currently controlled by application logic.

---

# Delete Order

When several related rows need to be removed, dependent rows should generally be handled before the primary staff record.

Conceptually:

```text
1. Availability
2. Days Off
3. Schedule references
4. Staff
```

This order is particularly important if foreign keys are added in the future.

For example:

```sql
DELETE FROM [UWGB].[Availability]
WHERE id = @id;

DELETE FROM [UWGB].[DaysOff]
WHERE id = @id;

DELETE FROM [UWGB].[Schedule]
WHERE staffID = @id;

DELETE FROM [UWGB].[Staff]
WHERE id = @id;
```

This is an example of the logical relationship between the data; the current active implementation should be checked before changing production behavior.

---

# Parameterization

Active parameterized delete queries use values such as:

```sql
WHERE id = @id
```

and then provide the parameter separately:

```csharp
new SqlParameter(
    "@id",
    id)
```

This should remain the standard for future delete operations.

Avoid constructing statements such as:

```csharp
$"DELETE FROM [UWGB].[Staff] WHERE id = {id}"
```

Parameterized SQL:

- keeps SQL syntax separate from data
- avoids manual escaping
- improves consistency
- reduces SQL injection risk
- makes service methods easier to maintain

---

# Relationship to `DatabaseDelete`

The SQL files define **which rows SQL Server should remove**.

`DatabaseDelete` defines the application-facing operation and supplies the required values.

For example:

```text
DatabaseDelete.DeleteDaysOff(...)
          |
          | loops through selected dates
          v
Queries.DeleteDaysOff
          |
          | @id + @date
          v
DeleteDaysOff.sql
          |
          v
UWGB.DaysOff
```

This separation means the SQL file should stay focused on the database operation itself.

Business rules such as:

- whether deletion should be allowed
- whether confirmation is required
- which dates the user selected
- whether dependent records should also be removed

belong in the application/service layer rather than in a simple single-purpose query.

---

# Adding a New Delete Query

When adding another delete operation, follow the existing resource-based pattern.

## 1. Create the SQL File

Example:

```text
Queries/
└── Delete/
    └── DeleteScheduleByName.sql
```

Example SQL:

```sql
DELETE FROM
    [UWGB].[Schedule]
WHERE
    scheduleName = @scheduleName
```

## 2. Register It in `Queries.resx`

The resource entry should point to the SQL file so Visual Studio generates:

```csharp
Queries.DeleteScheduleByName
```

## 3. Add the Service Method

Place the application-facing method in:

```text
Services/
└── Database/
    └── DatabaseDelete.cs
```

Example:

```csharp
public static void DeleteScheduleByName(
    string scheduleName)
{
    ExecuteNonQuery(
        Queries.DeleteScheduleByName,
        new SqlParameter(
            "@scheduleName",
            scheduleName));
}
```

## 4. Keep SQL and C# Parameter Names Synchronized

This:

```sql
@scheduleName
```

must exactly match:

```csharp
new SqlParameter(
    "@scheduleName",
    scheduleName)
```

A mismatch will cause the command to fail at runtime.

## 5. Verify the Scope Before Adding the Query

For every `DELETE`, explicitly determine whether the operation is intended to remove:

```text
one row
several matching rows
all rows belonging to one entity
the entire table
```

Then ensure the `WHERE` clause reflects that intent.

---

# Destructive Query Checklist

Before changing or adding a delete query, verify:

- [ ] The correct table is targeted.
- [ ] The `WHERE` clause exists unless a full-table delete is intentional.
- [ ] All parameter names match the corresponding C# `SqlParameter` names.
- [ ] Related/dependent data has been considered.
- [ ] The delete does not accidentally affect unrelated staff or schedules.
- [ ] The query is registered in `Queries.resx` if it is intended to be active.
- [ ] The corresponding `DatabaseDelete` method actually references the resource.
- [ ] Full-table deletes are clearly named and intentionally invoked.

---

# Current Maintenance Notes

The following details reflect the current repository implementation:

- `DeleteAllAvailability.sql`, `DeleteDaysOff.sql`, `DeleteJobSettingsOnDay.sql`, and `DeleteAllJobSettings.sql` are active query resources.
- `DeleteAllByID.sql` exists in the folder but is not currently registered in `Queries.resx`.
- `DatabaseDelete.DeleteAllByID` currently uses inline SQL instead of `DeleteAllByID.sql`.
- The standalone `DeleteAllByID.sql` appears to contain an incorrect final table/column combination: `[UWGB].[Staff] WHERE staffID = @id`.
- `staffID` exists on `UWGB.Schedule`, not `UWGB.Staff`.
- The active inline `DatabaseDelete.DeleteAllByID` currently deletes all rows from `UWGB.JobSettings`.
- The active staff-deletion method does not currently clean up schedule rows containing the removed staff ID.
- `DeleteAllJobSettings.sql` intentionally contains no `WHERE` clause and removes every job-settings row.
- The current schema does not define foreign-key relationships between staff and availability, days off, or schedules, so cleanup behavior is controlled by application logic.

Update this README whenever delete-query parameters, table relationships, resource registration, or `DatabaseDelete` behavior changes.
