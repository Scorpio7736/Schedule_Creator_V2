# Email tests

Run from the solution directory:

    dotnet test Schedule_Creator_V2.Tests/Schedule_Creator_V2.Tests.csproj --filter FullyQualifiedName~Services.Email

## Added coverage

- EmailThemeServiceTests: theme selection, case-insensitive matching, default fallback, null inputs, packaged CSS insertion, repeated placeholders, missing style markers, palette conversion, non-cascading color replacements, and unrelated hex tokens.
- EmailRenderingCoverageTests: public rendering behavior across the active renderer and two legacy renderers, including HTML encoding, rich-text sanitation, malformed rich text, images, unsupported schemes, announcements, requests, signatures, footers, attachments, missing templates, and optional-section removal.
- EmailPresetCoverageTests: all ten presets through the active pipeline, template recognition, and independence of mutable preset inputs.
- EmailSectionCoverageTests: optional-section add/list/remove lifecycle, preservation of required sections, unknown section handling, and null arguments.

These complement the existing tests for recipients, staff, validation, input forms, rich text, MIME/EML formatting, content, sections, and preset definitions.

Some rendering checks are component tests: they read the packaged HTML/CSS assets. Local-image tests use unique temporary files and remove them in finally blocks. The new tests do not launch an email client or send email.

## Verified result (2026-10-15)

260 email test cases pass, with zero failures and zero skipped cases.
The original baseline had 167 cases; the expanded suite adds 93 cases.

## Regressions fixed

- LetsGlowEmailContentService removes optional sections using the table-row IDs present in the packaged template. It no longer depends on obsolete comment markers. Removal handles nested table rows and reports missing or unclosed sections.
- EmailContentService renders the supplied footer organization, website label, website URL, and footer text. Text and attributes are HTML-encoded; website URLs use the existing link normalization and validation. Default footer inputs still apply when no custom footer is supplied.

The original 11 failing cases now pass. Additional checks cover removal of optional Let's Glow sections and custom footer text/link encoding.

Run the command above for current results. Earlier files under TestResults/EmailFinal describe the pre-fix run and are historical, not the current result.

This suite covers email services; it is not full-application coverage.