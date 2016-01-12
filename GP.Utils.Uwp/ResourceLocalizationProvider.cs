// ==========================================================================
// ResourceLocalizationProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace GP.Utils
{
    /// <summary>
    /// Provides strings from resources.
    /// </summary>
    public class ResourceLocalizationProvider : ILocalizationProvider
    {
        /// <summary>
        /// Gets the string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="T:Systen.ArgumentException"><paramref name="key" /> is null or empty.</exception>
        public string GetString(string key, CultureInfo culture)
        {
            ResourceLoader resourceLoader = new ResourceLoader();

            string result = resourceLoader.GetString(key);

            return result;
        }
    }
}
