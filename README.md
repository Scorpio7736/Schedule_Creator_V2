# Schedule Creator V2 (SCV2)

**Schedule Creator V2** is a Windows desktop application built with **C#**, **.NET 8**, **WPF**, and **SQL Server LocalDB** to simplify employee scheduling and staff administration.

Inspired by real staffing and operational challenges at a university climbing tower, SCV2 brings staff records, availability, time-off requests, schedule creation, and employee communications into a single desktop workflow.

> **Project status:** Actively developed personal software project. Some workflows and integrations may still be evolving.

## Features

### Staff management

- Add, edit, view, and remove staff records.
- Store employee information and job-related qualifications.
- Manage recurring availability and time-off records.
- View employee rosters and related information.

### Scheduling

- Configure work days and operating hours.
- Build and view named staff schedules.
- Use predefined shift templates for automatic schedule generation.
- Consider staff availability, excluded employees, and configured shift needs.
- Prioritize employees who have not yet received shifts and track assigned minutes as part of the generator's assignment logic.
- Export a schedule image for sharing.

Automatic generation is a scheduling aid, not a guarantee that every requested shift can be staffed. Results depend on available staff, the configured shift templates, and current application rules.

### Email builder

SCV2 includes an email-composition workflow for staff communications:

- Choose from email types such as schedules, staff training, belay certifications, time off, and custom messages.
- Select recipients from staff records.
- Build themed HTML messages using reusable templates and configurable sections.
- Enter formatted content through WPF rich-text controls.
- Preview and validate email content.
- Package messages as **`.eml` files**, with support for embedded images and related assets.

The email subsystem produces message files for use with email clients; do not assume that every environment is configured for direct automatic sending.

## Technology stack

| Technology | Use |
| --- | --- |
| **C#** | Application and business logic |
| **.NET 8** | Windows desktop runtime |
| **WPF / XAML** | User interface and navigation |
| **SQL Server LocalDB** | Relational storage |
| **Microsoft.Data.SqlClient** | Parameterized SQL access |
| **SQL** | Database schema and CRUD queries |
| **HTML / CSS** | Email templates and themes |
| **xUnit** | Unit and integration-oriented tests |
| **Visual Studio** | Primary development environment |

## Getting started

### Requirements

- **Windows**
- **Visual Studio** with the **.NET desktop development** workload, or the .NET 8 SDK
- **SQL Server Express LocalDB** (instance name `MSSQLLocalDB` in the current configuration)
- Access to NuGet for initial dependency restoration

### Clone and build

1. Clone the repository:

   ```powershell
   git clone https://github.com/Scorpio7736/Schedule_Creator_V2.git
   ```

2. Open `Schedule_Creator_V2.sln` in Visual Studio.
3. Confirm that the .NET 8 SDK and LocalDB are installed.
4. Review the connection strings in `Schedule_Creator_V2/App.config` for your local environment.
5. Restore packages, build the solution, and run the `Schedule_Creator_V2` project.

Or build from PowerShell:

```powershell
dotnet build Schedule_Creator_V2.sln
```

The application targets `net8.0-windows` and requires Windows for normal WPF execution.

### Database setup

The project includes SQL scripts and migration services for database creation and schema setup:

- `Queries/Migration/CreateDatabase.sql`
- `Queries/Migration/CreateSchemas.sql`
- `Queries/Migration/CreateTables.sql`

The database layer uses configured connection strings named `LocalDbConnection` and `LocalDbConnectionMaster`. A fresh installation needs a working LocalDB instance and an initialized database before features that read or write records can operate.

See the [database migration documentation](Schedule_Creator_V2/Queries/Migration/Queries_Migration_README.md) and [database service documentation](Schedule_Creator_V2/Services/Database/Services_Database_README.md) for implementation details. Use a development database when experimenting with migration or deletion routines.

## Typical workflow

1. **Configure operations:** Define operating days, hours, and job settings.
2. **Add staff:** Create employee records and maintain qualifications.
3. **Enter availability:** Record when employees can work and their days off.
4. **Build a schedule:** Assign shifts manually or generate a draft from shift templates.
5. **Review and adjust:** Check coverage, staffing conflicts, and operational requirements.
6. **Share information:** View/export schedules or prepare a themed staff email.

## Architecture

The project separates views, data models, database operations, scheduling services, and email-generation logic.

```text
Schedule_Creator_V2/
├── Schedule_Creator_V2.sln
├── Schedule_Creator_V2/
│   ├── App.xaml
│   ├── App.config
│   ├── Models/
│   │   ├── Enums/
│   │   ├── Records/
│   │   └── Objects/
│   ├── Views/
│   │   ├── Scheduling/
│   │   ├── Staff Management/
│   │   └── Insights/
│   ├── Services/
│   │   ├── Database/
│   │   ├── Scheduling/
│   │   └── Email/
│   ├── Queries/
│   │   ├── Create/
│   │   ├── Read/
│   │   ├── Update/
│   │   ├── Delete/
│   │   └── Migration/
│   └── ExtensionMethods/
└── Schedule_Creator_V2.Tests/
    └── Services/
        ├── DatabaseTests/
        └── EmailTests/
```

### Scheduling services

- **`ScheduleGeneratorService`** constructs schedule assignments using shift templates, availability, operating hours, exclusion rules, and fairness-oriented tracking.
- **`ScheduleService`** supports schedule operations.
- **`ScheduleImageExportService`** handles visual schedule export.

### Database services

- **`Database`** centralizes SQL connection and command execution.
- **`DatabaseCreate`**, **`DatabaseRead`**, **`DatabaseUpdate`**, and **`DatabaseDelete`** organize CRUD operations.
- **`DataMigragtion`** and migration scripts support database setup and changes.

The project uses SQL Server queries for staff, availability, days off, job settings, and schedules.

### Email services

The email subsystem separates its responsibilities into several layers:

| Service | Responsibility |
| --- | --- |
| `EmailTypeService` | Configures supported email types |
| `EmailInputFormService` | Builds dynamic WPF input controls |
| `EmailRecipientService` | Manages message recipients |
| `EmailValidationService` | Validates required input |
| `EmailContentService` | Builds subject and HTML content |
| `RichTextHtmlConverter` | Converts supported WPF rich-text formatting into HTML |
| `EmlEmailService` | Packages email content and assets into `.eml` output |

The project also contains themed templates and styles, including **Let's Glow** and **Tower of Power**.

See the [email service documentation](Schedule_Creator_V2/Services/Email/Services_Email_README.md).

## Automated scheduling: design considerations

The generator includes rules for:

- Matching shifts to staff availability
- Trimming shift templates to configured operating hours
- Excluding selected staff
- Handling leadership and other shift categories
- Tracking assigned minutes across the generated schedule
- Prioritizing staff who have not yet been scheduled

Scheduling is a constraint-based problem. Availability may be limited, and no single generation approach can guarantee a fully staffed, perfectly fair result for every input. Review generated schedules before using them operationally.

## Testing

The repository includes an **xUnit** test project with coverage for database and email-related behaviors.

Areas represented in the tests include:

- Database connections, migrations, and CRUD operations
- Email input, recipient, theme, and type services
- Email validation and rendering
- Rich-text-to-HTML conversion
- `.eml` message packaging

Run tests on Windows with the .NET SDK:

```powershell
dotnet test Schedule_Creator_V2.sln
```

Some database tests may require LocalDB and an appropriate test database configuration. Review test fixtures before running tests against any existing database. The presence of tests does not imply that every current test passes or that every feature is fully covered.

## Current limitations and future improvements

SCV2 remains under development. Potential improvements include:

- More flexible constraint handling and scheduling preferences
- Better conflict explanation and coverage reporting
- Expanded scheduling algorithm tests
- More robust onboarding for initial database creation
- Improved error handling and validation
- Additional accessibility and interface refinements
- Streamlined publishing and distribution for Windows users

## Why I built it

SCV2 grew out of the real-world challenge of coordinating part-time student employees, availability, scheduling restrictions, and staff communications. It is a practical application of software engineering to an operational problem: translate requirements into data models, user interfaces, scheduling logic, persistence, and reusable services.

The project showcases experience in **desktop application development, relational database design, algorithmic problem-solving, automated testing, and software architecture**.

---

Developed by **Jack London** ([@Scorpio7736](https://github.com/Scorpio7736)).
