// Validate a CII XML file using the InvoiceXML REST API. Runs the CII D16B XSD,
// then the business rules matching the profile the document declares in BT-24:
// the EN 16931 Schematron for plain core documents, or the Factur-X / ZUGFeRD
// per-profile rules when the document declares one of those profiles.
//
// Get your API key at: https://www.invoicexml.com/account/authentication
// Full API reference:  https://www.invoicexml.com/docs/api/validate/cii
//
// Required NuGet package:
//   dotnet add package Flurl.Http

using Flurl.Http;

public static class ValidateCii
{
    private const string Endpoint = "https://api.invoicexml.com/v1/validate/cii";

    public static async Task<string> RunAsync(string xmlPath = "invoice.xml")
    {
        // Get an InvoiceXML API key at: https://www.invoicexml.com/account/authentication
        // Raw key only, without the "Bearer " prefix (WithOAuthBearerToken adds it).
        var apiKey = Environment.GetEnvironmentVariable("INVOICEXML_API_KEY")
            ?? throw new InvalidOperationException(
                "Set INVOICEXML_API_KEY environment variable. " +
                "Get an InvoiceXML API key at " +
                "https://www.invoicexml.com/account/authentication");

        if (!File.Exists(xmlPath))
            throw new FileNotFoundException($"Input XML not found: {xmlPath}");

        try
        {
            // POST the XML as multipart/form-data. The profile is detected from the
            // document itself, so no profile parameter is needed. The response is
            // a JSON validation report. Response structure:
            // https://www.invoicexml.com/docs/api/validate/cii
            var report = await Endpoint
                .WithOAuthBearerToken(apiKey)
                .PostMultipartAsync(mp => mp
                    .AddFile("file", xmlPath, "application/xml"))
                .ReceiveString();

            Console.WriteLine(report);
            return report;
        }
        catch (FlurlHttpException ex)
        {
            var body = await ex.GetResponseStringAsync();
            Console.Error.WriteLine($"InvoiceXML API error {(int?)ex.StatusCode}: {body}");
            throw;
        }
    }
}
