// Create a UN/CEFACT Cross Industry Invoice (CII D16B, EN 16931 compliant) using
// the InvoiceXML REST API. Sends a JSON invoice model and receives the XML.
//
// InvoiceXML:          https://www.invoicexml.com/
// Get your API key at: https://www.invoicexml.com/account/authentication
// Full API reference:  https://www.invoicexml.com/docs/api/create/cii
//
// Required NuGet package:
//   dotnet add package Flurl.Http

using Flurl.Http;

public static class CreateCii
{
    private const string Endpoint = "https://api.invoicexml.com/v1/create/cii";

    public static async Task<string> RunAsync(string outputPath = "invoice-cii.xml")
    {
        // Get an InvoiceXML API key at:
        // https://www.invoicexml.com/account/authentication
        // Raw key only, without the "Bearer " prefix (WithOAuthBearerToken adds it).
        var apiKey = Environment.GetEnvironmentVariable("INVOICEXML_API_KEY")
            ?? throw new InvalidOperationException(
                "Set INVOICEXML_API_KEY environment variable. " +
                "Get an InvoiceXML API key at " +
                "https://www.invoicexml.com/account/authentication");

        // Minimal CII invoice payload; totals and the VAT breakdown are calculated
        // from the line items. Full field reference:
        // https://www.invoicexml.com/docs/api/create/cii
        var payload = new
        {
            invoice = new
            {
                invoiceNumber = "CII-2026-001",
                issueDate     = "2026-05-18",
                currency      = "EUR",
                seller = new
                {
                    name              = "Acme GmbH",
                    vatIdentifier     = "DE123456789",
                    legalRegistration = new { identifier = "HRB 12345" },
                    postalAddress = new
                    {
                        line1    = "Hauptstraße 12",
                        city     = "Berlin",
                        postCode = "10115",
                        country  = "DE"
                    }
                },
                buyer = new
                {
                    name = "Globex SAS",
                    postalAddress = new
                    {
                        line1    = "15 rue de Rivoli",
                        city     = "Paris",
                        postCode = "75001",
                        country  = "FR"
                    }
                },
                paymentDetails = new { paymentAccountIdentifier = "DE89370400440532013000" },
                lines = new[]
                {
                    new
                    {
                        quantity       = 10,
                        priceDetails   = new { netPrice = 150.00m },
                        vatInformation = new { rate = 19.00m },
                        item           = new { name = "Senior consulting" }
                    }
                }
            }
        };

        try
        {
            var xml = await Endpoint
                .WithOAuthBearerToken(apiKey)
                .PostJsonAsync(payload)
                .ReceiveString();

            await File.WriteAllTextAsync(outputPath, xml);
            Console.WriteLine($"CII invoice saved: {outputPath} ({xml.Length:N0} chars)");
            return xml;
        }
        catch (FlurlHttpException ex)
        {
            var body = await ex.GetResponseStringAsync();
            Console.Error.WriteLine($"InvoiceXML API error {(int?)ex.StatusCode}: {body}");
            throw;
        }
    }
}
