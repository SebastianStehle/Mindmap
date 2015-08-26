// ==========================================================================
// PropertiesBag.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using GP.Windows;

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

        public bool HasProperty(string propertyName)
        {
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
    }
}
