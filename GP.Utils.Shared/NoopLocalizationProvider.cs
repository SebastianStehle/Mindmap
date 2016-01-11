// ==========================================================================
// NoopLocalizationProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;

namespace GP.Utils
{
    /// <summary>
    /// Does nothing.
    /// </summary>
    public sealed class NoopLocalizationProvider : ILocalizationProvider
    {
        /// <summary>
        /// Gets the string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="key" /> is null or empty.</exception>
        public string GetString(string key, CultureInfo culture)
        {
            return key;
        }
    }
}
