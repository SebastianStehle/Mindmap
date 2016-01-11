// ==========================================================================
// LocalizationManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;

namespace GP.Utils
{
    /// <summary>
    /// Provides access to localized resources.
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// Gets or sets the current resource provider.
        /// </summary>
        /// <value>
        /// The current resource provider.
        /// </value>
        public static ILocalizationProvider Provider { get; set; }

        /// <summary>
        /// Gets the string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <returns>The localized string with the specified key.</returns>
        /// <exception cref="ArgumentException"><paramref name="key"/> is null or empty.</exception>
        public static string GetString(string key)
        {
            return GetString(CultureInfo.CurrentCulture, key);
        }

        /// <summary>
        /// Gets the string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        /// <exception cref="ArgumentException"><paramref name="key" /> is null or empty.</exception>
        public static string GetString(CultureInfo culture, string key)
        {
            Guard.NotNullOrEmpty(key, nameof(key));
            Guard.NotNull(culture, nameof(culture));

            ILocalizationProvider provider = Provider;

            if (provider == null)
            {
                throw new InvalidOperationException("Provider is not specified.");
            }

            string resourceString = provider.GetString(key, culture);

            if (resourceString == null)
            {
                throw new ArgumentException("Resource not found", nameof(key));
            }

            return resourceString;
        }

        /// <summary>
        /// Gets the formatted string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The localized string with the specified key.</returns>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        public static string GetFormattedString(string key, params object[] args)
        {
            return GetFormattedString(CultureInfo.CurrentCulture, key, args);
        }

        /// <summary>
        /// Gets the formatted string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The localized string with the specified key.</returns>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        public static string GetFormattedString(CultureInfo culture, string key, params object[] args)
        {
            Guard.NotNullOrEmpty(key, nameof(key));
            Guard.NotNull(culture, nameof(culture));

            ILocalizationProvider provider = Provider;

            if (provider == null)
            {
                throw new InvalidOperationException("Provider is not specified.");
            }

            string resourceString = provider.GetString(key, culture);

            if (resourceString == null)
            {
                throw new ArgumentException("Resource not found", nameof(key));
            }

            return string.Format(CultureInfo.CurrentCulture, resourceString, args);
        }

        /// <summary>
        /// Tries to get the string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="fallback">The fallback string that is used when no resource could be found.</param>
        /// <returns>The localized string with the specified key.</returns>
        /// <exception cref="ArgumentException"><paramref name="key"/> is null or empty.</exception>
        public static string TryGetString(string key, string fallback = null)
        {
            return TryGetString(CultureInfo.CurrentCulture, key, fallback);
        }

        /// <summary>
        /// Tries to get the string with the specified key.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="fallback">The fallback string that is used when no resource could be found.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="key" /> is null or empty.</exception>
        public static string TryGetString(CultureInfo culture, string key, string fallback = null)
        {
            Guard.NotNullOrEmpty(key, nameof(key));
            Guard.NotNull(culture, nameof(culture));

            ILocalizationProvider provider = Provider;

            string resourceString = null;

            if (provider != null)
            {
                try
                {
                    resourceString = provider.GetString(key, culture);
                }
                catch
                {
                    resourceString = null;
                }
            }

            return resourceString ?? (fallback ?? key.SeparateByUpperLetters());
        }

        /// <summary>
        /// Tries to get the formatted string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The localized string with the specified key.</returns>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        public static string TryGetFormattedString(string key, params object[] args)
        {
            return TryGetFormattedString(CultureInfo.CurrentCulture, key, null, args);
        }

        /// <summary>
        /// Tries to get the formatted string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="fallback">The fallback string that is used when no resource could be found.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        public static string TryGetFormattedString(string key, string fallback, object[] args)
        {
            return TryGetFormattedString(CultureInfo.CurrentCulture, key, fallback, args);
        }

        /// <summary>
        /// Tries to get the formatted string with the specified key.
        /// </summary>
        /// <param name="key">The key to identity the resource. Cannot be null or empty.</param>
        /// <param name="fallback">The fallback string that is used when no resource could be found.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// The localized string with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="ArgumentException">Resource key cannot be null or empty.;key</exception>
        /// <exception cref="InvalidOperationException">Provider is not specified.</exception>
        public static string TryGetFormattedString(CultureInfo culture, string key, string fallback, object[] args)
        {
            Guard.NotNullOrEmpty(key, nameof(key));
            Guard.NotNull(culture, nameof(culture));

            ILocalizationProvider provider = Provider;

            if (provider != null)
            {
                try
                {
                    string resourceString = provider.GetString(key, culture) ?? (fallback ?? key.SeparateByUpperLetters());

                    return resourceString != null ? string.Format(CultureInfo.CurrentCulture, resourceString, args) : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
