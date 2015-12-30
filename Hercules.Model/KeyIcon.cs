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
        private const int DefaultSize = 32;
        private const string PropertyKey = "Key";
        private const string PropertyKeyOld = "IconKey";
        private readonly string key;

        public string Key
        {
            get { return key; }
        }

        public int PixelWidth
        {
            get { return DefaultSize; }
        }

        public int PixelHeight
        {
            get { return DefaultSize; }
        }

        public KeyIcon(string key)
        {
            Guard.NotNullOrEmpty(key, nameof(key));

            this.key = key;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey, key);
        }

        public static INodeIcon TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            string key;

            if (properties.TryParseString(PropertyKey, out key) ||
                properties.TryParseString(PropertyKeyOld, out key))
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    return new KeyIcon(key);
                }
            }

            return null;
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
