// ==========================================================================
// ISelectable.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Provides an interface for all items that can be selected by using a simple property.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Occurs when the current item has been selected or unselected by the user.
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected by the user 
        /// or by the application code.
        /// </summary>
        /// <value>
        /// true if this item is selected by the user; otherwise, false.
        /// </value>
        /// <remarks>By default this property is implemented as dependency property.</remarks>
        bool IsSelected { get; set; }
    }
}
