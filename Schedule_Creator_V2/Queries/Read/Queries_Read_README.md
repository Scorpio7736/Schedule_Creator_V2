# Read Queries

This folder contains the SQL `SELECT` statements used by Schedule Creator V2 to retrieve application data from SQL Server LocalDB.

The queries are registered through `Queries.resx`, exposed as strongly named properties on the generated `Queries` class, and are primarily consumed by:

```text
Services/
└── Database/
    └── DatabaseRead.cs
```

The normal read flow is:

```text
View / Application Logic
        |
        v
DatabaseRead
        |
        v
Queries.<ReadQuery>
        |
        v
SQL file in Queries/Read
        |
        v
Database.ExecuteReader
        |
        v
SqlDataReader
        |
        v
C# record / enum / collection
```

The SQL files decide **which rows and columns are returned**. `DatabaseRead` is responsible for translating those SQL values into the strongly typed objects used elsewhere in the application.

---

## Folder Overview

| File | Query Resource | Primary Consumer | Result |
| --- | --- | --- | --- |
| `ReadAllScheduleNames.sql` | `Queries.ReadAllScheduleNames` | `DatabaseRead.ReadAllScheduleNames()` | `List<string>` |
| `ReadAvailByID.sql` | `Queries.ReadAvailByID` | `DatabaseRead.ReadAvailByID(int)` | `List<Availability>` |
| `ReadAvailForStaffByID.sql` | `Queries.ReadAvailForStaffByID` | `DatabaseRead.ReadAvailForStaffByID(int)` | `List<Availability>` |
| `ReadDaysOff.sql` | `Queries.ReadDaysOff` | `DatabaseRead.ReadDaysOff()` | `List<DaysOff>` |
| `ReadJobSettings.sql` | `Queries.ReadJobSettings` | `DatabaseRead.ReadJobSettings()` | `List<JobSettings>` |
| `ReadJobSettingsDays.sql` | `Queries.ReadJobSettingsDays` | `DatabaseRead.ReadJobSettingsDays()` | `List<DayOfWeek>` |
| `ReadSchedule.sql` | `Queries.ReadSchedule` | No current `DatabaseRead` wrapper | Raw schedule rows |
| `ReadScheduleByScheduleName.sql` | `Queries.ReadScheduleByScheduleName` | `DatabaseRead.ReadScheduleByScheduleName(string)` | `List<ScheduleRow>` |
| `ReadScheduleOnDay.sql` | `Queries.ReadScheduleOnDay` | No current `DatabaseRead` wrapper | Schedule rows for one day |
| `ReadStaff.sql` | `Queries.ReadStaff` | `DatabaseRead.ReadStaff()` | `List<Staff>` |
| `ReadStaffAvailOnDay.sql` | `Queries.ReadStaffAvailOnDay` | `DatabaseRead.ReadStaffAvailOnDay(DayOfWeek)` | `List<Staff>` |
| `ReadStaffByID.sql` | `Queries.ReadStaffByID` | `DatabaseRead.ReadStaffByID(int)` | `Staff` |
| `ReadStaffNamesAndAvailOnDay.sql` | `Queries.ReadStaffNamesAndAvailOnDay` | `DatabaseRead.ReadStaffNamesAndAvailOnDay(DayOfWeek)` | `List<StaffNameAndAvail>` |
| `ReadStaffWithNoAvail.sql` | `Queries.ReadStaffWithNoAvail` | `DatabaseRead.ReadStaffWithNoAvail()` | `List<Staff>` |

---

# How Read Queries Are Executed

Most read operations use:

```csharp
Database.ExecuteReader(...)
```

which returns a `SqlDataReader`.

A typical pattern is:

```csharp
List<Staff> returnList =
    new List<Staff>();

using (var reader =
    ExecuteReader(Queries.ReadStaff))
{
    while (reader.Read())
    {
        returnList.Add(
            new Staff(
                // map columns here
            ));
    }
}

return returnList;
```

The SQL query returns database values, while `DatabaseRead` converts those values into application types.

---

# Common Data Conversion Logic

Several conversions appear repeatedly in `DatabaseRead`.

## SQL `TIME` to `TimeOnly`

SQL Server returns a `TIME` column as `TimeSpan`.

The service converts it using:

```csharp
TimeOnly.FromTimeSpan(
    (TimeSpan)reader["startTime"])
```

The reverse conversion occurs in `DatabaseCreate`.

---

## SQL `DATE` to `DateOnly`

SQL Server returns a `DATE` as `DateTime`.

The service converts it using:

```csharp
DateOnly.FromDateTime(
    (DateTime)reader["date"])
```

Nullable date fields first check for `DBNull`.

---

## Stored Enum Text to C# Enum

Fields such as staff positions and some day-of-week values are stored as text.

They are reconstructed using:

```csharp
Enum.Parse<Positions>(
    (string)reader["position"])
```

or:

```csharp
Enum.Parse<DayOfWeek>(
    (string)reader["DayOfWeek"])
```

This means stored text must match a valid enum name.

---

## SQL `NULL` to Nullable C# Value

For values that can be null, the service checks:

```csharp
reader["column"] is DBNull
```

before converting.

For example:

```csharp
reader["staffID"] is DBNull
    ? null
    : (int)reader["staffID"]
```

---

# `ReadAllScheduleNames.sql`

## Purpose

Returns each distinct saved schedule name.

## SQL

```sql
SELECT DISTINCT
    scheduleName
FROM
    [UWGB].[Schedule]
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadAllScheduleNames()
```

## Returned Column

```text
scheduleName
```

The service maps each value into:

```csharp
List<string>
```

---

## Why `DISTINCT` Is Required

A saved schedule is represented by multiple rows in `UWGB.Schedule`.

For example:

```text
scheduleName             day         staffID
------------------------------------------------
Fall 26 - Final Draft    Monday      1
Fall 26 - Final Draft    Monday      4
Fall 26 - Final Draft    Tuesday     1
Fall 26 - Final Draft    Wednesday   7
```

Without `DISTINCT`, the schedule name would appear once for every shift row.

With:

```sql
SELECT DISTINCT scheduleName
```

the result becomes:

```text
Fall 26 - Final Draft
```

only once.

This method is used by the schedule-building logic to identify existing schedule names and prevent duplicates.

---

# `ReadAvailByID.sql`

## Purpose

Returns all availability rows belonging to one staff ID.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Availability]
WHERE
    id = @id
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff ID whose availability should be returned. |

## Called By

```csharp
DatabaseRead.ReadAvailByID(
    int id)
```

## Returned Columns

Because the query uses:

```sql
SELECT *
```

the current table returns:

```text
id
dayOfTheWeek
startTime
endTime
```

## C# Result

Each row becomes:

```csharp
Availability
```

and the method returns:

```csharp
List<Availability>
```

---

# `ReadAvailForStaffByID.sql`

## Purpose

Also returns all availability rows belonging to one staff member.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Availability]
WHERE
    @id = id
```

This is functionally equivalent to:

```sql
WHERE id = @id
```

used by `ReadAvailByID.sql`.

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff ID whose availability should be returned. |

## Called By

```csharp
DatabaseRead.ReadAvailForStaffByID(
    int id)
```

## C# Result

The rows are mapped into:

```csharp
List<Availability>
```

and then explicitly sorted before returning:

```csharp
return returnList
    .OrderBy(a => a.dayOfTheWeek)
    .ToList();
```

---

## Difference from `ReadAvailByID`

At the SQL level, the two queries are effectively duplicates.

```text
ReadAvailByID
    -> WHERE id = @id

ReadAvailForStaffByID
    -> WHERE @id = id
```

The primary behavioral difference is in C#:

```text
ReadAvailByID
    -> returns database order

ReadAvailForStaffByID
    -> sorts by dayOfTheWeek
```

If this area is refactored later, these two query resources may be candidates for consolidation.

---

# `ReadDaysOff.sql`

## Purpose

Returns all stored day-off records.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[DaysOff]
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadDaysOff()
```

## Returned Columns

```text
id
Date
reason
```

## C# Mapping

Each row becomes:

```csharp
new DaysOff(
    (int)reader["id"],
    DateOnly.FromDateTime(
        (DateTime)reader["date"]),
    (string)reader["reason"])
```

The final result is:

```csharp
List<DaysOff>
```

---

# `ReadJobSettings.sql`

## Purpose

Returns all configured operating-day settings.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[JobSettings]
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadJobSettings()
```

## Returned Columns

```text
DayOfWeek
OpeningTime
ClosingTime
```

## C# Mapping

Each row becomes:

```csharp
JobSettings
```

with:

```text
DayOfWeek string
    -> DayOfWeek enum

SQL TIME
    -> TimeSpan
    -> TimeOnly
```

Example conversion:

```csharp
new JobSettings(
    Enum.Parse<DayOfWeek>(
        (string)reader["DayOfWeek"]),

    TimeOnly.FromTimeSpan(
        (TimeSpan)reader["OpeningTime"]),

    TimeOnly.FromTimeSpan(
        (TimeSpan)reader["ClosingTime"]))
```

---

# `ReadJobSettingsDays.sql`

## Purpose

Returns only the days that currently have job settings configured.

## SQL

```sql
SELECT DISTINCT
    DayOfWeek
FROM
    [UWGB].[JobSettings]
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadJobSettingsDays()
```

## C# Result

Each stored day name is converted to:

```csharp
DayOfWeek
```

and returned as:

```csharp
List<DayOfWeek>
```

---

## Why This Query Exists Separately

Some parts of the UI do not need opening and closing times.

They only need to know:

```text
Which days are configured?
```

For example:

```text
Monday      configured
Tuesday     configured
Wednesday   configured
Thursday    configured
Friday      not configured
```

This query avoids requiring the UI to load complete `JobSettings` records just to determine active schedule days.

---

# `ReadSchedule.sql`

## Purpose

Returns every row stored in `UWGB.Schedule`.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Schedule]
```

## Parameters

None.

## Resource Status

This SQL file is registered as:

```csharp
Queries.ReadSchedule
```

but the current `DatabaseRead.cs` does **not** contain a corresponding:

```csharp
ReadSchedule()
```

method.

Therefore, the query resource exists but is not currently wrapped by the main database-read service.

---

## Expected Returned Columns

```text
dayOfWeek
staffID
scheduleName
startTime
endTime
```

A future wrapper would likely map each row into:

```csharp
ScheduleRow
```

similar to `ReadScheduleByScheduleName`.

---

# `ReadScheduleByScheduleName.sql`

## Purpose

Returns every shift row belonging to one named schedule.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Schedule]
WHERE
    scheduleName = @scheduleName
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@scheduleName` | Saved schedule name to retrieve. |

## Called By

```csharp
DatabaseRead.ReadScheduleByScheduleName(
    string scheduleName)
```

## C# Mapping

Each result becomes:

```csharp
ScheduleRow
```

The service converts:

```text
dayOfWeek
    -> DayOfWeek enum

staffID
    -> nullable int

startTime
    -> TimeOnly

endTime
    -> TimeOnly

scheduleName
    -> string
```

Example:

```csharp
new ScheduleRow(
    Enum.Parse<DayOfWeek>(
        (string)reader["dayOfWeek"]),

    reader["staffID"] is DBNull
        ? null
        : (int)reader["staffID"],

    TimeOnly.FromTimeSpan(
        (TimeSpan)reader["startTime"]),

    TimeOnly.FromTimeSpan(
        (TimeSpan)reader["endTime"]),

    (string)reader["scheduleName"])
```

---

# `ReadScheduleOnDay.sql`

## Purpose

Returns schedule rows for one day of the week.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Schedule]
WHERE
    dayOfWeek = @dayOfWeek
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@dayOfWeek` | Day whose schedule rows should be returned. |

## Resource Status

This query is registered as:

```csharp
Queries.ReadScheduleOnDay
```

but there is currently no matching public method in `DatabaseRead.cs`.

The query therefore exists as an available SQL resource but is not currently wrapped by the primary read service.

---

# `ReadStaff.sql`

## Purpose

Returns every staff record.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Staff];
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadStaff()
```

## C# Result

Each row is converted into:

```csharp
Staff
```

and returned as:

```csharp
List<Staff>
```

The mapper reads:

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

---

## Certification Conversion

The current schema stores:

```text
belayCert
```

as text.

The reader converts it using:

```csharp
bool.Parse(
    (string)reader["belayCert"])
```

The certification dates are nullable:

```csharp
reader["certifiedOn"] is DBNull
    ? null
    : DateOnly.FromDateTime(
        (DateTime)reader["certifiedOn"])
```

The same pattern is used for `expiresOn`.

---

# `ReadStaffAvailOnDay.sql`

## Purpose

Returns staff members who have at least one Availability row for the requested day.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Staff]
WHERE
    id IN (
        SELECT
            id
        FROM
            [UWGB].[Availability]
        WHERE
            dayOfTheWeek = @dayOfTheWeek
    );
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@dayOfTheWeek` | Day used to filter availability records. |

## Called By

```csharp
DatabaseRead.ReadStaffAvailOnDay(
    DayOfWeek dayOfTheWeek)
```

## Query Logic

The inner query finds staff IDs with availability on the requested day:

```sql
SELECT id
FROM [UWGB].[Availability]
WHERE dayOfTheWeek = @dayOfTheWeek
```

The outer query then retrieves the complete Staff records:

```text
Availability
     |
     | IDs available that day
     v
Staff
     |
     v
complete Staff records
```

---

## Important Scope Detail

This query checks only:

```text
dayOfTheWeek
```

It does **not** compare:

```text
startTime
endTime
```

Therefore, the method means:

```text
"staff with some availability on this day"
```

rather than:

```text
"staff available for a specific requested shift time"
```

Any time-range validation must occur elsewhere.

---

# `ReadStaffByID.sql`

## Purpose

Returns the staff record matching one ID.

## SQL

```sql
SELECT
    *
FROM
    [UWGB].[Staff]
WHERE
    id = @id
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@id` | Staff ID to retrieve. |

## Called By

```csharp
DatabaseRead.ReadStaffByID(
    int id)
```

## Expected Result

Because `Staff.id` is the primary key, the query should return either:

```text
one row
```

or:

```text
no rows
```

---

## Current No-Row Behavior

If a matching row exists, the service returns a populated:

```csharp
Staff
```

If no row exists, the current implementation does **not** return `null`.

Instead, it returns:

```csharp
new Staff(
    1,
    "",
    "",
    "",
    Positions.Unknown,
    "",
    false,
    null,
    null)
```

This placeholder behavior is part of the current service contract and should be considered when calling this method.

---

# `ReadStaffNamesAndAvailOnDay.sql`

## Purpose

Returns a compact combination of staff identity information and availability times for one day.

## SQL

```sql
SELECT
    s.id,
    s.fName,
    s.lName,
    a.dayOfTheWeek,
    a.startTime,
    a.endTime
FROM
    [UWGB].[Staff] s
RIGHT JOIN
    [UWGB].[Availability] a
ON
    s.id = a.id
WHERE
    a.dayOfTheWeek = @day
```

## Parameters

| Parameter | Meaning |
| --- | --- |
| `@day` | Availability day to retrieve. |

## Called By

```csharp
DatabaseRead.ReadStaffNamesAndAvailOnDay(
    DayOfWeek day)
```

## C# Result

Each row becomes:

```csharp
StaffNameAndAvail
```

containing:

```text
staff ID
first name
last name
availability start time
availability end time
```

---

## Why This Query Is Useful

The schedule-building UI often does not need an entire Staff record.

It needs something closer to:

```text
Riley
2:45 PM - 8:15 PM
```

This query retrieves only the identity and availability information needed for that type of display.

---

## `RIGHT JOIN` Behavior

The current query uses:

```sql
RIGHT JOIN [UWGB].[Availability] a
```

which means Availability is the preserved side of the join.

Conceptually:

```text
Availability row
      |
      +--> matching Staff row exists
      |       -> staff values returned
      |
      +--> matching Staff row missing
              -> Staff columns may be NULL
```

Because the current schema does not define a foreign key from `Availability.id` to `Staff.id`, an orphaned Availability row is technically possible.

However, the C# mapper currently assumes:

```csharp
(int)reader["id"]
(string)reader["fName"]
(string)reader["lName"]
```

are non-null.

Therefore, an orphaned Availability row could cause a mapping exception.

This is an important consideration if staff-deletion or database-integrity logic is changed.

---

# `ReadStaffWithNoAvail.sql`

## Purpose

Returns staff members who have no rows in `UWGB.Availability`.

## SQL

```sql
SELECT
    s.id,
    s.fName,
    s.mName,
    s.lName,
    s.position,
    s.email,
    s.belayCert,
    s.certifiedOn,
    s.expiresOn
FROM
    UWGB.Staff AS s
WHERE NOT EXISTS (
    SELECT 1
    FROM UWGB.Availability AS a
    WHERE a.id = s.id
);
```

## Parameters

None.

## Called By

```csharp
DatabaseRead.ReadStaffWithNoAvail()
```

## Query Logic

For each staff member, SQL checks whether a matching Availability row exists.

```text
Staff
  |
  +--> Availability exists?
          |
          +--> yes -> exclude
          |
          +--> no  -> return staff member
```

The key portion is:

```sql
WHERE NOT EXISTS (...)
```

---

## Why `SELECT 1` Is Used

Inside `EXISTS` or `NOT EXISTS`, SQL only needs to determine whether a matching row exists.

The selected value itself is irrelevant.

Therefore:

```sql
SELECT 1
```

communicates:

```text
"Only test for existence."
```

rather than requesting meaningful column data from the subquery.

---

# Full Staff Mapping

Several read methods map SQL rows into the same `Staff` record:

```text
ReadStaff
ReadStaffByID
ReadStaffAvailOnDay
ReadStaffWithNoAvail
```

They use the same general transformation:

```text
SQL row
   |
   +--> id                -> int
   +--> fName             -> string
   +--> mName             -> string
   +--> lName             -> string
   +--> position          -> Positions enum
   +--> email             -> string
   +--> belayCert         -> bool
   +--> certifiedOn       -> DateOnly?
   +--> expiresOn         -> DateOnly?
   |
   v
Staff
```

If the Staff schema changes, all of these mappings should be reviewed together.

---

# `SELECT *` Convention

Several current queries use:

```sql
SELECT *
```

including:

```text
ReadAvailByID
ReadAvailForStaffByID
ReadDaysOff
ReadJobSettings
ReadSchedule
ReadScheduleByScheduleName
ReadScheduleOnDay
ReadStaff
ReadStaffAvailOnDay
ReadStaffByID
```

This is convenient because the queries automatically return every column in the table.

However, it also couples the reader more tightly to the table structure.

For example, if table columns are later:

```text
renamed
removed
changed in type
```

the C# mapper may need to be updated.

Queries such as `ReadStaffWithNoAvail.sql` explicitly list columns, which makes the expected result shape easier to see from the SQL file itself.

---

# Query Parameters

Parameterized read queries use patterns such as:

```sql
WHERE id = @id
```

with the C# service supplying:

```csharp
new SqlParameter(
    "@id",
    id)
```

Current read-query parameters include:

| Parameter | Used By |
| --- | --- |
| `@id` | Availability and Staff ID queries |
| `@scheduleName` | `ReadScheduleByScheduleName` |
| `@dayOfWeek` | `ReadScheduleOnDay` |
| `@dayOfTheWeek` | `ReadStaffAvailOnDay` |
| `@day` | `ReadStaffNamesAndAvailOnDay` |

The SQL parameter name must match the `SqlParameter` supplied by C#.

---

# Query Result Ordering

Most current SELECT queries contain no:

```sql
ORDER BY
```

clause.

SQL Server therefore does not guarantee a specific result order.

For example:

```sql
SELECT *
FROM [UWGB].[Staff]
```

should not be assumed to return staff alphabetically or by ID unless the query explicitly specifies that order.

One current exception is handled in C# rather than SQL:

```csharp
ReadAvailForStaffByID(...)
```

sorts its returned collection using:

```csharp
OrderBy(a => a.dayOfTheWeek)
```

after the rows are read.

When deterministic ordering is important, add an explicit `ORDER BY` in SQL or clearly sort in C#.

---

# Relationship to the Database Schema

The read queries rely on the tables created by:

```text
Queries/Migration/CreateTables.sql
```

The main relationships are:

```text
                 +------------------+
                 |    UWGB.Staff    |
                 |       id         |
                 +--------+---------+
                          |
             +------------+-------------+
             |                          |
             v                          v
+-------------------------+   +----------------------+
| UWGB.Availability       |   | UWGB.DaysOff        |
| id                      |   | id                   |
| dayOfTheWeek            |   | Date                 |
| startTime               |   | reason               |
| endTime                 |   +----------------------+
+-------------------------+
             |
             | staff ID is also used by
             v
+-------------------------+
| UWGB.Schedule           |
| staffID                 |
| scheduleName            |
| dayOfWeek               |
| startTime               |
| endTime                 |
+-------------------------+
```

These are logical application relationships. The current schema does not define foreign-key constraints for them.

---

# Adding a New Read Query

When adding another read operation, follow the existing resource/service pattern.

## 1. Create the SQL File

Example:

```text
Queries/
└── Read/
    └── ReadScheduleByStaffID.sql
```

Example:

```sql
SELECT
    *
FROM
    [UWGB].[Schedule]
WHERE
    staffID = @staffID
```

---

## 2. Register the Query in `Queries.resx`

The SQL file should be registered so Visual Studio generates:

```csharp
Queries.ReadScheduleByStaffID
```

---

## 3. Add a `DatabaseRead` Method

Example:

```csharp
public static List<ScheduleRow>
    ReadScheduleByStaffID(
        int staffID)
{
    List<ScheduleRow> returnList =
        new List<ScheduleRow>();

    using (var reader =
        ExecuteReader(
            Queries.ReadScheduleByStaffID,
            new SqlParameter(
                "@staffID",
                staffID)))
    {
        while (reader.Read())
        {
            // map ScheduleRow
        }
    }

    return returnList;
}
```

---

## 4. Map Database Types in the Service Layer

Keep SQL-to-C# transformations inside `DatabaseRead`.

For example:

```text
TIME
    -> TimeOnly

DATE
    -> DateOnly

NVARCHAR position
    -> Positions enum

DBNull
    -> nullable C# value
```

Views should receive application objects rather than handling `SqlDataReader` directly.

---

## 5. Add Explicit Ordering When Required

If a caller depends on ordering, either:

```sql
ORDER BY ...
```

in the query or:

```csharp
.OrderBy(...)
```

in the service.

Do not rely on SQL Server's incidental row order.

---

# Read Query Checklist

When creating or changing a SELECT query, verify:

- [ ] The query targets the correct `[UWGB]` table.
- [ ] Required parameters match the C# `SqlParameter` names.
- [ ] Every column expected by `DatabaseRead` is returned.
- [ ] Nullable SQL values are handled with `DBNull` where necessary.
- [ ] Stored enum values can be parsed into the expected C# enum.
- [ ] SQL `TIME` values are converted to `TimeOnly`.
- [ ] SQL `DATE` values are converted to `DateOnly`.
- [ ] Result ordering is explicit if callers depend on it.
- [ ] Join behavior cannot unexpectedly produce `NULL` values the mapper cannot handle.
- [ ] The SQL file is registered in `Queries.resx`.
- [ ] A corresponding `DatabaseRead` wrapper exists if the query is intended for application use.

---

# Current Maintenance Notes

The following details reflect the current repository implementation:

- All current files in `Queries/Read` are registered as query resources.
- `ReadSchedule.sql` is registered but does not currently have a matching `DatabaseRead.ReadSchedule()` method.
- `ReadScheduleOnDay.sql` is registered but does not currently have a matching `DatabaseRead.ReadScheduleOnDay()` method.
- `ReadAvailByID.sql` and `ReadAvailForStaffByID.sql` are functionally equivalent at the SQL level.
- `ReadAvailForStaffByID()` sorts the result by `dayOfTheWeek`; `ReadAvailByID()` does not.
- `ReadStaffAvailOnDay.sql` checks only whether staff have availability on a day; it does not verify that their availability covers a particular start/end time.
- `ReadStaffNamesAndAvailOnDay.sql` uses a `RIGHT JOIN`, preserving Availability rows even if the associated Staff row is missing.
- Because there are currently no foreign-key constraints between Availability and Staff, orphaned availability rows are technically possible.
- The `ReadStaffNamesAndAvailOnDay()` mapper assumes Staff values are non-null, so an orphaned row could cause an exception.
- `Staff.mName` is nullable in the database schema, while the current Staff mapping code casts `reader["mName"]` directly to `string` in several methods. A database `NULL` middle name could therefore require additional handling.
- Most queries use `SELECT *`, so their result shape depends directly on the current table schema.
- Most read queries do not specify `ORDER BY`; callers should not assume deterministic database ordering.
- `ReadStaffByID()` currently returns a placeholder `Staff` object rather than `null` when the requested ID is not found.
- `belayCert` is stored as text and parsed using `bool.Parse()`, so its stored value must remain a valid Boolean string.
- Day-of-week storage varies across tables, so different readers use either integer-backed or string-backed enum conversion depending on the source table.

Update this README whenever SELECT logic, query parameters, database schemas, result models, or `DatabaseRead` mappings change.
