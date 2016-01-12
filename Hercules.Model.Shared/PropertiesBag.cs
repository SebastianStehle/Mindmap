// ==========================================================================
// PropertiesBag.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class PropertiesBag
    {
        private readonly Dictionary<string, PropertyValue> internalDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);

        public int Count
        {
            get { return internalDictionary.Count; }
        }

        public IReadOnlyDictionary<string, PropertyValue> Properties
        {
            get { return internalDictionary; }
        }

        public IEnumerable<string> PropertyNames
        {
            get { return internalDictionary.Keys; }
        }

        public PropertyValue this[string key]
        {
            get { return internalDictionary[key]; }
        }

        public bool Contains(string propertyName)
        {
            Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

            return internalDictionary.ContainsKey(propertyName);
        }

        public void Set(string propertyName, object value)
        {
            Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

            internalDictionary[propertyName] = new PropertyValue(value);
        }

        public bool Remove(string propertyName)
        {
            Guard.NotNullOrEmpty(propertyName, nameof(propertyName));

            return internalDictionary.Remove(propertyName);
        }

        public bool Rename(string oldPropertyName, string newPropertyName)
        {
            Guard.NotNullOrEmpty(oldPropertyName, nameof(oldPropertyName));
            Guard.NotNullOrEmpty(newPropertyName, nameof(newPropertyName));

            if (internalDictionary.ContainsKey(newPropertyName))
            {
                throw new ArgumentException($"An property with the key '{newPropertyName}' already exists.", newPropertyName);
            }

            if (string.Equals(oldPropertyName, newPropertyName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The property names '{newPropertyName}' are equal.", newPropertyName);
            }

            PropertyValue property;

            if (internalDictionary.TryGetValue(oldPropertyName, out property))
            {
                internalDictionary[newPropertyName] = property;
                internalDictionary.Remove(oldPropertyName);

                return true;
            }

            return false;
        }
    }
}
