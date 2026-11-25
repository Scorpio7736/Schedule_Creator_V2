# Schedule Creator V2 — Master Development Checklist

> Living source of truth for development priorities, planned updates, implementation progress, and release readiness.

**Branch:** `master`  
**Platform:** C# / .NET 8 / WPF / SQL Server LocalDB  
**Last updated:** November 24, 2026  
**Current focus:** Stabilization and planning the next update.

## 1. Development workflow

- [ ] Record and prioritize the requirements for the next update.
- [ ] Define acceptance criteria before implementation.
- [ ] Implement changes and associated regression tests.
- [ ] Build and test the solution on Windows.
- [ ] Review affected UI and operational workflows.
- [ ] Update documentation and this checklist.
- [ ] Record the commit and verification results.

**Status convention:** Unchecked means pending; checked means completed at the stated scope. Implementation and verification are tracked separately. Proposed backlog items are not approved scope until moved into a planned update.

## 2. Current stabilization — November 24, 2026

### Completed implementation

- [x] EMAIL-001 — Add Tower Ball Score Update email preset.
- [x] EMAIL-002 — Update email preset count and expected ordering to match all 14 templates.
- [x] EMAIL-003 — Reject malformed HTTP/HTTPS image sources in all three email renderers.
- [x] EMAIL-004 — Reconnect the Let's Glow theme service to the active HTML rendering pipeline.
- [x] EMAIL-005 — Restore Let's Glow dark-mode rules, Outlook fallbacks, and responsive styling.
- [x] TEST-001 — Add six malformed-image-URL regression test cases and update palette assertions.
- [x] DEV-001 — Establish this master development checklist.

Implementation reference: [Email regression fixes (e7be936)](https://github.com/Scorpio7736/Schedule_Creator_V2/commit/e7be936d1a44e26fcaed17d9be7b8009922d5bc0).

### Pending verification — next priority

- [ ] QA-001 — Run the complete Windows test suite and confirm the eight previously reported failures are resolved.
- [ ] QA-002 — Record total, passed, failed, and skipped test counts.
- [ ] QA-003 — Preview all 14 email presets and check for unintended rendering regressions.
- [ ] QA-004 — Verify Let's Glow Schedule, Announcements, and Requests in Outlook light and dark modes; check button contrast, custom header images, and responsive layout.
- [ ] QA-005 — Verify Tower Ball staff selection, free-text scores, and header rendering in the generated email and exported message.
- [ ] QA-006 — Confirm ordinary HTTP/HTTPS URLs, scheme-less URLs, supported local image files, and embedded images still render.

**Verification status:** Source changes committed; full Windows test execution and Outlook visual verification remain pending.

## 3. Next update — planning

**Version:** TBD  
**Goal:** TBD  
**Status:** Awaiting requirements

### Approved feature scope

_No new update requirements have been entered yet._

<!-- Add approved tasks as Markdown checkboxes with a unique ID. -->

### Acceptance criteria

- [ ] Define acceptance criteria for each approved feature.
- [ ] Identify affected services, views, database queries, and tests.
- [ ] Identify dependencies and migration requirements, if applicable.

### Implementation checklist

_Add implementation tasks after scope is approved._

## 4. Master backlog

These are proposed planning categories and improvement candidates, not commitments. Select and move items into an approved update before implementation.

### Scheduling

- [ ] SCHED-001 — Evaluate more flexible shift constraints and staff preferences.
- [ ] SCHED-002 — Improve staffing conflict explanations and uncovered-shift reporting.
- [ ] SCHED-003 — Expand automated scheduling tests for availability, days off, leadership coverage, and fairness.

### Staff management and availability

_No specific new requirements entered._

### Email and communications

- [ ] EMAIL-006 — Evaluate consolidating duplicated image-source validation across renderers.

### Database and persistence

- [ ] DB-001 — Improve first-run database setup and migration onboarding.
- [ ] DB-002 — Review error handling and validation for database operations.

### UI / UX

- [ ] UI-001 — Review accessibility, navigation consistency, and interface refinements.

### Testing and code quality

- [ ] TEST-002 — Evaluate Windows-based continuous integration for build and test execution.

### Distribution and deployment

- [ ] REL-001 — Evaluate a streamlined Windows publishing and installation workflow.

## 5. Release readiness checklist

Use for each approved update.

- [ ] All approved requirements implemented.
- [ ] Clean solution build succeeds.
- [ ] Complete test suite passes on Windows.
- [ ] Relevant manual UI checks completed.
- [ ] Database migrations tested against a development database, if applicable.
- [ ] No credentials or private staff data introduced into the repository.
- [ ] Existing scheduling and email workflows remain functional.
- [ ] README and service documentation updated where applicable.
- [ ] Version and commit references recorded.

### Verification commands

```powershell
dotnet restore Schedule_Creator_V2.sln
dotnet build Schedule_Creator_V2.sln
dotnet test Schedule_Creator_V2.sln
```

Use a development/test database for database-related testing.

## 6. Development history

| Date | Update | Implementation | Verification |
| --- | --- | --- | --- |
| 2026-11-24 | Tower Ball Score Update email | Added | Pending manual verification |
| 2026-11-24 | Email test and Let's Glow regression fixes | Committed (e7be936) | Pending Windows test run |
| 2026-11-24 | Master development checklist | Created | Not applicable |

## 7. New update template

Copy this section when planning an additional update.

```markdown
## Update — [Name / Version]

Goal:
Priority:
Dependencies:

### Requirements
- [ ] [ID] — [Requirement]

### Implementation
- [ ] [Task]

### Acceptance criteria
- [ ] [Expected behavior]

### Verification
- [ ] Build succeeds.
- [ ] Tests pass.
- [ ] Manual checks complete.

Commit:
Verification results:
```
