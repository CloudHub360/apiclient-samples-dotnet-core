﻿using System;

namespace Waives.APIClient.Sample.Models
{
    public class Classifier
    {
        public string Name { get; }

        public Classifier(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}