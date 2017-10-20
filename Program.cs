using System;
using System.Net.Http;
using System.Threading.Tasks;
using CH360.APIClient.Sample.Models;

namespace CH360.APIClient.Sample
{
    class Program
    {
        private static readonly Uri ApiUri = new Uri("https://api.dev.cloudhub360.com");
        private const string ClientId = "<INSERT_YOUR_CLIENTID_HERE>";
        private const string ClientSecret = "<INSERT_YOUR_CLIENTSECRET_HERE>";

        public static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args)).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            const string classifierName = "my-classifier";
            const string samplesZipFile = @"<INSERT-PATH-TO-ZIP-SAMPLES-FILE-HERE>";
            const string documentFile = @"<INSERT-PATH-TO-NEW-DOCUMENT-FILE-HERE";

            using (var httpClient = new HttpClient { BaseAddress = ApiUri })
            {
                var apiClient = new ApiClient(httpClient, ClientId, ClientSecret);

                // STEP 1 - Setup

                // First, either create a classifier if we haven't already
                // var classifier = await apiClient.CreateClassifier(classifierName);

                // or construct a Classifier object manually if we have
                var classifier = new Classifier(classifierName);

                // Now add samples of each document to the classifier so it can learn the characteristics of each document type
                var samplesAdded = await apiClient.AddSamplesFromZipToClassifier(classifier, samplesZipFile);

                foreach (var sample in samplesAdded)
                {
                    Console.WriteLine($"Added sample to '{classifier.Name}' for document type '{sample.DocumentType}' from '{sample.Path}'");
                }

                // STEP 2 - Processing
                // Now we can start to process documents...

                // Create a document from our input file
                var document = await apiClient.CreateDocument(documentFile);
                Console.WriteLine($"Created document with ID '{document.ID}' from file '{documentFile}'");

                // Classify it using our classifier
                var result = await apiClient.ClassifyDocument(document, classifier);
                Console.WriteLine($"Classified document '{document.ID}'. Result:");
                Console.WriteLine($"  Document Type: {result.DocumentType}");
                Console.WriteLine($"  IsConfident: {result.IsConfident}");
            }

            Console.ReadLine();
        }
    }
}