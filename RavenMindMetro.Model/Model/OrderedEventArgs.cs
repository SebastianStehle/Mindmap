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
    /// Stores the old and new orderindex.
    /// </summary>
    public sealed class OrderedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the old index of the object.
        /// </summary>
        /// <value>The old index of the object.</value>
        public int OldIndex { get; private set; }

        /// <summary>
        /// Gets the new index of the object.
        /// </summary>
        /// <value>The new index of the object.</value>
        public int NewIndex { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedEventArgs"/> with the new value and old index of the object.
        /// </summary>
        /// <param name="newIndex">The new index of the property.</param>
        /// <param name="oldIndex">The old index of the property.</param>
        public OrderedEventArgs(int newIndex, int oldIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        #endregion
    }
}
