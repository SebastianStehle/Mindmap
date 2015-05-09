// ==========================================================================
// CommandProperties.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace RavenMind.Model
{
    public sealed class CommandProperties
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public void Set(string key, object value)
        {
            properties[key] = value;
        }

        public T Get<T>(string key)
        {
            T result = default(T);

            if (key != null)
            {
                object temp = null;

                if (properties.TryGetValue(key, out temp) && temp is T)
                {
                    result = (T)temp;
                }
            }

            return result;
        }
    }
}
