// ==========================================================================
// INodeView.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace RavenMind.Model
{
    /// <summary>
    /// Represents a view for a single node.
    /// </summary>
    public interface INodeView
    {
        /// <summary>
        /// Gets the final render size of the view that is depending on the 
        /// content and any other visual elements like borders.
        /// </summary>
        /// <value>
        /// The size of the view when it will be rendered.
        /// </value>
        Size Size { get; }

        /// <summary>
        /// Sets the position of the view.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="animated">Use this value to animate the position change.</param>
        void SetPosition(Point position, bool animated);
    }
}
