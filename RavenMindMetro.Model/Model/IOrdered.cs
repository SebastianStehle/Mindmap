// ==========================================================================
// IOrdered.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Defines that the model can be ordered.
    /// </summary>
    public interface IOrdered
    {
        /// <summary>
        /// Occurs when the ordering has been changed.
        /// </summary>
        event EventHandler<OrderedEventArgs> OrderChanged;

        /// <summary>
        /// Gets or sets the order index.
        /// </summary>
        /// <value>
        /// The order index.
        /// </value>
        /// <remarks>
        /// By default this property is implemented as dependency property.
        /// </remarks>
        int OrderIndex { get; set; }

        /// <summary>
        /// Notifies the ordered object that it has been reordered by the user.
        /// </summary>
        /// <param name="oldIndex">The old order index.</param>
        void NotifyReordered(int oldIndex);
    }
}
