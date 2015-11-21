// ==========================================================================
// KeyIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Windows;

namespace Hercules.Model
{
    public sealed class KeyIcon : INodeIcon, IEquatable<KeyIcon>
    {
        private const string PropertyKeyForKey = "Key";
        private readonly string key;

        public string Key
        {
            get { return key; }
        }

        public KeyIcon(string key)
        {
            Guard.NotNullOrEmpty(key, nameof(key));

            this.key = key;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKeyForKey, key);
        }

        public static INodeIcon TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            INodeIcon result = null;

            if (properties.Contains(PropertyKeyForKey))
            {
                string key = properties[PropertyKeyForKey].ToString();

                if (!string.IsNullOrWhiteSpace(key))
                {
                    result = new KeyIcon(key);
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyIcon);
        }

        public bool Equals(INodeIcon other)
        {
            return Equals(other as KeyIcon);
        }

        public bool Equals(KeyIcon other)
        {
            return other != null && other.key == key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}
