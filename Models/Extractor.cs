using System;

namespace CH360.APIClient.Sample.Models
{
    public class Extractor
    {
        public string Name { get; }

        public Extractor(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}