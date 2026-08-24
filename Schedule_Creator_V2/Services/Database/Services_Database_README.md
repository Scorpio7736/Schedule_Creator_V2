# Database Services

This folder contains the data-access layer for Schedule Creator V2. Its responsibility is to isolate SQL Server / LocalDB operations from the rest of the application and provide simple C# methods for creating, reading, updating, deleting, and initializing application data.

The database services use `Microsoft.Data.SqlClient` and SQL statements exposed through the generated `Queries` resource class. Most application code should call these service methods instead of creating `SqlConnection`, `SqlCommand`, or raw SQL directly.

## Folder Overview

| File | Responsibility |
| --- | --- |
| `Database.cs` | Shared SQL execution helpers and connection creation. |
| `DatabaseCreate.cs` | INSERT operations for schedules, job settings, staff, days off, and availability. |
| `DatabaseRead.cs` | SELECT operations and conversion of database rows into application models/records. |
| `DatabaseUpdate.cs` | UPDATE operations for staff information and belay certification. |
| `DatabaseDelete.cs` | DELETE operations for availability, days off, job settings, and staff-related data. |
| `DataMigragtion.cs` | First-run database creation and schema initialization. |

> **Note:** `DataMigragtion.cs` is the current filename/class name in the project. The spelling is preserved here so the documentation matches the code.

---

## Architecture

The database layer follows a CRUD-oriented service structure:

```text
Views / Application Logic
          |
          v
+-------------------------+
| DatabaseCreate          |
| DatabaseRead            |
| DatabaseUpdate          |
| DatabaseDelete          |
+-------------------------+
          |
          v
+-------------------------+
| Database                |
| ExecuteNonQuery         |
| ExecuteReader           |
| ExecuteScalar<T>        |
+-------------------------+
          |
          v
+-------------------------+
| Queries resource class  |
| parameterized SQL       |
+-------------------------+
          |
          v
SQL Server LocalDB
Schedule_Creator_V2
```

The CRUD classes inherit from `Database`, allowing them to reuse the shared execution methods while keeping database operations grouped by purpose.

The primary application connection string is named `LocalDbConnection`. A second connection, `LocalDbConnectionMaster`, is used only when the application must connect to SQL Server's `master` database to create the application's LocalDB database for the first time.

---

# `Database.cs`

`Database.cs` is the base data-access class used by the other services in this folder.

Its main job is to:

1. Read the `LocalDbConnection` connection string from `App.config`.
2. Open a SQL Server LocalDB connection.
3. Create a `SqlCommand` attached to that connection.
4. Add any supplied `SqlParameter` objects.
5. Execute the command using the appropriate SQL execution method.

## `ExecuteNonQuery`

```csharp
Database.ExecuteNonQuery(commandText, parameters);
```

Used when the SQL command does not need to return rows.

Typical uses include:

- `INSERT`
- `UPDATE`
- `DELETE`
- schema creation

The method creates a command, adds all supplied parameters, calls `ExecuteNonQuery()`, and disposes the command afterward.

## `ExecuteReader`

```csharp
using (var reader = ExecuteReader(commandText, parameters))
{
    while (reader.Read())
    {
        // map database values
    }
}
```

Used for `SELECT` statements that return one or more rows.

The returned `SqlDataReader` is consumed by `DatabaseRead`. Callers wrap the reader in a `using` statement so it is disposed after the result set has been processed.

## `ExecuteScalar<T>`

```csharp
int id = ExecuteScalar<int>(commandText, parameters);
```

Used when only the first column of the first returned row is needed. This is useful for generated IDs, counts, or other single-value results.

## `GetNewSqlCommand`

This private helper creates and opens the LocalDB connection and returns a `SqlCommand` associated with it.

The connection is configured through:

```text
App.config
    -> LocalDbConnection
    -> Schedule_Creator_V2 LocalDB database
```

---

# `DatabaseCreate.cs`

`DatabaseCreate` contains the application's INSERT operations.

Each public method translates application data into SQL parameters and passes those parameters to `ExecuteNonQuery` together with a SQL statement from the `Queries` resource class.

## `CreateSchedule`

Accepts a `ScheduleRow` and inserts a schedule entry.

Values written include:

- day of week
- staff ID
- schedule name
- start time
- end time

`staffID` is nullable. When no staff member has been assigned, the service sends `DBNull.Value` to SQL Server.

`TimeOnly` values are converted to `TimeSpan` before being sent to SQL Server because the database columns use the SQL `TIME` type.

## `CreateJobSettings`

Accepts a `JobSettings` record and stores the operating hours for a day of the week.

The stored values are:

- day of week
- opening time
- closing time

The day is stored using `DayOfWeek.ToString()`.

## `CreateStaff`

Creates a new staff record using:

- first name
- middle name
- last name
- position
- email
- belay certification status

`Positions` is stored as its string representation.

The current method accepts a `profilePicture` argument, but that value is not currently included in the SQL parameters sent to the database.

## `CreateDaysOff`

Accepts a staff ID, a list of dates, and a reason.

The service loops through the supplied dates and executes one insert for each date.

## `CreateAvailability`

Accepts an `Availability` record and inserts:

- staff ID
- day of week
- start time
- end time

---

# `DatabaseRead.cs`

`DatabaseRead` contains the SELECT operations for the application.

Its main responsibility is not only retrieving rows, but also converting raw SQL values into the strongly typed records and enums used by the rest of Schedule Creator V2.

Common conversions include:

```text
SQL TIME       -> TimeSpan -> TimeOnly
SQL DATE       -> DateTime -> DateOnly
NVARCHAR enum  -> Enum.Parse<T>()
SQL NULL       -> DBNull -> nullable C# value
```

## Staff Reads

### `ReadStaff()`

Returns all staff as a `List<Staff>`.

Each database row is mapped into a `Staff` record including certification dates when present.

### `ReadStaffByID(int id)`

Returns one staff member matching the supplied ID.

If no matching row exists, the current implementation returns a fallback `Staff` instance with blank values and `Positions.Unknown` rather than returning `null` or throwing an exception.

### `ReadStaffWithNoAvail()`

Returns staff members that currently have no availability records, based on `Queries.ReadStaffWithNoAvail`.

### `ReadStaffAvailOnDay(DayOfWeek dayOfTheWeek)`

Returns staff members considered available on a specified day.

### `ReadStaffNamesAndAvailOnDay(DayOfWeek day)`

Returns the smaller `StaffNameAndAvail` model used when the application needs a staff member's identity and availability window without loading the full `Staff` object.

## Availability Reads

### `ReadAvailForStaffByID(int id)`

Returns all availability records for one staff member and orders them by `dayOfTheWeek` before returning the list.

### `ReadAvailByID(int id)`

Returns availability rows matching a staff ID.

## Schedule Reads

### `ReadScheduleByScheduleName(string scheduleName)`

Returns all schedule rows with the requested schedule name.

The reader converts:

- the stored day name back into `DayOfWeek`
- SQL `TIME` values into `TimeOnly`
- a nullable `staffID` into `int?`

### `ReadAllScheduleNames()`

Returns the schedule names currently stored in the Schedule table. The schedule builder uses this to identify existing schedule names and prevent duplicate names.

## Job Settings Reads

### `ReadJobSettings()`

Returns the configured opening and closing times for each stored day.

### `ReadJobSettingsDays()`

Returns only the `DayOfWeek` values that currently have job settings. This is useful when the UI needs to determine which schedule-day columns should be enabled or displayed.

## Days-Off Reads

### `ReadDaysOff()`

Returns all stored `DaysOff` records and converts the SQL date into `DateOnly`.

---

# `DatabaseUpdate.cs`

`DatabaseUpdate` contains modifications to existing database records.

## `UpdateStaff`

Updates the main editable fields for an existing staff member:

- first name
- middle name
- last name
- position
- email

The staff ID is used to identify the row to update.

## `UpdateBelayCert`

Updates a staff member's belay certification information:

- certification status
- certified-on date
- expiration date

The date parameters are optional. Nullable `DateOnly` values are converted to `DateTime` for SQL Server; missing dates are sent as `DBNull.Value`.

---

# `DatabaseDelete.cs`

`DatabaseDelete` groups removal operations.

## `DeleteDaysOff`

Accepts a staff ID and a collection of dates, then deletes each matching day-off record individually.

## `DeleteAllAvailability`

Deletes all availability rows associated with a staff ID.

This is useful when replacing a staff member's availability with a new set of availability records.

## `DeleteJobSettingsOnDay`

Deletes the job settings for one specific day of the week.

## `DeleteAllJobSettings`

Deletes all job-setting records.

## `DeleteAllByID`

Deletes data associated with a staff ID.

The current implementation executes several DELETE statements in one command. There is also an inline TODO noting that schedule rows containing the staff ID need to be handled so removed staff members do not leave schedule data that can cause problems elsewhere in the application.

### Current implementation warning

`DeleteAllByID` currently also executes:

```sql
DELETE FROM [UWGB].[JobSettings];
```

That statement removes **all** job settings rather than data specific to the supplied staff ID. Treat this method carefully when modifying staff-deletion logic.

---

# `DataMigragtion.cs`

`DataMigragtion` handles first-run database initialization.

It is called during application startup to ensure that Schedule Creator V2 has a usable LocalDB database before database-dependent pages are used.

## Initialization Flow

```text
Application starts
      |
      v
EnsureDatabaseExists()
      |
      v
Does AppData.mdf exist?
   /         \
 yes         no
  |           |
continue      v
         CreateDatabase()
              |
              +--> Create database file
              |
              +--> Create UWGB schema
              |
              +--> Create tables
              |
              +--> Show welcome message
```

## `EnsureDatabaseExists`

This is the entry point for initialization.

If the expected database file exists, no work is performed. If it does not exist, the database creation process begins.

## Database File Location

The MDF file is created beneath the current Windows user's Local Application Data folder:

```text
%LOCALAPPDATA%\Schedule_Creator_V2\AppData.mdf
```

The directory is created automatically when necessary.

## `CreateDatabaseFile`

Creating a database requires a connection that does not depend on the application database already existing. For this reason, this method uses the `LocalDbConnectionMaster` connection string and connects to SQL Server's `master` database.

It then loads the database-creation SQL from `Queries.CreateDatabase` and replaces the database/log-file placeholders with the paths calculated at runtime.

## `CreateDatabaseSchema`

After the database file exists, this method executes:

```csharp
Queries.CreateSchemas
Queries.CreateTables
```

The current schema creates the following main tables under the `[UWGB]` schema:

| Table | Purpose |
| --- | --- |
| `UWGB.Staff` | Staff identity, role, email, and belay-certification information. |
| `UWGB.Availability` | Day/time availability ranges for staff. |
| `UWGB.DaysOff` | Individual requested/unavailable dates and reasons. |
| `UWGB.JobSettings` | Opening and closing hours for each operating day. |
| `UWGB.Schedule` | Saved schedule assignments, shift times, and schedule names. |

After first-run setup completes, the service displays the application's welcome message.

---

# Query Organization

The database services generally do not contain the complete SQL statements themselves. Instead, they reference properties such as:

```csharp
Queries.CreateSchedule
Queries.ReadStaff
Queries.UpdateStaff
Queries.DeleteDaysOff
```

These values originate from the SQL/query resources in the project. Keeping the SQL separate from the C# service methods makes it easier to inspect and modify database statements without mixing large SQL blocks into the application logic.

The main exception currently is `DatabaseDelete.DeleteAllByID`, which contains its DELETE statements inline.

---

# Data Type Conventions

Several conversions are repeated throughout this layer and should remain consistent when new database methods are added.

| Application Type | Database Representation | Conversion |
| --- | --- | --- |
| `DayOfWeek` | `NVARCHAR` or integer depending on table/query | `.ToString()` / `Enum.Parse<DayOfWeek>()` |
| `Positions` | `NVARCHAR` | `.ToString()` / `Enum.Parse<Positions>()` |
| `TimeOnly` | SQL `TIME` | `ToTimeSpan()` / `TimeOnly.FromTimeSpan()` |
| `DateOnly` | SQL `DATE` | `ToDateTime()` / `DateOnly.FromDateTime()` |
| nullable value | SQL `NULL` | `DBNull.Value` / `reader[...] is DBNull` |

When adding a new operation, prefer parameterized SQL using `SqlParameter` rather than constructing SQL by concatenating user or application values.

---

# Adding a New Database Operation

When extending this layer, use the existing separation of responsibilities:

1. Add or update the SQL statement in the appropriate query resource.
2. Put the C# method in the matching CRUD class:
   - INSERT -> `DatabaseCreate`
   - SELECT -> `DatabaseRead`
   - UPDATE -> `DatabaseUpdate`
   - DELETE -> `DatabaseDelete`
3. Build `SqlParameter` objects for dynamic values.
4. Use `ExecuteNonQuery`, `ExecuteReader`, or `ExecuteScalar<T>` from `Database`.
5. Convert database values into application types inside the database service rather than in the UI.
6. Dispose readers with `using` when processing result sets.

Example:

```csharp
public static void ExampleUpdate(
    int id,
    string value)
{
    ExecuteNonQuery(
        Queries.ExampleUpdate,
        new SqlParameter("@id", id),
        new SqlParameter("@value", value));
}
```

This keeps views and other application services independent of SQL implementation details.

---

# Current Maintenance Notes

The following behaviors reflect the current implementation and are worth remembering when this layer is changed:

- `DatabaseRead.ReadStaffByID` returns a placeholder `Staff` object when no row is found.
- `DatabaseCreate.CreateStaff` currently receives a `profilePicture` parameter but does not persist it.
- Belay certification status is currently stored as text in the database rather than a SQL `BIT` column.
- `DeleteAllByID` contains a TODO for cleaning schedule rows that reference a removed staff member.
- `DeleteAllByID` currently deletes every row from `UWGB.JobSettings`; this should be reviewed before relying on the method as a staff-only cleanup operation.
- The CRUD services use parameterized SQL for dynamic values, which should remain the standard for future operations.

These notes document the current code rather than defining permanent design requirements. Update this README when the implementation changes.
