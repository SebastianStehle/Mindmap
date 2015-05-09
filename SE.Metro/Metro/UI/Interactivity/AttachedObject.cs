// ==========================================================================
// AttachedObject.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Abstract base class for all adorners, which provides the common properties
    /// used by the adorner layer.
    /// </summary>
    /// <remarks>Do not use this class directly. Always dervice from the abstract
    /// version of this class or from a the node adorner from shape nodes.</remarks>
    public abstract class AttachedObject : Control, IAttachedObject
    {
        #region Properties

        private FrameworkElement associatedObject;
        /// <summary>
        /// Gets a reference to the adorned object this is used by the 
        /// current instance.
        /// </summary>
        /// <value>The adorned object.</value>
        public FrameworkElement AssociatedObject
        {
            get
            {
                if (associatedObject == null)
                {
                    throw new InvalidOperationException("Associated Object can only be used when the adorner is attached.");
                }

                return associatedObject;
            }
        }

        private readonly Type associatedType;
        /// <summary>
        /// Gets the type to which this behavior can be attached. 
        /// </summary>
        /// <value>The type to which this behavior can be attached. </value>
        public Type AssociatedType
        {
            get
            {
                return associatedType;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedObject"/> class with the 
        /// type of the associated object.
        /// </summary>
        /// <param name="associatedType">Type of the associated object. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="associatedType"/> is null.</exception>
        protected AttachedObject(Type associatedType)
        {
            if (associatedType == null)
            {
                throw new ArgumentNullException("associatedType");
            }

            this.associatedType = associatedType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the adorner to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to. Cannot be null.</param>
        /// <exception cref="InvalidOperationException">
        ///     The adorner is already hosted on a different element.
        ///     - or -
        ///     <paramref name="dependencyObject"/> does not satisfy the Behavior type constraint.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="dependencyObject"/> is null.</exception>
        public void Attach(FrameworkElement dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            if (associatedObject != null)
            {
                throw new InvalidOperationException("Cannot host adorner multiple times.");
            }

            associatedObject = dependencyObject;

            OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();

            associatedObject = null;
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
        
        #endregion
    }
}
