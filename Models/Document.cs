using System.Collections.Generic;

namespace Waives.APIClient.Sample.Models
{
    public class Document
    {
        public string ID { get; set; }

        public List<DocumentFile> Files { get; set; }
    }
}