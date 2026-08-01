"use strict";

/*
 * UWGB Climbing Tower Email Builder
 *
 * This file controls the editable HTML email template.
 *
 * IMPORTANT:
 * JavaScript does not run inside most email clients.
 * Use these functions while building or previewing the email, then call
 * getFinalEmailHtml() to generate the completed HTML before sending it.
 */

const EMAIL_SECTION_IDS = Object.freeze({
    header: "headerSection",
    body: "bodySection",
    request: "requestSection",
    attachments: "attachmentsSection",
    signature: "signatureSection",
    footer: "footerSection"
});


/* =========================================================
   BASIC ELEMENT FUNCTIONS
   ========================================================= */

/**
 * Finds an HTML element by ID.
 *
 * @param {string} htmlId
 * @returns {HTMLElement}
 */
function getEmailElement(htmlId) {
    if (typeof htmlId !== "string" || htmlId.trim() === "") {
        throw new Error("A valid HTML element ID is required.");
    }

    const element = document.getElementById(htmlId);

    if (!element) {
        throw new Error(`No HTML element was found with ID "${htmlId}".`);
    }

    return element;
}


/**
 * Replaces the visible text inside an element.
 *
 * This safely inserts plain text and does not interpret HTML tags.
 *
 * @param {string} htmlId
 * @param {string} textToFill
 */
function fillItemById(htmlId, textToFill) {
    const element = getEmailElement(htmlId);
    element.textContent = textToFill ?? "";
}


/**
 * Replaces the contents of an element with HTML.
 *
 * Only use this function with trusted HTML.
 *
 * @param {string} htmlId
 * @param {string} htmlToFill
 */
function fillHtmlById(htmlId, htmlToFill) {
    const element = getEmailElement(htmlId);
    element.innerHTML = htmlToFill ?? "";
}


/**
 * Updates an attribute on an HTML element.
 *
 * @param {string} htmlId
 * @param {string} attributeName
 * @param {string} attributeValue
 */
function setAttributeById(htmlId, attributeName, attributeValue) {
    const element = getEmailElement(htmlId);

    if (
        typeof attributeName !== "string" ||
        attributeName.trim() === ""
    ) {
        throw new Error("A valid attribute name is required.");
    }

    element.setAttribute(attributeName, attributeValue ?? "");
}


/**
 * Removes an attribute from an HTML element.
 *
 * @param {string} htmlId
 * @param {string} attributeName
 */
function removeAttributeById(htmlId, attributeName) {
    const element = getEmailElement(htmlId);
    element.removeAttribute(attributeName);
}


/* =========================================================
   LINK FUNCTIONS
   ========================================================= */

/**
 * Sets the URL for a link.
 *
 * @param {string} htmlId
 * @param {string} url
 */
function setLinkById(htmlId, url) {
    const element = getEmailElement(htmlId);

    if (!(element instanceof HTMLAnchorElement)) {
        throw new Error(
            `Element "${htmlId}" is not an anchor element.`
        );
    }

    element.href = url ?? "";
}


/**
 * Sets both the visible text and URL for a link.
 *
 * @param {string} htmlId
 * @param {string} linkText
 * @param {string} url
 */
function fillLinkById(htmlId, linkText, url) {
    const element = getEmailElement(htmlId);

    if (!(element instanceof HTMLAnchorElement)) {
        throw new Error(
            `Element "${htmlId}" is not an anchor element.`
        );
    }

    element.textContent = linkText ?? "";
    element.href = url ?? "";
}


/**
 * Updates an email link.
 *
 * @param {string} htmlId
 * @param {string} emailAddress
 */
function setEmailLinkById(htmlId, emailAddress) {
    const element = getEmailElement(htmlId);
    const email = emailAddress?.trim() ?? "";

    if (!(element instanceof HTMLAnchorElement)) {
        throw new Error(
            `Element "${htmlId}" is not an anchor element.`
        );
    }

    element.textContent = email;
    element.href = email === "" ? "" : `mailto:${email}`;
}


/* =========================================================
   IMAGE FUNCTIONS
   ========================================================= */

/**
 * Updates an image source and optional alternate text.
 *
 * @param {string} htmlId
 * @param {string} imageSource
 * @param {string|null} alternateText
 */
function setImageById(htmlId, imageSource, alternateText = null) {
    const element = getEmailElement(htmlId);

    if (!(element instanceof HTMLImageElement)) {
        throw new Error(
            `Element "${htmlId}" is not an image element.`
        );
    }

    element.src = imageSource ?? "";

    if (alternateText !== null) {
        element.alt = alternateText;
    }
}


/* =========================================================
   SECTION VISIBILITY
   ========================================================= */

/**
 * Shows or hides a section.
 *
 * Hidden sections remain in the document until
 * removeHiddenSections() or getFinalEmailHtml() is called.
 *
 * @param {string} sectionId
 * @param {boolean} isVisible
 */
function setSectionVisibility(sectionId, isVisible) {
    const section = getEmailElement(sectionId);

    section.dataset.emailVisible = isVisible ? "true" : "false";

    if (isVisible) {
        section.style.removeProperty("display");
        section.removeAttribute("hidden");
    } else {
        section.style.setProperty("display", "none", "important");
        section.setAttribute("hidden", "");
    }
}


/**
 * Shows the header section.
 *
 * @param {boolean} isVisible
 */
function setHeaderVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.header,
        Boolean(isVisible)
    );
}


/**
 * Shows the body section.
 *
 * @param {boolean} isVisible
 */
function setBodyVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.body,
        Boolean(isVisible)
    );
}


/**
 * Shows the request section.
 *
 * @param {boolean} isVisible
 */
function setRequestVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.request,
        Boolean(isVisible)
    );
}


/**
 * Shows the attachments section.
 *
 * @param {boolean} isVisible
 */
function setAttachmentsVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.attachments,
        Boolean(isVisible)
    );
}


/**
 * Shows the signature section.
 *
 * @param {boolean} isVisible
 */
function setSignatureVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.signature,
        Boolean(isVisible)
    );
}


/**
 * Shows the footer section.
 *
 * @param {boolean} isVisible
 */
function setFooterVisibility(isVisible) {
    setSectionVisibility(
        EMAIL_SECTION_IDS.footer,
        Boolean(isVisible)
    );
}


/**
 * Sets the visibility of all email sections.
 *
 * Any option not supplied defaults to true.
 *
 * @param {{
 *   header?: boolean,
 *   body?: boolean,
 *   request?: boolean,
 *   attachments?: boolean,
 *   signature?: boolean,
 *   footer?: boolean
 * }} options
 */
function setEmailSectionVisibility(options = {}) {
    const settings = {
        header: true,
        body: true,
        request: true,
        attachments: true,
        signature: true,
        footer: true,
        ...options
    };

    setHeaderVisibility(settings.header);
    setBodyVisibility(settings.body);
    setRequestVisibility(settings.request);
    setAttachmentsVisibility(settings.attachments);
    setSignatureVisibility(settings.signature);
    setFooterVisibility(settings.footer);
}


/**
 * Shows every email section.
 */
function showAllSections() {
    setEmailSectionVisibility({
        header: true,
        body: true,
        request: true,
        attachments: true,
        signature: true,
        footer: true
    });
}


/**
 * Removes hidden sections from the current document.
 *
 * This cannot be undone unless the original template is reloaded.
 */
function removeHiddenSections() {
    Object.values(EMAIL_SECTION_IDS).forEach(sectionId => {
        const section = document.getElementById(sectionId);

        if (!section) {
            return;
        }

        const isHidden =
            section.dataset.emailVisible === "false" ||
            section.hasAttribute("hidden") ||
            section.style.display === "none";

        if (isHidden) {
            section.remove();
        }
    });
}


/* =========================================================
   ATTACHMENT LIST
   ========================================================= */

/**
 * Creates the attachment list from an array of file names.
 *
 * An empty array automatically hides the attachments section.
 *
 * @param {string[]} attachmentNames
 */
function setAttachments(attachmentNames) {
    if (!Array.isArray(attachmentNames)) {
        throw new Error(
            "setAttachments requires an array of file names."
        );
    }

    const validAttachments = attachmentNames
        .filter(name => typeof name === "string")
        .map(name => name.trim())
        .filter(name => name !== "");

    if (validAttachments.length === 0) {
        setAttachmentsVisibility(false);
        return;
    }

    const list = document.createElement("ul");
    list.style.margin = "0";
    list.style.paddingLeft = "20px";

    validAttachments.forEach(fileName => {
        const item = document.createElement("li");
        item.textContent = fileName;
        list.appendChild(item);
    });

    const attachmentContainer =
        getEmailElement("attachmentsList");

    attachmentContainer.replaceChildren(list);
    setAttachmentsVisibility(true);
}


/* =========================================================
   REQUEST SECTION
   ========================================================= */

/**
 * Configures the request section.
 *
 * @param {{
 *   label?: string,
 *   title?: string,
 *   body?: string,
 *   buttonText?: string,
 *   buttonUrl?: string
 * }} request
 */
function setRequest(request = {}) {
    const {
        label = "Requested Action",
        title = "",
        body = "",
        buttonText = "COMPLETE REQUEST",
        buttonUrl = ""
    } = request;

    fillItemById("requestLabel", label);
    fillItemById("requestTitle", title);
    fillItemById("requestBody", body);
    fillLinkById("requestButton", buttonText, buttonUrl);

    setRequestVisibility(true);
}


/* =========================================================
   SIGNATURE SECTION
   ========================================================= */

/**
 * Configures the email signature.
 *
 * @param {{
 *   closing?: string,
 *   name?: string,
 *   title?: string,
 *   organization?: string,
 *   email?: string,
 *   phone?: string
 * }} signature
 */
function setSignature(signature = {}) {
    const {
        closing = "Thank you,",
        name = "",
        title = "",
        organization = "UWGB UREC Climbing Tower",
        email = "",
        phone = ""
    } = signature;

    fillItemById("signatureClosing", closing);
    fillItemById("senderName", name);
    fillItemById("senderTitle", title);
    fillItemById("senderOrganization", organization);
    setEmailLinkById("senderEmail", email);
    fillItemById("senderPhone", phone);

    setSignatureVisibility(true);
}


/* =========================================================
   EMAIL CONTENT
   ========================================================= */

/**
 * Configures the main email header.
 *
 * @param {{
 *   label?: string,
 *   heading?: string,
 *   subtitle?: string
 * }} header
 */
function setEmailHeader(header = {}) {
    const {
        label = "",
        heading = "",
        subtitle = ""
    } = header;

    fillItemById("headerLabel", label);
    fillItemById("emailHeading", heading);
    fillItemById("headerSubtitle", subtitle);
}


/**
 * Sets the browser document title.
 *
 * @param {string} title
 */
function setEmailTitle(title) {
    document.title = title ?? "";

    const titleElement = document.getElementById("emailTitle");

    if (titleElement) {
        titleElement.textContent = title ?? "";
    }
}


/**
 * Sets the hidden preheader text shown by many inboxes.
 *
 * @param {string} preheaderText
 */
function setPreheaderText(preheaderText) {
    fillItemById("preheaderText", preheaderText);
}


/**
 * Sets the greeting and trusted HTML body.
 *
 * @param {string} greeting
 * @param {string} bodyHtml
 */
function setEmailBody(greeting, bodyHtml) {
    fillItemById("recipientGreeting", greeting);
    fillHtmlById("emailBody", bodyHtml);
}


/* =========================================================
   FINAL HTML OUTPUT
   ========================================================= */

/**
 * Creates final email HTML without altering the visible preview.
 *
 * Hidden sections are removed from the returned copy.
 *
 * @returns {string}
 */
function getFinalEmailHtml() {
    const documentCopy =
        document.documentElement.cloneNode(true);

    Object.values(EMAIL_SECTION_IDS).forEach(sectionId => {
        const section = documentCopy.querySelector(
            `#${CSS.escape(sectionId)}`
        );

        if (!section) {
            return;
        }

        const isHidden =
            section.dataset.emailVisible === "false" ||
            section.hasAttribute("hidden") ||
            section.style.display === "none";

        if (isHidden) {
            section.remove();
        }
    });

    /*
     * Remove the JavaScript file reference because JavaScript should
     * not be included in the final email sent to recipients.
     */
    documentCopy
        .querySelectorAll("script")
        .forEach(script => script.remove());

    return `<!doctype html>\n${documentCopy.outerHTML}`;
}


/**
 * Copies the completed email HTML to the clipboard.
 *
 * @returns {Promise<void>}
 */
async function copyFinalEmailHtml() {
    const finalHtml = getFinalEmailHtml();

    if (!navigator.clipboard) {
        throw new Error(
            "Clipboard access is unavailable in this browser."
        );
    }

    await navigator.clipboard.writeText(finalHtml);
}


/**
 * Downloads the completed email HTML as a file.
 *
 * @param {string} fileName
 */
function downloadFinalEmailHtml(
    fileName = "uwgb-tower-email.html"
) {
    const finalHtml = getFinalEmailHtml();

    const blob = new Blob(
        [finalHtml],
        { type: "text/html;charset=utf-8" }
    );

    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");

    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);
}


/* =========================================================
   OPTIONAL EXAMPLE
   ========================================================= */

/*
document.addEventListener("DOMContentLoaded", () => {
    setEmailTitle("Tower Availability Reminder");

    setPreheaderText(
        "Please complete your tower availability form by Friday."
    );

    setEmailHeader({
        label: "Tower Team Reminder",
        heading: "Complete Your Availability Form",
        subtitle: "Please submit your response by Friday at 5:00 p.m."
    });

    setEmailBody(
        "Hi Tower Team,",
        `
        <p style="margin:0 0 16px 0;">
            Please complete your availability form for the
            upcoming semester.
        </p>

        <p style="margin:0;">
            Your response helps us create an accurate and fair
            climbing tower schedule.
        </p>
        `
    );

    setRequest({
        label: "Requested Action",
        title: "Submit Your Availability",
        body: "Complete the form by Friday at 5:00 p.m.",
        buttonText: "OPEN AVAILABILITY FORM",
        buttonUrl: "https://example.com/"
    });

    setAttachments([
        "Tower Availability Instructions.pdf",
        "Fall Semester Schedule.pdf"
    ]);

    setSignature({
        closing: "Thank you,",
        name: "Jack London",
        title: "Climbing Tower Lead Supervisor",
        organization: "UWGB UREC Climbing Tower",
        email: "example@uwgb.edu",
        phone: ""
    });

    setEmailSectionVisibility({
        header: true,
        body: true,
        request: true,
        attachments: true,
        signature: true,
        footer: true
    });
});
*/