using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Waives.APIClient.Sample.Models;
using Waives.APIClient.Sample.Requests;
using Waives.APIClient.Sample.Responses;
using Waives.APIClient.Sample.Responses.Classifier;
using Waives.APIClient.Sample.Responses.Document;
using Waives.APIClient.Sample.Responses.Document.Classify;
using Waives.APIClient.Sample.Responses.Extractor;
using Waives.APIClient.Sample.Responses.Token;
using Waives.APIClient.Sample.Responses.Document.ExtractData;

namespace Waives.APIClient.Sample
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient, string clientId, string clientSecret)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (clientSecret == null) throw new ArgumentNullException(nameof(clientSecret));

            var token = GetTokenAsync(clientId, clientSecret).Result;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Create a new classifier. After creation, samples must be added to the classifier before it can be used to classify documents.
        /// </summary>
        /// <param name="classifierName">The desired name for the classifier</param>
        /// <returns>The created classifier</returns>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#classifier-resource for an introduction to classifiers.</remarks>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#create-classifier for documentation of this endpoint</remarks>
        public async Task<Classifier> CreateClassifier(string classifierName)
        {
            if (classifierName == null) throw new ArgumentNullException(nameof(classifierName));

            var request = new ApiRequest<ClassifierResponse>($"/classifiers/{classifierName}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Post);
            return new Classifier(response.Name);
        }

        /// <summary>
        /// Delete a classifier.
        /// </summary>
        /// <param name="classifierName">The name of the classifier</param>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#delete-classifier for documentation of this endpoint</remarks>
        /// <remarks>This request succeeds even if the specified classifier does not exist.</remarks>
        public async Task DeleteClassifier(string classifierName)
        {
            if (classifierName == null) throw new ArgumentNullException(nameof(classifierName));

            var request = new ApiRequest<EmptyResponse>($"/classifiers/{classifierName}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Delete);
        }

        /// <summary>
        /// Add a set of sample documents, labelled with their document types, and saved in a ZIP file to a classifier.
        /// </summary>
        /// <param name="classifier">The classifier to add the samples to</param>
        /// <param name="zipFilePath">The path to the ZIP file</param>
        /// <returns>Details of all the samples added from the ZIP file</returns>
        /// /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#add-samples-from-zip-file for documentation of this endpoint</remarks>
        public async Task<IEnumerable<ZipSample>> AddSamplesFromZipToClassifier(Classifier classifier, string zipFilePath)
        {
            if (classifier == null) throw new ArgumentNullException(nameof(classifier));
            if (zipFilePath == null) throw new ArgumentNullException(nameof(zipFilePath));

            using (var stream = File.OpenRead(zipFilePath))
            {
                var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/zip");

                var request = new ApiRequest<ClassifierAddSamplesResponse>($"/classifiers/{classifier.Name}/samples", _httpClient);
                var response = await request.IssueBinary(content, HttpMethod.Post);
                return response
                    .Samples
                    .Select(s => new ZipSample
                    {
                        DocumentType = s.DocumentType,
                        Path = s.Path
                    });
            }
        }

        /// <summary>
        /// Add a single sample file to a classifier
        /// </summary>
        /// <param name="classifier">The name of the Classifier to add the sample to</param>
        /// <param name="sampleFilePath">The path to the sample's file</param>
        /// <param name="documentType">The document type of the sample</param>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#add-single-sample-file for documentation of this endpoint</remarks>
        public async Task AddSingleSampleToClassifier(Classifier classifier, string sampleFilePath, string documentType)
        {
            if (classifier == null) throw new ArgumentNullException(nameof(classifier));
            if (sampleFilePath == null) throw new ArgumentNullException(nameof(sampleFilePath));
            if (documentType == null) throw new ArgumentNullException(nameof(documentType));

            using (var stream = File.OpenRead(sampleFilePath))
            {
                var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/zip");

                var request = new ApiRequest<ClassifierAddSamplesResponse>($"/classifiers/{classifier.Name}/sample/{documentType}", _httpClient);
                await request.IssueBinary(content, HttpMethod.Post);
            }
        }

        /// <summary>
        /// Get the details of an existing classifier
        /// </summary>
        /// <param name="classifierName">The name of the classifier</param>
        /// <returns>Details of the classifier</returns>
        public async Task<Classifier> GetClassifierDetails(string classifierName)
        {
            if (classifierName == null) throw new ArgumentNullException(nameof(classifierName));

            var request = new ApiRequest<ClassifierResponse>($"/classifiers/{classifierName}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Get);
            return new Classifier(response.Name);
        }

        /// <summary>
        /// Create a new document and add the supplied file to it.
        /// Remember to delete the document when you are finished with it, otherwise you will fill
        /// your quota of documents.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The ID of the created document</returns>
        public async Task<Document> CreateDocument(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            using (var stream = File.OpenRead(filePath))
            {
                var content = new StreamContent(stream);
                var request = new ApiRequest<DocumentResponse>($"/documents", _httpClient);
                var response = await request.IssueBinary(content, HttpMethod.Post);

                return new Document
                {
                    ID = response.Id,
                    Files = response.EmbeddedResources.Files.Select(f => new DocumentFile
                    {
                        ID = f.ID,
                        FileType = f.FileType,
                        SHA256 = f.SHA256,
                        Size = f.Size
                    }).ToList()
                };
            }
        }

        /// <summary>
        /// Delete the specified document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task DeleteDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var request = new ApiRequest<EmptyResponse>($"/documents/{document.ID}", _httpClient);
            await request.Issue(request, HttpMethod.Delete);
        }

        /// <summary>
        /// Classify the specified document and return its document type.
        /// </summary>
        /// <param name="document">A document created with the CreateDocument method</param>
        /// <param name="classifier">The classifier to use</param>
        /// <returns>The results of the classification</returns>
        public async Task<ClassificationResult> ClassifyDocument(Document document, Classifier classifier)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (classifier == null) throw new ArgumentNullException(nameof(classifier));

            var request = new ApiRequest<ClassifyDocumentResponse>($"/documents/{document.ID}/classify/{classifier.Name}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Post);

            return new ClassificationResult
            {
                DocumentType = response.Results.DocumentType,
                IsConfident = response.Results.IsConfident,
                Scores = response.Results
                    .Scores
                    .Select(s => new ClassificationResultDocumentTypeScore
                    {
                        DocumentType = s.DocumentType,
                        Score = s.Score

                    }).ToList()
            };
        }

        /// <summary>
        /// Create a new extractor from an extractor configuration file
        /// </summary>
        /// <param name="extractorName">The desired name for the extractor</param>
        /// <param name="extractorConfigurationFilePath">The path to the extractor configuration file</param>
        /// <returns>The created extractor</returns>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#create-extractor for documentation of this endpoint</remarks>
        public async Task<Extractor> CreateExtractor(string extractorName, string extractorConfigurationFilePath)
        {
            if (extractorName == null) throw new ArgumentNullException(nameof(extractorName));
            if (extractorConfigurationFilePath == null) throw new ArgumentNullException(nameof(extractorConfigurationFilePath));

            using (var stream = File.OpenRead(extractorConfigurationFilePath))
            {
                var content = new StreamContent(stream);
                var request = new ApiRequest<ExtractorResponse>($"/extractors/{extractorName}", _httpClient);
                var response = await request.IssueBinary(content, HttpMethod.Post);

                return new Extractor(response.Name);
            }
        }

        /// <summary>
        /// Delete an extractor
        /// </summary>
        /// <param name="extractorName">The name of the extractor</param>
        /// <remarks>See https://cloudhub360.readme.io/v1.0/reference#delete-extractor for documentation of this endpoint</remarks>
        /// <remarks>This request succeeds even if the specified extractor does not exist.</remarks>
        public async Task DeleteExtractor(string extractorName)
        {
            if (extractorName == null) throw new ArgumentNullException(nameof(extractorName));

            var request = new ApiRequest<EmptyResponse>($"/extractors/{extractorName}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Delete);
        }

        /// <summary>
        /// Extract data from the specified document using an extractor
        /// </summary>
        /// <param name="document">A document created with the CreateDocument method</param>
        /// <param name="extractor">The extractor to use</param>
        /// <returns>The results of the extraction</returns>
        public async Task<ExtractDataResponse> ExtractData(Document document, Extractor extractor)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (extractor == null) throw new ArgumentNullException(nameof(extractor));

            var request = new ApiRequest<ExtractDataResponse>($"/documents/{document.ID}/extract/{extractor.Name}", _httpClient);
            var response = await request.Issue(request, HttpMethod.Post);

            return response;
        }

        /// <summary>
        /// Extract data from the specified document using an extractor
        /// </summary>
        /// <param name="document">A document created with the CreateDocument method</param>
        /// <param name="extractor">The extractor to use</param>
        /// <returns>A JObject containing the deserialising extraction results response</returns>
        public async Task<JObject> ExtractDataToJObject(Document document, Extractor extractor)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (extractor == null) throw new ArgumentNullException(nameof(extractor));

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/documents/{document.ID}/extract/{extractor.Name}"))
            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseJson);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
                throw new ApiException(error.Message, response);
            }
        }

        private async Task<string> GetTokenAsync(string clientId, string clientSecret)
        {
            var request = new ApiRequest<GetTokenResponse>("/oauth/token", _httpClient);
            var formValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            var response = await request.IssueForm(formValues);
            return response.AccessToken;
        }
    }
}