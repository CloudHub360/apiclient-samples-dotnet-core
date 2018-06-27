using System;

namespace Waives.APIClient.Sample.Models
{
    public class Extractor
    {
        public string Name { get; }

        public Extractor(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
        }
    }
}