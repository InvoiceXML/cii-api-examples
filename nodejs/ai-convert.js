// Experimental: convert a plain PDF invoice to CII using AI.
// Review output manually before sending to a customer or tax authority.
//
// Get API key: https://www.invoicexml.com/account/authentication
// Docs:        https://www.invoicexml.com/docs/api/transform/to/cii
//
// Requires Node.js 18+. No dependencies.

const fs = require('fs');

// Raw key only, without the "Bearer " prefix (it is added below).
const apiKey = 'YOUR_API_KEY';

(async () => {
    const form = new FormData();
    form.append('file', new Blob([fs.readFileSync('plain-invoice.pdf')], { type: 'application/pdf' }), 'plain-invoice.pdf');

    const response = await fetch('https://api.invoicexml.com/v1/transform/to/cii', {
        method: 'POST',
        headers: { Authorization: `Bearer ${apiKey}` },
        body: form,
    });

    const xml = await response.text();
    if (!response.ok) {
        console.error(`InvoiceXML API error ${response.status}: ${xml}`);
        process.exit(1);
    }

    fs.writeFileSync('converted-cii.xml', xml);
    console.log(`Saved converted-cii.xml (${xml.length} chars)`);
    console.log('Review the output manually before sending.');
})();
