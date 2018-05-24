namespace Waives.APIClient.Sample
{
    static class Settings
    {
        public const string ClientId = "<INSERT_YOUR_CLIENTID_HERE>";
        public const string ClientSecret = "<INSERT_YOUR_CLIENTSECRET_HERE>";

        // The name of the classifier that will be created
        public const string ClassifierName = "my-classifier";

        // The path to ZIP file containing a valid set of sample documents to build a classifier from
        public const string SamplesZipFile = @"<INSERT-PATH-TO-ZIP-SAMPLES-FILE-HERE>";

        // The path to an example document that will be classified
        public const string DocumentFile = @"<INSERT-PATH-TO-DOCUMENT-FILE-HERE>";

        // The name of the extractor that will be created
        public const string ExtractorName = "my-extractor";

        // The path to an extractor configuration file (*.fpxlc)
        public const string ExtractorConfigurationFile = @"<INSERT-PATH-TO-CONFIGURATION-FILE-HERE";

        // The path to an example document that will have data extracted from it
        public const string ExtractorDocumentFile = @"<INSERT-PATH-TO-DOCUMENT-FILE-HERE";
    }
}