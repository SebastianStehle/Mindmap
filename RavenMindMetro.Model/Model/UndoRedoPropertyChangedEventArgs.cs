// ==========================================================================
// UndoRedoPropertyChangedEventArgs.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Event argument to notify that a property that is managed by the undo-redo-system has been changed.
    /// </summary>
    public sealed class UndoRedoPropertyChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        /// <value>The old value of the property.</value>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        /// <value>The new value of the property.</value>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets the name of the property that has been changed.
        /// </summary>
        /// <value>The name of the property that has been changed.</value>
        public string PropertyName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoPropertyChangedEventArgs"/> with the new value and old value of the property.
        /// </summary>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="propertyName">Thew name of the property.</param>
        public UndoRedoPropertyChangedEventArgs(string propertyName, object newValue, object oldValue)
        {
            OldValue = oldValue;
            NewValue = newValue;

            PropertyName = propertyName;
        }

        #endregion
    }
}
