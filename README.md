# Schedule Creator V2

Schedule Creator V2 is a WPF application for creating, publishing, and maintaining recurring staff schedules. It keeps track of employees, their availability, required certifications, and assigned shifts so supervisors can build balanced schedules with minimal manual work.

## Features
- **Staff management**: Add, edit, or remove staff members, including required belay certifications and job-specific settings.
- **Availability tracking**: Capture weekly availability and one-off days off, then surface the data when building schedules.
- **Schedule builder**: Assemble weekly shift grids, prevent duplicate schedule names, and save assignments for any day with available staff.
- **Views and exports**: Review saved schedules, view days-off lists, and generate staff email lists for communication.
- **Local database**: Uses SQL Server LocalDB; the database file and schema are created automatically on first launch.

## Getting Started
1. **Requirements**
   - Windows with .NET 8.0 SDK installed.
   - SQL Server LocalDB (installed with recent versions of Visual Studio).
2. **Run the app**
   - Open `Schedule_Creator_V2.sln` in Visual Studio 2022 or later.
   - Restore NuGet packages and build the solution.
   - Launch the `Schedule_Creator_V2` project. On first run, the app will provision the LocalDB database under your local application data folder.

## Project Notes
- The app uses the `Microsoft.Data.SqlClient` provider and `Queries.resx` to manage SQL schema creation and CRUD operations.
- Microsoft Graph client libraries are referenced for future email delivery features alongside the generated email lists.
- UI pages live under the `Schedule_Creator_V2` project root (e.g., `Build_Schedule.xaml`, `View_Schedule.xaml`, `Add_Staff.xaml`) with accompanying code-behind files.
