using System;
using System.Net.Http;
using System.Threading.Tasks;
using CH360.APIClient.Sample.Models;
using Newtonsoft.Json.Linq;

namespace CH360.APIClient.Sample
{
    class Program
    {
        private static readonly Uri ApiUri = new Uri("https://api.cloudhub360.com");
        private const string ClientId = "<INSERT_YOUR_CLIENTID_HERE>";
        private const string ClientSecret = "<INSERT_YOUR_CLIENTSECRET_HERE>";

        // The name of the classifier that will be created
        const string ClassifierName = "my-classifier";

        // The path to ZIP file containing a valid set of sample documents to build a classifier from
        const string SamplesZipFile = @"<INSERT-PATH-TO-ZIP-SAMPLES-FILE-HERE>";

        // The path to an example document that will be classified
        const string DocumentFile = @"<INSERT-PATH-TO-DOCUMENT-FILE-HERE>";

        // The name of the extractor that will be created
        const string ExtractorName = "my-extractor";

        // The path to an extractor configuration file (*.fpxlc)
        const string ExtractorConfigurationFile = @"<INSERT-PATH-TO-CONFIGURATION-FILE-HERE";

        // The path to an example document that will have data extracted from it
        const string ExtractorDocumentFile = @"<INSERT-PATH-TO-DOCUMENT-FILE-HERE";

        public static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args)).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            using (var httpClient = new HttpClient { BaseAddress = ApiUri })
            {
                var apiClient = new ApiClient(httpClient, ClientId, ClientSecret);

                // Create a classifier and samples to it from a samples zip file.
                // This only needs doing once and is typically done during project deployment.
                await CreateClassifier(apiClient, ClassifierName, SamplesZipFile);

                // Use the trained classifier to classify a document.
                await ClassifyDocument(apiClient, ClassifierName, DocumentFile);

                // Create an extractor and use it to extract data from a document
                await ExtractData(apiClient);
            }

            Console.ReadLine();
        }

        private static async Task CreateClassifier(ApiClient apiClient, string classifierName, string samplesZipFile)
        {
            // Delete any existing classifier with this name, if there is one
            await apiClient.DeleteClassifier(classifierName);

            // Create a new classifier
            var classifier = await apiClient.CreateClassifier(classifierName);

            // Now add samples of each document to the classifier so it can learn the characteristics of each document type
            Console.WriteLine($"Adding samples to '{classifier.Name}'...");
            var samplesAdded = await apiClient.AddSamplesFromZipToClassifier(classifier, samplesZipFile);

            foreach (var sample in samplesAdded)
            {
                Console.WriteLine($"Added sample to '{classifier.Name}' for document type '{sample.DocumentType}' from '{sample.Path}'");
            }
        }

        private static async Task ClassifyDocument(ApiClient apiClient, string classifierName, string documentFile)
        {
            var classifier = new Classifier(classifierName);

            // Create a document from our input file
            var document = await apiClient.CreateDocument(documentFile);
            Console.WriteLine($"Created document with ID '{document.ID}' from file '{documentFile}'");

            // Classify it using our classifier
            var result = await apiClient.ClassifyDocument(document, classifier);
            Console.WriteLine($"Classified document '{document.ID}'. Result:");
            Console.WriteLine($"  Document Type: {result.DocumentType}");
            Console.WriteLine($"  IsConfident: {result.IsConfident}");
        }

        private static async Task ExtractData(ApiClient apiClient)
        {
            // Delete any existing extractor with this name, if there is one
            await apiClient.DeleteExtractor(ExtractorName);

            // Create a new extractor using the configuration file
            var extractor = await apiClient.CreateExtractor(ExtractorName, ExtractorConfigurationFile);

            // Create a document to extract data from
            var document = await apiClient.CreateDocument(ExtractorDocumentFile);

            // Extract data to a response type
            var extractDataResponse = await apiClient.ExtractData(document, extractor);
            foreach (var fieldResult in extractDataResponse.FieldResults)
            {
                Console.WriteLine($"{fieldResult.FieldName}: {fieldResult?.Result?.Text} Rejected={fieldResult?.Result?.Rejected}");
            }

            // Instead of deserialising to a response type, you can
            // use JSONPath to select just the data we need from the response.

            // Here, we are selecting an enumerable of all the field names then
            // getting the text for every field
            var jObject = await apiClient.ExtractDataToJObject(document, extractor);
            var fieldNameTokens = jObject.SelectTokens("field_results[*].field_name");

            foreach (var token in fieldNameTokens)
            {
                var fieldName = token.Value<string>();

                var text = jObject
                    .SelectToken($"field_results[?(@.field_name=='{fieldName}')].result.text");

                Console.WriteLine($"{token}: {text}");
            }
        }
    }
}