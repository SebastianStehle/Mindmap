// ==========================================================================
// KeyIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class KeyIcon : INodeIcon, IEquatable<KeyIcon>
    {
        public static Func<string, Task<Stream>> StreamProvider;

        private const int DefaultSize = 32;
        private readonly string key;

        public string Key
        {
            get { return key; }
        }

        public string Name
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

        public async Task<Stream> OpenAsStreamAsync()
        {
            try
            {
                if (StreamProvider != null)
                {
                    return await StreamProvider(key);
                }

                return null;
            }
            catch
            {
                return null;
            }
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
