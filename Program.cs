using System;
using System.Net.Http;
using System.Threading.Tasks;
using Waives.APIClient.Sample.Models;
using Newtonsoft.Json.Linq;

namespace Waives.APIClient.Sample
{
    class Program
    {
        // Before running this code you will need to specify your API Client Id & Secret in Settings.Cs.

        // This code is intended as an illustration and simple starting point rather than resilient high-throughput
        // production-ready code. In particular:
        //  - there is no parallelisation of requests (which can give x10 overall throughput)
        //  - no attempt is made to handle & retry transient network and service errors

        // The code in this class depends on values in Settings.cs for paths to samples, configuration and document files.
        // You will need to specify these values or disable relevant sections of code in the class before you can run
        // successfully.
        private static readonly Uri ApiUri = new Uri("https://api.waives.io");

        public static void Main(string[] args)
        {

            try
            {
                Task.Run(() => MainAsync(args)).Wait();
            }
            catch (AggregateException e)
            {
                foreach (var exception in e.Flatten().InnerExceptions)
                {
                    Console.WriteLine(exception);
                }
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static async Task MainAsync(string[] args)
        {
            using (var httpClient = new HttpClient { BaseAddress = ApiUri })
            {
                var apiClient = new ApiClient(httpClient, Settings.ClientId, Settings.ClientSecret);

                // Create a classifier and samples to it from a samples zip file.
                // This only needs doing once and is typically done during project deployment.
                await CreateClassifier(apiClient, Settings.ClassifierName, Settings.SamplesZipFile);

                // Use the trained classifier to classify a document.
                await ClassifyDocument(apiClient, Settings.ClassifierName, Settings.DocumentFile);

                // Create an extractor from an extractor configuration file (.fpxlc). You can skip this if you are using
                // a Waives-provided extractor such as "waives.invoices.gb"
                await CreateExtractor(apiClient, Settings.ExtractorName, Settings.ExtractorConfigurationFile);

                // Use the extractor to extract data from a document
                await ExtractData(apiClient, Settings.ExtractorName, Settings.ExtractorDocumentFile);
            }
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
            try
            {

                // Classify it using our classifier
                var result = await apiClient.ClassifyDocument(document, classifier);
                Console.WriteLine($"Classified document '{document.ID}'. Result:");
                Console.WriteLine($"  Document Type: {result.DocumentType}");
                Console.WriteLine($"  IsConfident: {result.IsConfident}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                await apiClient.DeleteDocument(document);
                Console.WriteLine($"Deleted document with ID '{document.ID}'");
            }

        }

        private static async Task CreateExtractor(ApiClient apiClient, string extractorName, string extractorConfigurationFile)
        {
            // Delete any existing extractor with this name, if there is one
            await apiClient.DeleteExtractor(extractorName);

            // Create a new extractor using the configuration file
            await apiClient.CreateExtractor(extractorName, extractorConfigurationFile);
            Console.WriteLine($"Created extractor {extractorName} from {extractorConfigurationFile}");
        }

        private static async Task ExtractData(ApiClient apiClient, string extractorName, string documentFile)
        {
            var extractor = new Extractor(extractorName);

            // Create a document to extract data from
            var document = await apiClient.CreateDocument(documentFile);
            Console.WriteLine($"Created document with ID '{document.ID}' from file '{documentFile}'");

            try
            {
                // You can extract data to a response type and get any result data you need
                // var extractDataResponse = await apiClient.ExtractData(document, extractor);

                // Or, instead of deserialising to a response type, you can
                // use JSONPath to select just the data we need from the response.

                // Here, we are selecting an enumerable of all the field names then
                // getting the text for every field
                var jObject = await apiClient.ExtractDataToJObject(document, extractor);
                var fieldNameTokens = jObject.SelectTokens("field_results[*].field_name");

                foreach (var token in fieldNameTokens)
                {
                    var fieldName = token.Value<string>();
                    var fieldText = string.Empty;
                    
                    // Get the result for the specified field name
                    var fieldResultToken = jObject
                        .SelectToken($"field_results[?(@.field_name=='{fieldName}')].result");

                    // If the result is not null then get its text property
                    if (fieldResultToken.HasValues)
                    {
                        fieldText = fieldResultToken
                            .SelectToken(".text")
                            .ToString();
                    }
                        
                    Console.WriteLine($"{fieldName}: {fieldText}");                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                await apiClient.DeleteDocument(document);
                Console.WriteLine($"Deleted document with ID '{document.ID}'");
            }
        }
    }
}