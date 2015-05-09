// ==========================================================================
// IUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Reflection;

namespace RavenMind.Model
{
    /// <summary>
    /// Contains all information about a property and its old and current 
    /// value.
    /// </summary>
    public sealed class PropertyChangedUndoRedoAction : IUndoRedoAction
    {
        #region Fields

        private readonly PropertyInfo targetProperty;
        private readonly object target;
        private readonly object oldValue;
        private readonly object newValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedUndoRedoAction"/> class
        /// with the property name and the value and old value of this property.
        /// </summary>
        /// <param name="target">The target object which property has been changed.</param>
        /// <param name="propertyName">Name of the property, which has been changed. Cannot be null or empty.</param>
        /// <param name="newValue">The value of the property with the specified name.</param>
        /// <param name="oldValue">The old value of the property with the specified name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public PropertyChangedUndoRedoAction(object target, string propertyName, object newValue, object oldValue)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name is null or whitespace", "propertyName");
            }
           
            var targetType = target.GetType();

            this.newValue = newValue;
            this.oldValue = oldValue;
            this.targetProperty = targetType.GetRuntimeProperty(propertyName);
            this.target = target;

            if (targetProperty == null)
            {
                throw new ArgumentException("Property does not exist onm the target.", "propertyName");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines an undo method, which is called to undo all changes
        /// that has been made by this action.
        /// </summary>
        public void Undo()
        {
            ChangePropertyValue(oldValue);
        }

        /// <summary>
        /// Executes the action again.
        /// </summary>
        public void Redo()
        {
            ChangePropertyValue(newValue);
        }

        private void ChangePropertyValue(object value)
        {
            targetProperty.SetValue(target, value, null);
        }

        #endregion
    }
}
