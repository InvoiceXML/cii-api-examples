# CII REST API Examples: Create, Validate, and Parse Cross Industry Invoice XML

Open-source code examples for working with the **UN/CEFACT Cross Industry Invoice (CII)**, the XML syntax behind **ZUGFeRD**, **Factur-X**, and **XRechnung**. CII is one of the two syntaxes permitted by the European norm **EN 16931** (the other is UBL 2.1), and this repository covers it as a **standalone XML document**, with no PDF wrapper.

Examples in **C#, Java, PHP, JavaScript, Node.js, and Python** covering every common CII operation: invoice creation, validation against the EN 16931 rules, parsing invoice data into JSON, and (experimental) AI-powered conversion of plain PDF invoices into compliant CII documents.

Provided and maintained by [InvoiceXML](https://www.invoicexml.com), a complete e-invoice compliance toolkit for electronic invoicing standards, including CII, ZUGFeRD, Factur-X, XRechnung, UBL, and Peppol BIS Billing, available via REST-API, no-code platforms, and MCP server.

## InvoiceXML vs building it yourself

The EN 16931 specification alone runs over 400 pages, the CII D16B schema is a deep UN/CEFACT tree that no one enjoys hand-rolling, and the validation Schematron updates with every regulatory revision. A realistic in-house build is 6 to 12 weeks of engineering, plus 0.3 to 0.5 FTE permanently allocated to tracking spec changes as more and more EU countries roll out their mandates. InvoiceXML absorbs all of that behind a single REST API covering CII, ZUGFeRD, Factur-X, XRechnung, UBL, and Peppol BIS. EU-hosted, GDPR-compliant, no invoice data persisted between calls, sub-second responses, 99.95% uptime. Your integration keeps calling the same endpoints when the spec changes next quarter, you get the latest compliance out of the box, zero maintenance required.

---

## Table of Contents

- [What is CII?](#what-is-cii)
- [When to use standalone CII](#when-to-use-standalone-cii)
- [Why use this repository](#why-use-this-repository)
- [Operations covered](#operations-covered)
- [Quick start by language](#quick-start-by-language)
  - [Create and validate CII with C# / .NET](#create-and-validate-cii-with-c--net)
  - [Create and validate CII with Java](#create-and-validate-cii-with-java)
  - [Create and validate CII with PHP](#create-and-validate-cii-with-php)
  - [Create and validate CII with JavaScript in the browser](#create-and-validate-cii-with-javascript-in-the-browser)
  - [Create and validate CII with Node.js](#create-and-validate-cii-with-nodejs)
  - [Create and validate CII with Python](#create-and-validate-cii-with-python)
- [Parse CII data as JSON](#parse-cii-data-as-json)
- [Render CII as a readable PDF](#render-cii-as-a-readable-pdf)
- [AI-powered PDF to CII conversion (Experimental)](#ai-powered-pdf-to-cii-conversion-experimental)
- [Profiles and validation](#profiles-and-validation)
- [Related e-invoicing standards](#related-e-invoicing-standards)
- [Frequently asked questions](#frequently-asked-questions)
- [Resources and references](#resources-and-references)
- [Contributing](#contributing)
- [License](#license)

---

## What is CII?

**CII** (Cross Industry Invoice) is an XML schema maintained by **UN/CEFACT**, the United Nations Centre for Trade Facilitation and Electronic Business. The version referenced by EN 16931 is **D16B**, whose root element is `CrossIndustryInvoice`.

CII is a **syntax**, not a document format in its own right. That distinction explains most of the confusion around it:

- **ZUGFeRD** and **Factur-X** are CII XML embedded inside a PDF/A-3 container.
- **XRechnung** can be CII (or UBL), with extra German rules layered on top.
- **Standalone CII** is the same XML on its own, with no PDF and no national CIUS.

This repository covers that last case. The examples produce plain **EN 16931** CII, declaring `urn:cen.eu:en16931:2017` as the guideline identifier in BT-24.

## When to use standalone CII

Reach for standalone CII when you want the structured data without a PDF wrapper:

- **ERP and EDI pipelines** that ingest or emit CII directly, with no interest in a visual document.
- **Embedding into your own PDF/A-3** container, when you already have a designed invoice PDF and only need the XML payload to attach.
- **Archival** of the machine-readable data separately from the human-readable rendering.
- **Interoperating with CII-based partners** who exchange raw XML over their own channels.

If you want a human-readable PDF with the data embedded, you want the hybrid formats instead: see the [ZUGFeRD examples](https://github.com/InvoiceXML/zugferd-api-examples) (Germany) or the [Factur-X examples](https://github.com/InvoiceXML/facturx-api-examples) (France). If you are invoicing a German public authority, you want [XRechnung](https://github.com/InvoiceXML/xrechnung-api-examples). If your recipient is on the Peppol network, they will most likely expect UBL.

## Why use this repository

The CII D16B schema is large, deeply nested, and documented mainly as UN/CEFACT reference material rather than as anything a developer can copy and run. This repository provides **side-by-side, copy-pasteable code examples** for the languages most commonly used in invoicing, ERP, and accounting systems, so developers can:

- Pick the language already used in their stack
- Skip hand-building a `CrossIndustryInvoice` tree from the raw schema
- Run the same operations (create, validate, parse to JSON, AI-convert) in any of the supported languages
- Compare behavior and output across implementations
- Use the produced invoices as test fixtures for their own systems

Every example in this repository produces or consumes invoices validated against the CII D16B XSD and the EN 16931 Schematron before delivery.

## Operations covered

| Operation | Description |
|---|---|
| **[Create CII](https://www.invoicexml.com/docs/api/create/cii)** | Build an EN 16931 compliant CII D16B XML invoice from a structured JSON request |
| **[Validate CII](https://www.invoicexml.com/docs/api/validate/cii)** | Verify a CII file against the D16B XSD and the business rules for the profile it declares |
| **[Extract JSON](https://www.invoicexml.com/docs/api/extract/json)** | Parse a CII XML file into structured invoice data as JSON for downstream systems |
| **[Render to PDF](https://www.invoicexml.com/docs/api/render/cii/to/pdf)** | Render a CII XML file into a human-readable PDF preview |
| **[AI Convert](https://www.invoicexml.com/docs/api/transform/to/cii)** *(experimental)* | Convert a plain, unstructured PDF invoice into a compliant CII document using a large language model |

---

## Quick start by language

Each language has its own folder containing all four operations as standalone, runnable files. Open the folder for your language to see installation steps, dependencies, and usage.

### Create and validate CII with C# / .NET

```
/csharp
  Create.cs         # Build a CII D16B XML invoice
  Validate.cs       # Validate against the D16B XSD and EN 16931 rules
  ExtractJson.cs    # Parse a CII XML into JSON
  AiConvert.cs      # (Experimental) Convert plain PDF to CII using AI
```

[Open the C# / .NET examples →](./csharp)

### Create and validate CII with Java

```
/java
  Create.java
  Validate.java
  ExtractJson.java
  AiConvert.java
  Render.java
```

[Open the Java examples →](./java)

### Create and validate CII with PHP

```
/php
  create.php
  validate.php
  extract-json.php
  ai-convert.php
  render.php
```

[Open the PHP examples →](./php)

### Create and validate CII with JavaScript in the browser

Browser-side examples for generating and inspecting CII invoices client-side, useful for web-based invoicing apps that want to avoid round-trips to a server.

```
/javascript
  create.html
  validate.html
  extract-json.html
  ai-convert.html
  render.html
```

[Open the JavaScript examples →](./javascript)

### Create and validate CII with Node.js

```
/nodejs
  create.js
  validate.js
  extract-json.js
  ai-convert.js
  render.js
```

[Open the Node.js examples →](./nodejs)

### Create and validate CII with Python

```
/python
  create.py
  validate.py
  extract_json.py
  ai_convert.py
  render.py
```

[Open the Python examples →](./python)

---

## Parse CII data as JSON

Many downstream systems (ERPs, accounting software, expense management tools) work natively with JSON rather than XML, and the CII tree is unpleasant to walk by hand: business terms live several namespaces deep under names like `ram:SpecifiedLineTradeAgreement`. The `ExtractJson` examples in every language upload a CII XML file and receive the EN 16931 model back as clean JSON: seller, buyer, line items, tax breakdown, totals, and payment details.

The invoice document sits under the **`invoice`** key of the response, so you read `invoice.seller.name` or `invoice.totals.grandTotalAmount` (BT-112). A complete sample response with every field of the invoice model populated is in [json/extract-json-response.json](./json/extract-json-response.json); optional fields that are absent from the source document come back as `null` (empty arrays for lists).

This is the fastest way to get invoice data out of a CII file and into a modern data pipeline.

---

## Render CII as a readable PDF

Standalone CII is a machine format with no visual layer, so nobody can read it without tooling, and eyeballing a raw `CrossIndustryInvoice` tree is not a reasonable way to check an invoice.

The `Render` examples upload a CII XML file and receive a formatted PDF preview, with every EN 16931 business term mapped to a readable layout. All CII profiles from MINIMUM through EXTENDED are supported, the profile being auto-detected from BT-24, and the labels can be localized with the `language` field (`en`, `de`, `fr`).

Two things worth knowing:

- **The PDF has no legal standing.** It is a visual representation for humans; the XML file remains the authoritative invoice for compliance and tax purposes. If you need a legally exchangeable hybrid PDF, you want [ZUGFeRD](https://github.com/InvoiceXML/zugferd-api-examples) or [Factur-X](https://github.com/InvoiceXML/facturx-api-examples) instead.
- **It is generated fresh from the structured data**, not extracted from any existing PDF layer. That is what makes it useful for verifying what your XML actually says, including for CII you extracted out of a ZUGFeRD or Factur-X file.

---

## AI-powered PDF to CII conversion (Experimental)

> **Experimental feature. Human verification required before any production use.**
>
> Real-world PDF invoices are often messy: scanned at low quality, irregularly formatted, multi-page, multilingual, or missing fields that EN 16931 requires. Producing a fully compliant CII document from such an input is non-trivial, and AI extraction can make subtle mistakes that automated validators may not catch: wrong tax category codes, transposed amounts, missing seller VAT identifiers, incorrect currency formatting.
>
> Output from the `AiConvert` examples **must always be reviewed by a human** before the invoice is sent to a customer or submitted to a tax authority. This is a developer convenience for bootstrapping test data and prototyping, **not** an unattended production pipeline.

The `AiConvert` examples in each language demonstrate the same pattern:

1. Read a plain (non-CII) PDF invoice
2. A large language model reads the document and infers the invoice fields (seller, buyer, line items, tax breakdown, totals, payment terms)
3. The API maps those fields to the EN 16931 semantic model and generates CII D16B XML

Useful for bootstrapping test fixtures from invoices you already have, rather than hand-writing CII trees.

---

## Profiles and validation

Validation runs in two layers, and what the second layer checks depends on what your document declares in **BT-24** (`GuidelineSpecifiedDocumentContextParameter`):

| Declared profile | Rules applied |
|---|---|
| `urn:cen.eu:en16931:2017` (what these examples produce) | CII D16B XSD, then the full EN 16931 Schematron |
| A Factur-X / ZUGFeRD profile (MINIMUM, BASIC WL, BASIC, EXTENDED) | The XSD, then that profile's own official rules, so an EXTENDED invoice is not wrongly failed for using extended fields |
| An XRechnung identifier | The XSD, then the KoSIT BR-DE rules |
| Anything unrecognized | The XSD and the EN 16931 base rules, plus a `PROFILE-DETECTION` warning explaining the fallback. An exotic identifier is never auto-failed |

The response reports `data.customizationId` (what the document declared) and `data.profile` (the rule set that was actually applied), and every finding carries a `layer` field telling you whether it came from the schema (`xsd`), the core rules (`en16931`), or a profile overlay (`cius`).

---

## Related e-invoicing standards

CII is one syntax among several in the European e-invoicing landscape:

- **UBL 2.1**: the other EN 16931 syntax, from OASIS. Pure XML like standalone CII, and the native format of the Peppol network.
- **ZUGFeRD 2.x**: German hybrid format, CII XML embedded in a PDF/A-3. See the [ZUGFeRD examples](https://github.com/InvoiceXML/zugferd-api-examples).
- **Factur-X**: French equivalent of ZUGFeRD 2.x. Technically identical specification, different branding. See the [Factur-X examples](https://github.com/InvoiceXML/facturx-api-examples).
- **XRechnung**: the German CIUS of EN 16931, expressible in CII or UBL, mandatory for German B2G. See the [XRechnung examples](https://github.com/InvoiceXML/xrechnung-api-examples).
- **Peppol BIS Billing 3.0**: UBL-based subset exchanged over the Peppol network.
- **EN 16931**: the European norm that underlies all of the above.

For a side-by-side comparison of when to use which format, see the [e-invoicing standards comparison on invoicexml.com](https://invoicexml.com/standards).

---

## Frequently asked questions

**What is the difference between CII and ZUGFeRD or Factur-X?**
ZUGFeRD and Factur-X *are* CII, wrapped in a PDF/A-3 so a human can read the invoice too. The XML inside them is structurally the same as what this repository produces. Use standalone CII when you do not need the PDF layer.

**How does CII differ from UBL?**
Both are valid EN 16931 syntaxes expressing the same semantic model, but they are entirely different XML: CII comes from UN/CEFACT, UBL from OASIS. CII dominates in Germany and France (via ZUGFeRD and Factur-X); UBL dominates on the Peppol network and in the Nordics, the Netherlands, and Belgium.

**Which CII version is used?**
D16B, the version referenced by EN 16931. The retired `CrossIndustryDocument` root used by ZUGFeRD 1.0 is not interchangeable and is reported as schema-invalid.

**Can I embed the output in a ZUGFeRD or Factur-X PDF?**
Yes. The generated CII XML is structurally identical to the XML those formats embed, so you can attach it to a PDF/A-3 in your own pipeline. If you would rather receive the complete hybrid PDF in one call, use `/v1/create/zugferd` or `/v1/create/facturx` instead.

**Do I need to calculate totals myself?**
No. The `invoice.totals` object and the `invoice.vatBreakdowns` array are calculated from your line items when omitted. Provide them explicitly only if you need to override the computed result.

**Is the output validated automatically?**
Yes. Every generated CII document is validated against the D16B XSD and the EN 16931 Schematron before delivery. If validation fails (for example mismatched totals), the API returns a 400 with `errorCode` 4001 and the violated rules as findings, each carrying the rule id, layer, and the JSON field paths to correct.

---

## Resources and references

- [UN/CEFACT Cross Industry Invoice (CII) specification](https://unece.org/trade/uncefact/e-invoice)
- [EN 16931 (European Committee for Standardization)](https://www.cencenelec.eu/)
- [Factur-X specification (FNFE-MPE)](https://fnfe-mpe.org/factur-x/), which profiles CII
- [ZUGFeRD specification (FeRD)](https://www.ferd-net.de/standards/zugferd), which profiles CII
- [invoicexml.com](https://invoicexml.com): guides, tutorials, validators, and standard comparisons for CII, ZUGFeRD, Factur-X, XRechnung, UBL, and Peppol

---

## Contributing

Pull requests are welcome. Useful contributions include:

- Additional languages (Go, Rust, Ruby, Kotlin, Swift)
- Alternative library implementations in existing languages
- Sample invoice fixtures covering edge cases (multi-currency, intra-EU reverse charge, zero-rated supplies, credit notes)
- Translations of the README and inline comments
- Bug fixes and validation rule clarifications

Please include a working example, pinned dependency versions, and a short note in the relevant language folder's README.
