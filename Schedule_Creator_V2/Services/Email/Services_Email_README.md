# Email Services

This folder contains the services that power Schedule Creator V2's email-building workflow.

Together, these files handle:

- defining available email types
- building the email-input UI
- managing staff recipients
- validating email inputs
- supporting optional email sections
- converting WPF rich text into email-safe HTML
- rendering the final HTML email
- packaging the rendered message into an `.eml` file

The email subsystem is designed so that the WPF views primarily coordinate user interaction while the services handle email-specific behavior.

> **Repository note:** This documentation describes the current email-service implementations. If files are moved between folders, keep their namespaces/usings synchronized with the new location.

---

## Folder Overview

| File | Responsibility |
| --- | --- |
| `EmailTypeService.cs` | Defines the supported email types and the input sections each type uses. |
| `EmailInputFormService.cs` | Dynamically builds WPF controls for an email type and writes user-entered values back into the input models. |
| `EmailContentService.cs` | Converts email inputs into the final subject and sanitized HTML body. |
| `EmailRecipientService.cs` | Manages the selected recipient collection displayed in the email UI. |
| `EmailStaffService.cs` | Supplies staff data used by the email-recipient workflow. |
| `EmailValidationService.cs` | Validates email type selection, recipients, and required input controls. |
| `EmlEmailService.cs` | Packages the rendered message into MIME/`.eml` output and handles embedded email assets. |
| `RichTextHtmlConverter.cs` | Converts WPF `FlowDocument` content into a limited email-safe HTML representation. |
| `emailSectionOption.cs` | Describes optional sections that can be added to or removed from custom emails. |
| `EmailBuilder.cs` | Large email-related source file retained as part of the subsystem; review its role before modifying or consolidating the active email pipeline. |

The subsystem also depends on files outside this folder, including:

```text
Models/
├── Constants/
├── Defaults/
├── Interfaces/
├── Objects/
│   └── Email.html
└── Records/

Images/
└── EmailImages/

Views/
└── Controls/
    └── EmailRichTextEditor.*
```

---

# High-Level Email Flow

The current email workflow can be viewed as a pipeline:

```text
EmailTypeService
      |
      | selects email definition
      v
EmailType + IEmailInputs records
      |
      v
EmailInputFormService
      |
      | dynamically builds WPF controls
      | captures user-entered values
      v
EmailValidationService
      |
      | validates required UI state
      v
EmailRecipientService
EmailStaffService
      |
      | supplies recipient data
      v
EmailContentService
      |
      | BuildSubject()
      | BuildHtmlBody()
      | sanitize rich text
      | inject sections into Email.html
      v
Rendered HTML Email
      |
      v
EmlEmailService
      |
      | MIME packaging
      | embedded images/assets
      v
.eml file
```

This separation keeps email-definition logic, UI generation, rendering, and export responsibilities from being concentrated in a single class.

---

# `EmailTypeService.cs`

## Purpose

`EmailTypeService` defines the email templates available to the application.

Its main entry point is:

```csharp
GetEmailTypes()
```

which constructs and returns the supported `EmailType` definitions.

Current email types include:

```text
Custom
Staff Training
Schedule
Belay Certification
Time Off
```

Each `EmailType` contains a collection of input records implementing:

```csharp
IEmailInputs
```

Those records determine which input sections appear in the UI and which values are later available to `EmailContentService`.

---

## Input Groups

Depending on the email type, the service can configure records such as:

```text
EmailDetailsInputs
CustomHeaderInputs
CustomBodyInputs
CustomImageInputs
CustomAnnouncementsInputs
CustomRequestInputs
CustomAttachmentsInputs
CustomSignatureInputs
CustomFooterInputs
```

Conceptually:

```text
EmailType
   |
   +--> EmailDetailsInputs
   |
   +--> CustomHeaderInputs
   |
   +--> CustomBodyInputs
   |
   +--> optional section
   |
   +--> CustomSignatureInputs
   |
   +--> CustomFooterInputs
```

The UI does not need to know how each email type was assembled. It receives the configured `EmailType` and lets `EmailInputFormService` inspect its input groups.

---

## Why This Service Exists

Without a central email-type service, each view would need to manually determine:

- which fields belong to a Schedule email
- which fields belong to a Training email
- which default values should be used
- which sections can be customized
- which input record types need to be created

`EmailTypeService` centralizes that configuration.

---

# `EmailInputFormService.cs`

## Purpose

`EmailInputFormService` converts an `EmailType` definition into actual WPF input controls.

It uses reflection to inspect each object in:

```csharp
emailType.inputs
```

and creates controls based on the property type and property name.

The overall flow is:

```text
EmailType.inputs
      |
      v
IEmailInputs record
      |
      v
Inspect public properties
      |
      +--> string
      |      |
      |      +--> TextBox
      |      |
      |      +--> RichTextBox
      |
      +--> List<string>
             |
             v
         multiline TextBox
```

---

## `BuildEmailInputControls`

The primary UI-building method is:

```csharp
BuildEmailInputControls(
    StackPanel container,
    EmailType emailType)
```

The method:

1. Clears the existing input controls.
2. Iterates through the email type's input groups.
3. Creates a bordered section for each input group.
4. Reflects over its public properties.
5. Adds the correct WPF control for each supported property.

This means adding an input property to a supported record can automatically expose that value to the dynamic email form without requiring a separate hard-coded control in the email page.

---

## Applying Input Values

After the user edits the controls, the service uses:

```csharp
ApplyInputValues(
    StackPanel container)
```

to walk through the generated UI and write current values back into their original input records.

Controls carry metadata in their:

```csharp
Tag
```

so the service knows:

```text
which input record owns this value
which property should be updated
```

The flow is:

```text
WPF control
    |
    | Tag
    v
EmailInputControlInfo
    |
    +--> InputGroup
    +--> PropertyInfo
            |
            v
      Property.SetValue(...)
```

---

# Plain-Text Inputs

Normal string properties become WPF:

```csharp
TextBox
```

The service determines whether the field should be single-line or multiline based on the property's intended use.

Typical long-form properties receive:

```text
AcceptsReturn = true
TextWrapping = Wrap
VerticalScrollBarVisibility = Auto
```

while short values use a standard single-line control.

---

# List Inputs

Properties of type:

```csharp
List<string>
```

are represented by one multiline `TextBox`.

Each line becomes one list item.

For example:

```text
Schedule image
Availability form
Training document
```

becomes:

```csharp
new List<string>
{
    "Schedule image",
    "Availability form",
    "Training document"
}
```

When values are applied, empty lines are removed and remaining entries are trimmed.

---

# Rich-Text Inputs

The current form service treats the following properties as rich text:

```text
EmailBody
RequestBody
AnnouncementsIntro
AttachmentsIntro
HeaderSubtitle
SignatureClosing
```

Instead of a normal `TextBox`, these properties receive a WPF:

```csharp
RichTextBox
```

with a formatting toolbar.

Current formatting options include:

```text
Bold
Italic
Underline
Bulleted List
Numbered List
```

The toolbar uses standard WPF editing commands such as:

```csharp
EditingCommands.ToggleBold
EditingCommands.ToggleItalic
EditingCommands.ToggleUnderline
EditingCommands.ToggleBullets
EditingCommands.ToggleNumbering
```

---

## Rich-Text Storage Marker

Formatted content is stored internally with the prefix:

```text
[[RICH_TEXT_HTML]]
```

Conceptually:

```text
WPF FlowDocument
      |
      v
HTML serialization
      |
      v
[[RICH_TEXT_HTML]]<p>...</p>
```

The prefix allows `EmailContentService` to distinguish formatted email content from ordinary text.

---

# `RichTextHtmlConverter.cs`

## Purpose

`RichTextHtmlConverter` provides a reusable conversion layer between WPF document content and limited HTML.

The supported output includes:

```html
<p>
<strong>
<em>
<u>
<ul>
<ol>
<li>
<br />
```

It intentionally supports a small formatting subset rather than arbitrary HTML.

---

## `ToHtml`

```csharp
RichTextHtmlConverter.ToHtml(
    FlowDocument document)
```

walks through the document's blocks and converts supported WPF elements into HTML.

Supported WPF structures include:

```text
Paragraph
Section
List
ListItem
Run
Span
LineBreak
```

Text from `Run` elements is HTML encoded before it is inserted into the generated markup.

---

## Formatting Conversion

Examples:

```text
WPF Bold
    -> <strong>

WPF Italic
    -> <em>

WPF Underline
    -> <u>

Bulleted List
    -> <ul>

Numbered List
    -> <ol>

LineBreak
    -> <br />
```

This allows the email renderer to receive structured content without allowing unrestricted HTML from the editor.

---

## `SetPlainText`

```csharp
SetPlainText(
    FlowDocument document,
    string? text)
```

clears a document and loads ordinary text into WPF paragraphs and line breaks.

It normalizes line endings and converts blank-line-separated content into multiple paragraphs.

---

# Rich-Text Architecture Note

There are currently **two rich-text conversion implementations** in the email subsystem.

## Implementation 1

`EmailInputFormService` dynamically creates its own raw `RichTextBox` and contains its own HTML serialization/parsing logic.

## Implementation 2

The reusable:

```text
EmailRichTextEditor
        |
        v
RichTextHtmlConverter
```

provides another rich-text conversion path.

Conceptually:

```text
Current Path A
EmailInputFormService
    -> RichTextBox
    -> internal serializer/parser


Current Path B
EmailRichTextEditor
    -> RichTextHtmlConverter
```

These two systems currently overlap in responsibility.

When this area is refactored, consolidating rich-text editing and conversion into a single implementation would reduce the chance that the two serializers behave differently.

---

## Current `<br>` Difference

One important current difference is:

```text
EmailInputFormService serializer
    -> <br>

RichTextHtmlConverter
    -> <br />
```

`EmailContentService` sanitizes rich HTML using `XElement.Parse(...)`, which expects XML-compatible markup.

Therefore, the self-closing form:

```html
<br />
```

is safer for that parser than a raw HTML:

```html
<br>
```

If malformed rich HTML reaches the content service, its sanitizer intentionally falls back to displaying the content safely as encoded plain text.

---

# `EmailContentService.cs`

## Purpose

`EmailContentService` is the central email-rendering layer.

Its two major outputs are:

```csharp
BuildSubject(
    EmailType emailType)

BuildHtmlBody(
    EmailType emailType)
```

The service receives populated email input records and converts them into the final subject and HTML message.

---

# HTML Template

The service loads:

```text
Models/
└── Objects/
    └── Email.html
```

at runtime.

The template contains section markers such as:

```html
<!-- HEADER -->
<!-- BODY -->
<!-- IMAGE -->
<!-- ANNOUNCEMENTS -->
<!-- REQUEST -->
<!-- ATTACHMENTS -->
<!-- SIGNATURE -->
<!-- FOOTER -->
```

`EmailContentService` replaces each marker with rendered HTML.

The flow is:

```text
Email.html
    |
    +--> HEADER marker
    +--> BODY marker
    +--> IMAGE marker
    +--> ANNOUNCEMENTS marker
    +--> REQUEST marker
    +--> ATTACHMENTS marker
    +--> SIGNATURE marker
    +--> FOOTER marker
             |
             v
       Final HTML body
```

If a required marker is missing, the service throws an exception rather than silently producing an incomplete email.

---

# Email Sections

The service contains builders for the major email sections.

These include:

```text
Header
Body
Image
Announcements
Request
Attachments
Signature
Footer
```

Each section retrieves the appropriate input record from:

```csharp
emailType.inputs
```

and then builds the corresponding email-safe HTML.

---

# Rich-Text Rendering and Sanitization

Rich-text values are passed through:

```csharp
RenderRichText(...)
```

The service supports both:

```text
[[RICH_TEXT_HTML]] prefixed content
```

and recognizable supported raw rich-text HTML.

Recognized tags include:

```html
<p>
<strong>
<b>
<em>
<i>
<u>
<ul>
<ol>
<li>
<br>
```

---

## Sanitization Strategy

Rich HTML is parsed using:

```csharp
XElement.Parse(...)
```

and reconstructed using only supported elements.

Unknown elements are stripped while safe child content is retained.

Input attributes are not blindly copied into the rendered output.

This gives the service a whitelist-style rendering approach:

```text
incoming rich HTML
       |
       v
parse
       |
       v
supported tag?
   /        \
 yes         no
  |           |
  v           v
rebuild     strip tag,
safe tag    preserve safe content
```

---

## Malformed Rich Text

If the rich-text HTML cannot be parsed, the service does not inject it directly.

Instead, it falls back to:

```text
safe encoded plain text
```

This prevents malformed or unexpected markup from being inserted into the final email.

---

# Plain-Text Encoding

Normal non-rich values are HTML encoded.

Depending on the field, the service uses either:

```text
encoded text + <br> line breaks
```

or:

```text
encoded paragraphs
```

This allows normal user text to preserve basic formatting without being interpreted as arbitrary HTML.

---

# Image Handling

`EmailContentService` supports an optional image section.

An image source can be resolved from supported inputs such as:

```text
data:image/... URI
local image file
configured email image source
```

Local image content can be converted into a data URI for rendering.

The service also HTML-encodes image attributes such as:

```text
src
alt
```

before inserting them into markup.

---

# Header Rendering

The header can use values from:

```csharp
CustomHeaderInputs
```

including fields such as:

```text
OrganizationName
HeaderLabel
EmailHeading
HeaderSubtitle
HeaderImageUrl
```

`HeaderSubtitle` supports rich-text rendering.

The current template also contains UWGB Climbing Tower-specific branding and default imagery.

---

# Body Rendering

The body primarily uses:

```text
RecipientGreeting
EmailBody
```

`RecipientGreeting` is encoded as ordinary text with line breaks.

`EmailBody` is rendered through the rich-text pipeline and uses paragraph formatting for ordinary plain-text input.

---

# Signature Rendering

The signature can include values such as:

```text
SignatureClosing
SenderName
SenderTitle
SenderOrganization
SenderEmail
SenderPhone
```

`SignatureClosing` supports rich text.

Email and phone elements are rendered conditionally when values exist.

---

# Footer Rendering

The service retrieves:

```csharp
CustomFooterInputs
```

or falls back to:

```csharp
EmailInputDefaults.DefaultFooterInputs
```

However, the current rendered footer still contains several hard-coded UWGB values, including organization text, the climbing website link, and the affiliation message.

This means modifying `CustomFooterInputs` does not necessarily change every visible footer value yet.

---

# `EmailRecipientService.cs`

## Purpose

`EmailRecipientService` manages the collection of staff members selected to receive an email.

It works with the recipient `DataGrid` used by the email UI.

Typical operations include:

```text
Add recipient
Remove selected recipient
Clear recipients
Read current recipients
```

---

## Duplicate Prevention

When a staff member is added, the service checks the existing recipient collection so the same staff ID is not added repeatedly.

Conceptually:

```text
Add Staff ID 5
      |
      v
Already in recipient collection?
      |
   +--+--+
   |     |
  yes    no
   |     |
ignore   add
```

This keeps recipient lists clean even when the same person is selected more than once in the UI.

---

# `EmailStaffService.cs`

## Purpose

`EmailStaffService` provides staff data for the email workflow.

It acts as a small bridge between the email subsystem and the database layer so email views do not need to perform their own direct staff queries.

Conceptually:

```text
Email UI
   |
   v
EmailStaffService
   |
   v
DatabaseRead
   |
   v
UWGB.Staff
```

This service can remain intentionally small because staff persistence belongs to the database layer rather than the email subsystem.

---

# `EmailValidationService.cs`

## Purpose

`EmailValidationService` validates the email composition UI before output is generated.

The current validation flow checks conditions such as:

```text
An email type has been selected
Recipients have been selected
Required text inputs contain values
```

This prevents the application from generating obviously incomplete messages.

---

## Control Discovery

The current implementation searches through the visual tree for:

```csharp
TextBox
```

controls and inspects their metadata to determine whether required values are present.

---

## Current Rich-Text Validation Limitation

Rich-text fields are created as:

```csharp
RichTextBox
```

rather than:

```csharp
TextBox
```

The current validation traversal only gathers normal `TextBox` controls.

Therefore, required rich-text properties are not currently included in the same required-field validation pass.

If a rich-text field must be mandatory, the validation service should eventually be extended to inspect both:

```text
TextBox
RichTextBox
```

or validate the underlying input records rather than relying only on WPF control types.

---

# `emailSectionOption.cs`

## Purpose

`emailSectionOption` supports the optional-section system used by custom emails.

It associates an optional email section with the input-group type that should be added or removed.

This allows the Custom email configuration to expose optional sections without hard-coding all of the behavior directly into the view.

Conceptually:

```text
Optional section selected
        |
        v
emailSectionOption
        |
        v
associated IEmailInputs type
        |
        v
add input group to EmailType
        |
        v
EmailInputFormService builds controls
        |
        v
EmailContentService renders section
```

Examples of optional content include areas such as:

```text
Image
Announcements
Request
Attachments
```

depending on the current email configuration.

---

# `EmlEmailService.cs`

## Purpose

`EmlEmailService` is the final output layer of the email pipeline.

By the time this service is called, `EmailContentService` has already produced:

```text
subject
HTML body
```

`EmlEmailService` packages that content into an email message that can be saved/opened as an:

```text
.eml
```

file.

---

## MIME Packaging

HTML email is more than a plain text file.

The service is responsible for building the MIME structure required for content such as:

```text
HTML body
embedded images
related resources
attachments/content parts
email headers
transfer encodings
```

Conceptually:

```text
Rendered HTML
      |
      +--> MIME headers
      |
      +--> HTML body
      |
      +--> inline resources
      |
      +--> related content
              |
              v
           .eml
```

This keeps low-level MIME formatting out of the UI and out of `EmailContentService`.

---

## Inline Images

The email renderer can produce image sources that later need to be represented correctly inside the `.eml` message.

`EmlEmailService` handles the packaging required for embedded/inline email assets so the final message can reference them using MIME content identifiers where appropriate.

---

## Output Responsibility

The intended separation is:

```text
EmailContentService
    -> "What should the email HTML contain?"

EmlEmailService
    -> "How should that email be packaged as an email file?"
```

Keeping those concerns separate makes it possible to improve HTML design without rewriting the MIME export layer.

---

# `EmailBuilder.cs`

`EmailBuilder.cs` is currently an unusually large file compared with the other services in this subsystem.

The active email workflow is already divided across:

```text
EmailTypeService
EmailInputFormService
EmailContentService
EmlEmailService
```

so this file should be treated carefully when the email architecture is refactored.

Before removing, replacing, or consolidating it:

1. Check for project references to its public members.
2. Determine whether any generated/static content is stored in it.
3. Verify that the current email preview/export path does not depend on it.
4. Avoid duplicating functionality already owned by the smaller email services.

If its responsibilities are eventually absorbed into the newer service pipeline, this README should be updated to reflect that change.

---

# `EmailRichTextEditor` Dependency

A reusable rich-text control currently exists outside this service folder:

```text
Views/
└── Controls/
    ├── EmailRichTextEditor.xaml
    └── EmailRichTextEditor.xaml.cs
```

Its `Html` getter currently returns:

```text
[[RICH_TEXT_HTML]]
+
RichTextHtmlConverter.ToHtml(...)
```

This matches the marker recognized by `EmailContentService`.

However, its current setter uses:

```csharp
RichTextHtmlConverter.SetPlainText(...)
```

so assigning previously generated HTML to `Html` does not currently reconstruct that HTML as formatted WPF content.

That behavior should be considered if this control becomes the sole editor used by `EmailInputFormService`.

---

# Models Used by the Email Services

The service folder depends heavily on records under:

```text
Models/Records
```

These records serve as the data-transfer layer between:

```text
EmailTypeService
EmailInputFormService
EmailContentService
```

Conceptually:

```text
EmailTypeService
    creates input records
          |
          v
EmailInputFormService
    modifies record properties
          |
          v
EmailContentService
    reads record properties
```

This is an important design boundary.

The WPF controls are temporary UI representations; the input records are the actual email data passed between the services.

---

# Constants and Defaults

Email configuration also relies on:

```text
Models/Constants
Models/Defaults
```

Examples include:

```text
EmailImageSources
EmailInputConstants
EmailInputDefaults
```

These centralize reusable values such as:

```text
default input content
default footer configuration
image sources
email labels/settings
```

Prefer updating shared defaults there rather than duplicating literal values across multiple email services.

---

# Email Template Dependency

The final HTML structure is stored in:

```text
Models/Objects/Email.html
```

The project file copies this template to the build output:

```xml
<None Update="Models\Objects\Email.html">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</None>
```

`EmailContentService` then loads the template using the application's output directory.

Therefore, deleting or renaming `Email.html` without updating the project and service code will cause email generation to fail.

---

# Email Image Dependency

Files beneath:

```text
Images/EmailImages/
```

are configured as content and copied to the application's output directory.

These assets support the branded email template and image rendering logic.

When adding a packaged email image, verify that:

```text
the file is included in the project
the output-copy behavior is correct
the configured image source matches its deployed location
```

---

# Adding a New Email Type

To add a new predefined email type, follow the existing pipeline.

## 1. Define the Input Requirements

Determine which records the new email needs.

For example:

```text
EmailDetailsInputs
CustomHeaderInputs
CustomBodyInputs
CustomSignatureInputs
CustomFooterInputs
```

Add a new record only if an existing input group cannot represent the required data.

---

## 2. Add the Type to `EmailTypeService`

Create the new `EmailType` and populate its:

```csharp
inputs
```

collection.

Use shared defaults where appropriate.

---

## 3. Verify Dynamic Form Support

If the new properties are:

```text
string
List<string>
```

the current `EmailInputFormService` can generally construct controls automatically.

If a new property type is introduced, the form service will need a matching control-generation branch.

---

## 4. Add Rendering Logic

If the new email uses only existing sections, `EmailContentService` may not require any changes.

If a completely new section is required:

1. Add the input record.
2. Add a template marker.
3. Add a section builder.
4. Replace the marker in `BuildHtmlBody`.
5. Decide whether the section is required or optional.

---

## 5. Validate It

Update `EmailValidationService` if the new type introduces required fields that are not covered by the existing validation logic.

---

# Adding a New Email Section

A new HTML section typically touches several layers:

```text
Models/Records
      |
      v
new IEmailInputs record
      |
      v
EmailTypeService
      |
      v
EmailInputFormService
      |
      v
Email.html marker
      |
      v
EmailContentService section builder
```

If the section should be optional for Custom emails, also integrate it with:

```text
emailSectionOption
```

---

# Adding a New Rich-Text Property

The current dynamic form only uses a rich editor for property names listed in:

```csharp
RichTextPropertyNames
```

Therefore, adding a new rich-text field currently requires:

1. Add the property to the appropriate input record.
2. Add its name to `RichTextPropertyNames`.
3. Render it with `RenderRichText(...)` in `EmailContentService`.
4. Ensure validation handles it if it is required.
5. Verify round-trip editor loading if the value can be edited after initial entry.

---

# Email Safety Principles

The current architecture uses several safeguards that should be preserved.

## Parameter/Data Separation

Email records hold user data rather than mixing that data directly into arbitrary HTML.

## HTML Encoding

Normal text values are encoded before being inserted into the email.

## Rich-Text Whitelist

Only a limited set of formatting tags is reconstructed by the rich-text sanitizer.

## Attribute Encoding

Dynamic attributes such as image sources and email addresses are encoded before use in HTML attributes.

## Malformed HTML Fallback

If rich text cannot be parsed safely, it is rendered as encoded text rather than inserted as untrusted markup.

These behaviors should remain intact when the editor or renderer is refactored.

---

# Recommended Responsibility Boundaries

The intended service boundaries are:

| Responsibility | Owner |
| --- | --- |
| Which email types exist? | `EmailTypeService` |
| Which input groups belong to a type? | `EmailTypeService` |
| How are input controls created? | `EmailInputFormService` |
| How are recipients managed? | `EmailRecipientService` |
| Where do recipient staff records come from? | `EmailStaffService` |
| Is the composition valid? | `EmailValidationService` |
| How does WPF rich text become safe HTML? | `RichTextHtmlConverter` / current form serializer |
| How are input records rendered into HTML? | `EmailContentService` |
| How is HTML packaged as an email file? | `EmlEmailService` |
| Which optional custom sections are available? | `emailSectionOption` + `EmailTypeService` |

Maintaining these boundaries helps prevent the email UI from accumulating rendering, MIME, database, and validation logic.

---

# Current Maintenance Notes

The following items describe the current implementation and are worth checking when this subsystem changes:

- `EmailTypeService` currently defines Custom, Staff Training, Schedule, Belay Certification, and Time Off email configurations.
- `EmailInputFormService` builds controls dynamically using reflection over `IEmailInputs` records.
- Rich-text properties currently include `EmailBody`, `RequestBody`, `AnnouncementsIntro`, `AttachmentsIntro`, `HeaderSubtitle`, and `SignatureClosing`.
- The rich-text marker is currently `[[RICH_TEXT_HTML]]`.
- `EmailContentService` recognizes and sanitizes a limited rich-text HTML tag set.
- `EmailContentService` uses `XElement.Parse(...)` for rich-text sanitization.
- `EmailInputFormService` currently serializes line breaks as `<br>`, while `RichTextHtmlConverter` emits `<br />`.
- The project currently contains two overlapping rich-text conversion/editor paths.
- `EmailRichTextEditor.Html` can export formatted HTML, but its setter currently loads the supplied value as plain text.
- `EmailValidationService` currently validates normal `TextBox` controls but does not include rich-text `RichTextBox` fields in the same required-field pass.
- `EmailContentService` currently contains some hard-coded UWGB footer content even though `CustomFooterInputs` is retrieved.
- `EmailRecipientService` prevents duplicate recipients by staff ID.
- `EmlEmailService` is responsible for MIME/`.eml` packaging rather than HTML layout.
- `Email.html` must remain available in the application output directory.
- Email image assets beneath `Images/EmailImages` are copied to output by the project configuration.
- `EmailBuilder.cs` is substantially larger than the other email services and should be reviewed carefully before architecture changes are made around it.

Update this README whenever email files are moved, email types or sections are added, rich-text behavior changes, the HTML template changes, or the final email-output pipeline is refactored.
