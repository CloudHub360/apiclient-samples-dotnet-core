using System.Collections.Generic;

namespace Waives.APIClient.Sample.Models
{
    public class ClassificationResult
    {
        public string DocumentType { get; set; }

        public bool IsConfident { get; set; }

        public IEnumerable<ClassificationResultDocumentTypeScore> Scores { get; set; }
    }
}