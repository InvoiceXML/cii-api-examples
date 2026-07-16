# CII for JavaScript in the Browser: InvoiceXML API Examples

Browser-side JavaScript examples for creating, validating, and parsing **CII** electronic invoices using the **[InvoiceXML API](https://www.invoicexml.com)**. Self-contained HTML files using vanilla JavaScript with native `fetch` and `FormData`. Compatible with **modern browsers**: Chrome, Firefox, Safari, Edge. **No build step, no framework, no dependencies.**

For background on the CII syntax itself (what it is, when to use it standalone), see the [main repository README](../README.md).

## ⚠️ Security warning: API keys in browser code

API keys embedded directly in browser-side JavaScript are visible to every visitor through View Source or DevTools. Anyone can extract the key and use it against your InvoiceXML quota.

These examples are appropriate for:

- **Local prototyping** and testing
- **Internal tools** behind your company's authentication
- **Single-user desktop apps** wrapped in Electron or Tauri

For **public-facing production websites**, route requests through your own backend so the API key stays server-side. See the [Node.js examples](../nodejs) or [PHP examples](../php) for server-side integration patterns.

## Get your API key

Sign up and generate a key here:

**→ [https://www.invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication)**

Set `apiKey` in the examples to the raw key only, without the `Bearer ` prefix. If your account page shows the full header value (e.g. `Bearer ixml_a1b2c3...`), copy only the part after `Bearer `. The code adds the prefix itself when building the `Authorization` header.

## Requirements

- Any modern browser:
  - Chrome 95+
  - Firefox 90+
  - Safari 14+
  - Edge 95+
- An InvoiceXML API key
- Optional: a local web server (`python -m http.server`, `npx serve`, etc.) for testing. The examples also work by opening the `.html` files directly via `file://`.

## Files in this folder

| File | Operation | API endpoint |
|---|---|---|
| [`create.html`](./create.html) | Build a CII D16B XML invoice | [`POST /v1/create/cii`](https://www.invoicexml.com/docs/api/create/cii) |
| [`validate.html`](./validate.html) | Validate a CII file against the D16B XSD and EN 16931 rules | [`POST /v1/validate/cii`](https://www.invoicexml.com/docs/api/validate/cii) |
| [`extract-json.html`](./extract-json.html) | Parse a CII XML into JSON | [`POST /v1/extract/json`](https://www.invoicexml.com/docs/api/extract/json) |
| [`ai-convert.html`](./ai-convert.html) | (Experimental) Convert a plain PDF to CII with AI | [`POST /v1/transform/to/cii`](https://www.invoicexml.com/docs/api/transform/to/cii) |
| [`render.html`](./render.html) | Render CII XML into a human-readable PDF | [`POST /v1/render/cii/to/pdf`](https://www.invoicexml.com/docs/api/render/cii/to/pdf) |

Each file is a complete, runnable HTML page. Open it in a browser, paste your API key into the `<script>` block, and click the button.

---

## Create a CII invoice in the browser

```html
<button id="run">Generate invoice</button>
<script>
const payload = {
    invoice: {
        invoiceNumber: 'CII-2026-001',
        issueDate:     '2026-05-18',
        currency:      'EUR',
        seller: { name: 'Acme GmbH', vatIdentifier: 'DE123456789', /* ... */ },
        buyer:  { name: 'Globex SAS', /* ... */ },
        paymentDetails: { paymentAccountIdentifier: 'DE89370400440532013000' },
        lines: [{ quantity: 10, priceDetails: { netPrice: 150.00 },
                  vatInformation: { rate: 19.00 }, item: { name: 'Senior consulting' } }],
    },
};

document.getElementById('run').addEventListener('click', async () => {
    const response = await fetch('https://api.invoicexml.com/v1/create/cii', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: 'Bearer ' + apiKey,
        },
        body: JSON.stringify(payload),
    });
    const xml = await response.text();
    const blob = new Blob([xml], { type: 'application/xml' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = 'invoice-cii.xml';
    a.click();
});
</script>
```

The XML is downloaded directly to the user's machine via `URL.createObjectURL` and a triggered `<a download>`. Totals and the VAT breakdown are calculated from the line items when omitted.

[Full example: `create.html`](./create.html) | [API reference](https://www.invoicexml.com/docs/api/create/cii)

---

## Validate a CII file in the browser

```html
<input type="file" id="file" accept=".xml,application/xml,text/xml">
<button id="run">Validate</button>
<pre id="output"></pre>

<script>
document.getElementById('run').addEventListener('click', async () => {
    const file = document.getElementById('file').files[0];
    const form = new FormData();
    form.append('file', file);

    const response = await fetch('https://api.invoicexml.com/v1/validate/cii', {
        method: 'POST',
        headers: { Authorization: 'Bearer ' + apiKey },
        body: form,
    });
    document.getElementById('output').textContent = await response.text();
});
</script>
```

The file is read directly from the `<input type="file">` element. No FileReader or Blob conversion needed: `FormData.append(file)` accepts the `File` object directly.

[Full example: `validate.html`](./validate.html) | [API reference](https://www.invoicexml.com/docs/api/validate/cii)

---

## Extract CII data as JSON in the browser

Useful for displaying invoice details in a web UI, populating form fields from an uploaded CII file, or building drag-and-drop invoice review tools.

```js
const form = new FormData();
form.append('file', fileInput.files[0]);

const response = await fetch('https://api.invoicexml.com/v1/extract/json', {
    method: 'POST',
    headers: { Authorization: 'Bearer ' + apiKey },
    body: form,
});
const invoice = await response.json();
// The invoice fields sit at the root of the response.
console.log(invoice.seller.name, invoice.totals.grandTotalAmount);
```

[Full example: `extract-json.html`](./extract-json.html) | [API reference](https://www.invoicexml.com/docs/api/extract/json)


---

## (Experimental) Convert a plain PDF to CII with AI

> **Experimental feature. Human verification required before any production use.**
>
> AI extraction can make subtle mistakes that automated validators may not catch: wrong tax category codes, transposed amounts, missing seller VAT identifiers, incorrect currency formatting. Always review the output PDF before sending it to a customer or tax authority. See the [AI conversion notes in the main README](../README.md#ai-powered-pdf-to-cii-conversion-experimental).

[Full example: `ai-convert.html`](./ai-convert.html) | [API reference](https://www.invoicexml.com/docs/api/transform/to/cii)

---

## Render CII as a readable PDF in the browser

CII is a machine format with no visual layer, so nobody can read it without tooling. This endpoint renders the XML into a formatted PDF preview, generated fresh from the structured data rather than extracted from any existing PDF layer, which makes it a quick way to eyeball what your XML actually says. The PDF is for reading only; the XML file remains the authoritative invoice for compliance and tax purposes.

```js
const form = new FormData();
form.append('file', fileInput.files[0]);
form.append('language', 'en');   // en, de, or fr

const response = await fetch('https://api.invoicexml.com/v1/render/cii/to/pdf', {
    method: 'POST',
    headers: { Authorization: 'Bearer ' + apiKey },
    body: form,
});

const blob = await response.blob();
const a = document.createElement('a');
a.href = URL.createObjectURL(blob);
a.download = 'invoice-preview.pdf';
a.click();
```

[Full example: `render.html`](./render.html) | [API reference](https://www.invoicexml.com/docs/api/render/cii/to/pdf)

---

## Framework integration

The patterns below assume the API key is held server-side and proxied through your backend. Replace `/api/cii/...` with your own proxy endpoints.

### React

```jsx
function CreateCIIButton() {
    const [status, setStatus] = useState('');

    async function handleClick() {
        setStatus('Generating...');
        const response = await fetch('/api/cii/create', { method: 'POST' });
        const blob = await response.blob();
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'invoice-cii.xml';
        a.click();
        setStatus('Done');
    }

    return <button onClick={handleClick}>Generate invoice</button>;
}
```

### Vue 3

```vue
<template>
    <button @click="handleClick">Generate invoice</button>
    <p>{{ status }}</p>
</template>

<script setup>
import { ref } from 'vue';

const status = ref('');

async function handleClick() {
    status.value = 'Generating...';
    const response = await fetch('/api/cii/create', { method: 'POST' });
    const blob = await response.blob();
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = 'invoice-cii.xml';
    a.click();
    status.value = 'Done';
}
</script>
```

### Svelte

```svelte
<script>
    let status = '';

    async function handleClick() {
        status = 'Generating...';
        const response = await fetch('/api/cii/create', { method: 'POST' });
        const blob = await response.blob();
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'invoice-cii.xml';
        a.click();
        status = 'Done';
    }
</script>

<button on:click={handleClick}>Generate invoice</button>
<p>{status}</p>
```

### Angular

Use `HttpClient` with `responseType: 'blob'`:

```typescript
this.http.post('/api/cii/create', payload, { responseType: 'blob' })
    .subscribe(blob => {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'invoice-cii.xml';
        a.click();
    });
```

---

## Common issues

- **CORS errors in the browser console**: if the browser blocks the request, route through your own backend (proxy) instead of calling the API directly from the browser.
- **API key visible in DevTools**: this is expected when the key is embedded in client-side code. For anything beyond local prototyping, move the call to a server-side proxy.
- **`HTTP 401 Unauthorized`**: API key missing or invalid. Generate one at [invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication). A frequent cause: setting `apiKey` to the whole `Bearer xxx` value, which sends `Bearer Bearer xxx`. Set the raw key only.
- **`HTTP 400 Bad Request` on Create**: a required field is missing or malformed. Frequent causes: `IssueDate` not in ISO format (`YYYY-MM-DD`), `Currency` not in ISO 4217 (`EUR`, `USD`), country codes not in ISO 3166-1 alpha-2 (`DE`, `FR`).
- **File input always empty**: `fileInput.files[0]` is `undefined` until the user actually picks a file. Always check before reading.
- **Mixed content warnings**: if your page is served over `https://`, all API calls must also be `https://`. The examples use `https://api.invoicexml.com` so this is fine, just do not change it to `http://`.
- **`file://` protocol limitations**: opening the HTML directly via `file://` works for these examples but breaks some browser APIs in stricter modes. Use a local server (`python -m http.server`, `npx serve`) if you hit issues.

---

## Resources

- [Get an InvoiceXML API key](https://www.invoicexml.com/account/authentication)
- [Create CII API reference](https://www.invoicexml.com/docs/api/create/cii)
- [Validate CII API reference](https://www.invoicexml.com/docs/api/validate/cii)
- [Extract JSON API reference](https://www.invoicexml.com/docs/api/extract/json)
- [Extract XML API reference](https://www.invoicexml.com/docs/api/extract/xml)
- [Transform to CII API reference](https://www.invoicexml.com/docs/api/transform/to/cii)
- [MDN: Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API)
- [MDN: FormData](https://developer.mozilla.org/en-US/docs/Web/API/FormData)
- [Main repository README](../README.md)
