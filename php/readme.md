# CII for PHP: InvoiceXML API Examples

PHP code samples for creating, validating, and parsing **CII** electronic invoices using the **[InvoiceXML API](https://www.invoicexml.com)**. Compatible with **PHP 7.0+** (PHP 8.x recommended), works in plain scripts, Laravel, Symfony, WordPress, WooCommerce, Drupal, Magento, or any other PHP framework. **Zero Composer dependencies**: uses PHP's built-in cURL extension.

For background on the CII syntax itself (what it is, when to use it standalone), see the [main repository README](../README.md).

## Get your API key

Every example in this folder calls the InvoiceXML REST API. Sign up and generate a free API key here:

**→ [https://www.invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication)**

Pass it as a Bearer token on every request:

```
Authorization: Bearer YOUR_API_KEY
```

**Important:** set `$apiKey` in the examples to the raw key only, without the `Bearer ` prefix. If your account page shows the full header value (e.g. `Bearer ixml_a1b2c3...`), copy only the part after `Bearer `. The code adds the prefix itself when building the `Authorization` header.

## Requirements

- **PHP 7.0 or later** (PHP 8.2+ recommended)
- The **cURL extension** (`php-curl`), bundled with most PHP installations by default
- No Composer, no external libraries

These examples use the built-in `curl_*` functions and `CURLFile` (available since PHP 5.5), so they run on essentially any modern PHP install without `composer install`.

## Files in this folder

| File | Operation | API endpoint |
|---|---|---|
| [`create.php`](./create.php) | Build a CII D16B XML invoice | [`POST /v1/create/cii`](https://www.invoicexml.com/docs/api/create/cii) |
| [`validate.php`](./validate.php) | Validate a CII file against the D16B XSD and EN 16931 rules | [`POST /v1/validate/cii`](https://www.invoicexml.com/docs/api/validate/cii) |
| [`extract-json.php`](./extract-json.php) | Parse a CII XML into JSON | [`POST /v1/extract/json`](https://www.invoicexml.com/docs/api/extract/json) |
| [`ai-convert.php`](./ai-convert.php) | (Experimental) Convert a plain PDF to CII with AI | [`POST /v1/transform/to/cii`](https://www.invoicexml.com/docs/api/transform/to/cii) |
| [`render.php`](./render.php) | Render CII XML into a human-readable PDF | [`POST /v1/render/cii/to/pdf`](https://www.invoicexml.com/docs/api/render/cii/to/pdf) |

Each file is standalone and runnable with `php create.php`. Open the file, replace `YOUR_API_KEY` with your real key, and execute.

> **Note on the snippets below:** they are excerpts from those files and assume `$apiKey` is already defined. When in doubt, copy the complete file.

---

## Create a CII invoice in PHP

```php
$payload = [
    'invoice' => [
        'invoiceNumber' => 'CII-2026-001',
        'issueDate'     => '2026-05-18',
        'currency'      => 'EUR',
        'seller' => [
            'name'              => 'Acme GmbH',
            'vatIdentifier'     => 'DE123456789',
            'legalRegistration' => ['identifier' => 'HRB 12345'],
            'postalAddress'     => ['line1' => 'Hauptstraße 12', 'city' => 'Berlin', 'postCode' => '10115', 'country' => 'DE'],
        ],
        'buyer' => [
            'name'          => 'Globex SAS',
            'postalAddress' => ['line1' => '15 rue de Rivoli', 'city' => 'Paris', 'postCode' => '75001', 'country' => 'FR'],
        ],
        'paymentDetails' => ['paymentAccountIdentifier' => 'DE89370400440532013000'],
        'lines' => [[
            'quantity'       => 10,
            'priceDetails'   => ['netPrice' => 150.00],
            'vatInformation' => ['rate' => 19.00],
            'item'           => ['name' => 'Senior consulting'],
        ]],
    ],
];

$ch = curl_init('https://api.invoicexml.com/v1/create/cii');
curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_POSTFIELDS     => json_encode($payload),
    CURLOPT_HTTPHEADER     => [
        'Content-Type: application/json',
        'Authorization: Bearer ' . $apiKey,
    ],
]);
$xml = curl_exec($ch);
file_put_contents('invoice-cii.xml', $xml);
```

Totals and the VAT breakdown are calculated from the line items when omitted, and the document declares `urn:cen.eu:en16931:2017` in BT-24.

The response is the CII D16B XML document, validated against the EN 16931 rules before delivery.

[Full example: `create.php`](./create.php) | [API reference](https://www.invoicexml.com/docs/api/create/cii)

---

## Validate a CII file in PHP

```php
$ch = curl_init('https://api.invoicexml.com/v1/validate/cii');
curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_POSTFIELDS     => [
        'file' => new CURLFile('invoice.xml', 'application/xml'),
    ],
    CURLOPT_HTTPHEADER     => ['Authorization: Bearer ' . $apiKey],
]);
$report = curl_exec($ch);
echo $report;
```

Returns a JSON validation report listing any rule failures (EN 16931 BR-* and BR-CO-* rules).

[Full example: `validate.php`](./validate.php) | [API reference](https://www.invoicexml.com/docs/api/validate/cii)

---

## Extract CII data as JSON in PHP

Useful for feeding CII invoices into REST APIs, ERPs, accounting systems, or any pipeline that prefers JSON over XML.

```php
$ch = curl_init('https://api.invoicexml.com/v1/extract/json');
curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_POSTFIELDS     => ['file' => new CURLFile('invoice.xml', 'application/xml')],
    CURLOPT_HTTPHEADER     => ['Authorization: Bearer ' . $apiKey],
]);
$json = curl_exec($ch);
$data = json_decode($json, true);
// The invoice fields sit at the root of the response.
echo $data['seller']['name'];
```

[Full example: `extract-json.php`](./extract-json.php) | [API reference](https://www.invoicexml.com/docs/api/extract/json)

---

---

## (Experimental) Convert a plain PDF to CII with AI

> **Experimental feature. Human verification required before any production use.**
>
> Real-world PDF invoices are often messy: scanned at low quality, irregularly formatted, multi-page, or missing fields that EN 16931 requires. AI extraction can make subtle mistakes that automated validators may not catch: wrong tax category codes, transposed amounts, missing seller VAT identifiers, incorrect currency formatting.
>
> Always review the output before sending it to a customer or tax authority. See the [AI conversion notes in the main README](../README.md#ai-powered-pdf-to-cii-conversion-experimental).

The endpoint takes the PDF alone; no extra parameters are needed.

```php
$ch = curl_init('https://api.invoicexml.com/v1/transform/to/cii');
curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_POSTFIELDS     => [
        'file' => new CURLFile('plain-invoice.pdf', 'application/pdf'),
    ],
    CURLOPT_HTTPHEADER     => ['Authorization: Bearer ' . $apiKey],
]);
$xml = curl_exec($ch);
file_put_contents('converted-cii.xml', $xml);
```

[Full example: `ai-convert.php`](./ai-convert.php) | [API reference](https://www.invoicexml.com/docs/api/transform/to/cii)

---

## Render CII as a readable PDF in PHP

CII is a machine format with no visual layer, so nobody can read it without tooling. This endpoint renders the XML into a formatted PDF preview, generated fresh from the structured data rather than extracted from any existing PDF layer, which makes it a quick way to eyeball what your XML actually says. The PDF is for reading only; the XML file remains the authoritative invoice for compliance and tax purposes.

```php
$ch = curl_init('https://api.invoicexml.com/v1/render/cii/to/pdf');
curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_POSTFIELDS     => [
        'file'     => new CURLFile('invoice.xml', 'application/xml'),
        'language' => 'en',   // en, de, or fr
    ],
    CURLOPT_HTTPHEADER     => ['Authorization: Bearer ' . $apiKey],
]);
$pdf = curl_exec($ch);
file_put_contents('invoice-preview.pdf', $pdf);
```

[Full example: `render.php`](./render.php) | [API reference](https://www.invoicexml.com/docs/api/render/cii/to/pdf)

---

## Framework integration

### Laravel

Return a CII invoice from a controller or route closure:

```php
use Illuminate\Support\Facades\Route;

Route::get('/invoices/{id}/cii', function ($id) {
    $pdf = app(CIIService::class)->create($id);
    return response($pdf, 200, [
        'Content-Type'        => 'application/pdf',
        'Content-Disposition' => "attachment; filename=\"invoice-{$id}.pdf\"",
    ]);
});
```

Store the API key in `config/services.php` and read it via `config('services.invoicexml.key')`.

### Symfony

```php
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class InvoiceController
{
    #[Route('/invoices/{id}/cii', name: 'cii_download')]
    public function download(int $id): Response
    {
        $pdf = $this->ciiService->create($id);
        return new Response($pdf, 200, [
            'Content-Type'        => 'application/pdf',
            'Content-Disposition' => "attachment; filename=\"invoice-{$id}.pdf\"",
        ]);
    }
}
```

### WordPress and WooCommerce

Hook into WooCommerce order lifecycle events to generate CII invoices automatically:

```php
add_action('woocommerce_order_status_completed', function ($order_id) {
    $order = wc_get_order($order_id);
    $pdf = create_cii_from_order($order);
    file_put_contents(WP_CONTENT_DIR . "/invoices/{$order_id}.pdf", $pdf);
});
```

The same pattern works in Magento, Drupal Commerce, and PrestaShop.

---

## Common issues

- **`HTTP 401 Unauthorized`**: API key missing or invalid. Generate one at [invoicexml.com/account/authentication](https://www.invoicexml.com/account/authentication) and confirm you are sending `Authorization: Bearer YOUR_API_KEY`. A frequent cause: setting `$apiKey` to the whole `Bearer xxx` value, which sends `Bearer Bearer xxx`. Set the raw key only.
- **`HTTP 400 Bad Request` on Create**: a required field is missing or malformed. Frequent causes: `IssueDate` not in ISO format (`YYYY-MM-DD`), `Currency` not in ISO 4217 (`EUR`, `USD`), country codes not in ISO 3166-1 alpha-2 (`DE`, `FR`).
- **`Class "CURLFile" not found`**: PHP cURL extension is missing. Install with `apt-get install php-curl` (Debian/Ubuntu), `yum install php-curl` (RHEL), or enable the extension in `php.ini`.
- **`SSL certificate problem` errors**: the server's CA bundle is out of date. Update OpenSSL on the host, or set `CURLOPT_CAINFO` to a current `cacert.pem` from curl.se/ca/.
- **Schematron BR-CO-* failures on Validate**: line totals do not match the header total, or tax category and tax percentage are inconsistent. Recompute totals before posting.

---

## Resources

- [Get an InvoiceXML API key](https://www.invoicexml.com/account/authentication)
- [Create CII API reference](https://www.invoicexml.com/docs/api/create/cii)
- [Validate CII API reference](https://www.invoicexml.com/docs/api/validate/cii)
- [Extract JSON API reference](https://www.invoicexml.com/docs/api/extract/json)
- [Extract XML API reference](https://www.invoicexml.com/docs/api/extract/xml)
- [Transform to CII API reference](https://www.invoicexml.com/docs/api/transform/to/cii)
- [PHP cURL documentation](https://www.php.net/manual/en/book.curl.php)
- [Main repository README](../README.md)
