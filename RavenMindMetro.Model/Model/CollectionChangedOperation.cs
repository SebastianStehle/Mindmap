// ==========================================================================
// CollectionChangedOperation.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{        
    /// <summary>
    /// Defines the type of the collection change operation.
    /// </summary>
    public enum CollectionChangedOperation
    {
        /// <summary>
        /// The shape has added to a collection.
        /// </summary>
        Added,
        /// <summary>
        /// The shape has removed from a collection.
        /// </summary>
        Removed
    }
}
