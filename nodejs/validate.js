// Validate a CII XML file using the InvoiceXML API.
// Runs the CII D16B XSD and the business rules for the profile the document declares.
// Get API key: https://www.invoicexml.com/account/authentication
// Docs:        https://www.invoicexml.com/docs/api/validate/cii
//
// Requires Node.js 18+. No dependencies.

const fs = require('fs');

// Raw key only, without the "Bearer " prefix (it is added below).
const apiKey = 'YOUR_API_KEY';

(async () => {
    const form = new FormData();
    form.append('file', new Blob([fs.readFileSync('invoice.xml')], { type: 'application/xml' }), 'invoice.xml');

    const response = await fetch('https://api.invoicexml.com/v1/validate/cii', {
        method: 'POST',
        headers: { Authorization: `Bearer ${apiKey}` },
        body: form,
    });

    const report = await response.text();
    if (!response.ok) {
        console.error(`InvoiceXML API error ${response.status}: ${report}`);
        process.exit(1);
    }

    console.log(report);
})();
