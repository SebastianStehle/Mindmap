﻿// ==========================================================================
// IAttachedObject.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// An interface for an object that can be attached to another object.
    /// </summary>
    public interface IAttachedObject
    {
        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>
        /// The associated object.
        /// </value>
        /// <remarks>Represents the object the instance is attached to.</remarks>
        FrameworkElement AssociatedObject { get; }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        void Attach(FrameworkElement dependencyObject);

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        void Detach();
    }
}
