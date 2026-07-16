<?php
// Create a UN/CEFACT Cross Industry Invoice (CII D16B) using the InvoiceXML API.
// Sends a JSON invoice model and receives the CII XML.
//
// Get API key: https://www.invoicexml.com/account/authentication
// Docs:        https://www.invoicexml.com/docs/api/create/cii

// Raw key only, without the "Bearer " prefix (it is added below).
$apiKey = 'YOUR_API_KEY';

// Totals and the VAT breakdown are calculated from the line items.
$payload = [
    'invoice' => [
        'invoiceNumber' => 'CII-2026-001',
        'issueDate'     => '2026-05-18',
        'currency'      => 'EUR',
        'seller' => [
            'name'              => 'Acme GmbH',
            'vatIdentifier'     => 'DE123456789',
            'legalRegistration' => ['identifier' => 'HRB 12345'],
            'postalAddress'     => [
                'line1'    => 'Hauptstraße 12',
                'city'     => 'Berlin',
                'postCode' => '10115',
                'country'  => 'DE',
            ],
        ],
        'buyer' => [
            'name' => 'Globex SAS',
            'postalAddress' => [
                'line1'    => '15 rue de Rivoli',
                'city'     => 'Paris',
                'postCode' => '75001',
                'country'  => 'FR',
            ],
        ],
        'paymentDetails' => ['paymentAccountIdentifier' => 'DE89370400440532013000'],
        'lines' => [
            [
                'quantity'       => 10,
                'priceDetails'   => ['netPrice' => 150.00],
                'vatInformation' => ['rate' => 19.00],
                'item'           => ['name' => 'Senior consulting'],
            ],
        ],
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

$xml    = curl_exec($ch);
$status = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($status !== 200) {
    fwrite(STDERR, "InvoiceXML API error $status: $xml\n");
    exit(1);
}

file_put_contents('invoice-cii.xml', $xml);
echo "Saved invoice-cii.xml (" . strlen($xml) . " bytes)\n";
