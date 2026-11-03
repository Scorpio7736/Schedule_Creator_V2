# Let's Glow email theme

The active renderer still uses the shared Email.html section contract. Only the Let's Glow branch of EmailThemeService invokes LetsGlowThemeService and loads LetsGlowStyles.css. Default and Tower of Power rendering paths are unchanged.

## Design

- Midnight outer background, dark indigo panels, light body text.
- Cyan headings and borders, electric pink accents, lime call-to-action buttons with dark text.
- Live-text hero replaces the standard daytime header photo for Let's Glow. A user-selected header image is preserved.
- Mobile typography and padding adapt at 640px.
- Colors are transformed within HTML style/bgcolor attributes, not within message text, links, or image data.

## Outlook support

Inline colors, bgcolor fallbacks, solid-color gradients, dark-mode media rules, and Outlook data-ogsc/data-ogsb selectors are included. Conditional MSO styles provide fixed desktop container width and text-fill fallbacks for Word-based Outlook. Decorative shadows are optional; layout and readable content do not depend on them.

Actual Outlook client screenshots were not available in this session. Browser emulation verifies responsive design and preferred-color-scheme behavior, but does not reproduce Outlook's proprietary color rewriting. Do not claim identical rendering across every Outlook version based on these checks.

References:
- https://www.litmus.com/blog/the-ultimate-guide-to-dark-mode-for-email-marketers
- https://github.com/matthieuSolente/email-darkmode
- https://support.microsoft.com/en-au/outlook/mail/dark-mode-in-outlook

## Verification

- Full solution: 408 passed, 0 failed, 0 skipped.
- All seven non-Glow preset HTML outputs matched their pre-change SHA-256 hashes.
- Headless Edge with emulated light and dark color schemes at 900px and 390px.
- Mobile document width: 390px; no horizontal overflow.
- Heading color in both modes: rgb(247,248,255).
- Button text in both modes: rgb(17,16,43).
- Screenshots visually inspected at desktop and mobile widths.

Previews and results are under TestResults/GlowAfter:
- Lets_Glow_Demo.html (sample content, not a sent email)
- desktop-light.png / desktop-dark.png
- mobile-light.png / mobile-dark.png
- glow-design.trx

To regenerate preset HTML, set SCHEDULE_EMAIL_PREVIEW_DIR to a local output directory and run the LetsGlowDesignTests filter. The exporter is opt-in and sends no emails.