// Create a UN/CEFACT Cross Industry Invoice (CII D16B) using the InvoiceXML API.
// Sends a JSON invoice model and receives the CII XML.
//
// Get API key: https://www.invoicexml.com/account/authentication
// Docs:        https://www.invoicexml.com/docs/api/create/cii
//
// Requires Java 15+ (uses text blocks).
// Dependency: com.squareup.okhttp3:okhttp:4.12.0

import okhttp3.*;
import java.nio.file.Files;
import java.nio.file.Paths;

public class Create {
    public static void main(String[] args) throws Exception {
        // Raw key only, without the "Bearer " prefix (it is added below).
        String apiKey = "YOUR_API_KEY";

        // Totals and the VAT breakdown are calculated from the line items.
        String json = """
            {
              "invoice": {
                "invoiceNumber": "CII-2026-001",
                "issueDate": "2026-05-18",
                "currency": "EUR",
                "seller": {
                  "name": "Acme GmbH",
                  "vatIdentifier": "DE123456789",
                  "legalRegistration": { "identifier": "HRB 12345" },
                  "postalAddress": {
                    "line1": "Hauptstraße 12",
                    "city": "Berlin",
                    "postCode": "10115",
                    "country": "DE"
                  }
                },
                "buyer": {
                  "name": "Globex SAS",
                  "postalAddress": {
                    "line1": "15 rue de Rivoli",
                    "city": "Paris",
                    "postCode": "75001",
                    "country": "FR"
                  }
                },
                "paymentDetails": { "paymentAccountIdentifier": "DE89370400440532013000" },
                "lines": [
                  {
                    "quantity": 10,
                    "priceDetails": { "netPrice": 150.00 },
                    "vatInformation": { "rate": 19.00 },
                    "item": { "name": "Senior consulting" }
                  }
                ]
              }
            }
            """;

        RequestBody body = RequestBody.create(json, MediaType.parse("application/json"));

        Request request = new Request.Builder()
            .url("https://api.invoicexml.com/v1/create/cii")
            .header("Authorization", "Bearer " + apiKey)
            .post(body)
            .build();

        try (Response response = new OkHttpClient().newCall(request).execute()) {
            String xml = response.body().string();
            if (!response.isSuccessful()) {
                System.err.println("InvoiceXML API error " + response.code() + ": " + xml);
                System.exit(1);
            }
            Files.write(Paths.get("invoice-cii.xml"), xml.getBytes("UTF-8"));
            System.out.println("Saved invoice-cii.xml (" + xml.length() + " chars)");
        }
    }
}
