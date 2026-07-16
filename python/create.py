# Create a UN/CEFACT Cross Industry Invoice (CII D16B) using the InvoiceXML API.
# Sends a JSON invoice model and receives the CII XML.
#
# Get API key: https://www.invoicexml.com/account/authentication
# Docs:        https://www.invoicexml.com/docs/api/create/cii
#
# Dependency:  pip install requests

import requests
import sys

# Raw key only, without the "Bearer " prefix (it is added below).
api_key = "YOUR_API_KEY"

# Totals and the VAT breakdown are calculated from the line items.
payload = {
    "invoice": {
        "invoiceNumber": "CII-2026-001",
        "issueDate":     "2026-05-18",
        "currency":      "EUR",
        "seller": {
            "name":              "Acme GmbH",
            "vatIdentifier":     "DE123456789",
            "legalRegistration": {"identifier": "HRB 12345"},
            "postalAddress": {
                "line1":    "Hauptstraße 12",
                "city":     "Berlin",
                "postCode": "10115",
                "country":  "DE",
            },
        },
        "buyer": {
            "name": "Globex SAS",
            "postalAddress": {
                "line1":    "15 rue de Rivoli",
                "city":     "Paris",
                "postCode": "75001",
                "country":  "FR",
            },
        },
        "paymentDetails": {"paymentAccountIdentifier": "DE89370400440532013000"},
        "lines": [
            {
                "quantity":       10,
                "priceDetails":   {"netPrice": 150.00},
                "vatInformation": {"rate": 19.00},
                "item":           {"name": "Senior consulting"},
            },
        ],
    },
}

response = requests.post(
    "https://api.invoicexml.com/v1/create/cii",
    headers={"Authorization": f"Bearer {api_key}"},
    json=payload,
)

if not response.ok:
    sys.stderr.write(f"InvoiceXML API error {response.status_code}: {response.text}\n")
    sys.exit(1)

with open("invoice-cii.xml", "w", encoding="utf-8") as f:
    f.write(response.text)

print(f"Saved invoice-cii.xml ({len(response.text)} chars)")
