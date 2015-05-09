// ==========================================================================
// IOutlineGenerator.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    /// <summary>
    /// Generates the outline for a document.
    /// </summary>
    public interface IOutlineGenerator
    {
        /// <summary>
        /// Generates the outline for the document.
        /// </summary>
        /// <param name="document">The document to create the outline for. Cannot be null.</param>
        /// <param name="noTextPlaceholder">The text to use when a node has no text. Cannot be null or empty.</param>
        /// <param name="useColors">A flag indicating if the colors of the nodes should be used.</param>
        /// <returns>The outline of the document.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="noTextPlaceholder"/> is null or empty.</exception>
        string GenerateOutline(Document document, bool useColors, string noTextPlaceholder);
    }
}
