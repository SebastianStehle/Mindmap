// ==========================================================================
// Behavior.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.Xaml.Interactivity;
using System;
using System.Globalization;
using Windows.UI.Xaml;

namespace GreenParrot.Windows.UI.Interactivity
{
    /// <summary>
    /// Base class for all type safe behaviors.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public abstract class Behavior<T> : DependencyObject, IBehavior where T : DependencyObject
    {
        private T element;

        /// <summary>
        /// Gets the associated element.
        /// </summary>
        /// <value>
        /// The associated element.
        /// </value>
        public T AssociatedElement
        {
            get
            {
                return element;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> is attached.
        /// </summary>
        /// <value>
        /// The associated element.
        /// </value>
        public DependencyObject AssociatedObject
        {
            get
            {
                return element;
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            Guard.NotNull(associatedObject, "associatedObject");

            element = associatedObject as T;

            if (element == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Behavior can only be attached to elements of type {0}.", typeof(T)));
            }

            OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected virtual void OnDetaching()
        {
        }
    }
}
