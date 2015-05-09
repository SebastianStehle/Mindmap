// ==========================================================================
// INodeView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;

namespace RavenMind.Model
{
    /// <summary>
    /// Layout system for mindmaps.
    /// </summary>
    public interface ILayout
    {
        /// <summary>
        /// Updates the layout of the document.
        /// </summary>
        /// <param name="document">The document to update the layout for. Cannot be null.</param>
        /// <param name="views">A dictionary to get access to the views. Cannot be null.</param>
        /// <param name="availableSize">The available size.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="document"/> is null.
        ///     - or -
        ///     <paramref name="views"/> is null.
        /// </exception>
        void UpdateLayout(Document document, Func<NodeBase, INodeView> views, Size availableSize);

        /// <summary>
        /// Enables or disables animations.
        /// </summary>
        /// <value>
        /// A value indicating if animating the changes.
        /// </value>
        bool IsAnimating { get; set; }

        /// <summary>
        /// Gets the bounds including the children of the specified node.
        /// </summary>
        /// <param name="node">The node to get the bounds for. Cannot be null.</param>
        /// <returns>
        /// The bounds of the node.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        Rect? GetBounds(Node node);
    }
}
