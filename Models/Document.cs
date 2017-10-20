using System.Collections.Generic;

namespace CH360.APIClient.Sample.Models
{
    public class Document
    {
        public string ID { get; set; }

        public List<DocumentFile> Files { get; set; }
    }
}