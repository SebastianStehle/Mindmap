// ==========================================================================
// Adorner.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Abstract base class for all adorners, which provides the common properties
    /// used by the adorner layer.
    /// </summary>
    /// <remarks>This is an infrastructure class. Behavior authors should derive from Adorner&lt;T&gt; instead of from this class.</remarks>
    public abstract class Adorner : AttachedObject, IAttachedObject
    {
        #region Events

        /// <summary>
        /// Occurs when the adorner has been invalidated and the transformation is calculated.
        /// </summary>
        public event EventHandler Invalidated;
        /// <summary>
        /// Raises the <see cref="E:Invalidated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnInvalidated(EventArgs e)
        {
            EventHandler eventHandler = Invalidated;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the visibility of the adorner has changed.
        /// </summary>
        public event EventHandler VisibilityChanged;
        /// <summary>
        /// Raises the <see cref="E:VisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnVisibilityChanged(EventArgs e)
        {
            EventHandler eventHandler = VisibilityChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Fields

        private readonly VisibilityListener visibilityListener = new VisibilityListener();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the container where this adorner is placed.
        /// </summary>
        /// <value>The container where this adorner is placed.</value>
        internal ContentControl Container { get; set; }

        /// <summary>
        /// Gets or sets the size of the previous matrix that has been assigned when the adorner has transformed last.
        /// </summary>
        /// <value>The previous matrix of the associated object.</value>
        internal Matrix PreviousMatrix { get; set; }

        /// <summary>
        /// Gets or sets the size of the previous size that has been assigned when the adorner has transformed last.
        /// </summary>
        /// <value>The previous size of the associated object.</value>
        internal Size PreviousSize { get; set; }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines the <see cref="IsExclusive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExclusiveProperty =
            DependencyProperty.Register("IsExclusive", typeof(bool), typeof(Adorner), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating if the adorner is exclusive.
        /// </summary>
        /// <value>A value indicating if the adorner is exclusive.</value>
        public bool IsExclusive
        {
            get { return (bool)GetValue(IsExclusiveProperty); }
            set { SetValue(IsExclusiveProperty, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Adorner"/> class with the 
        /// type of the associated object.
        /// </summary>
        /// <param name="associatedType">Type of the associated object. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="associatedType"/> is null.</exception>
        protected Adorner(Type associatedType)
            : base(associatedType)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invalidates this object and forces the adorner system to calculate the transformation of the object.
        /// </summary>
        protected void Invalidate()
        {
            OnInvalidated(EventArgs.Empty);
        }

        /// <summary>
        /// Called when the adonerer is attached to the <see cref="AdornerLayer"/>.
        /// </summary>
        protected override void OnAttached()
        {
            visibilityListener.Bind(this, OnVisibilityChanged);

            base.OnAttached();
        }

        /// <summary>
        /// Called when the adonerer is attached from the <see cref="AdornerLayer"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            visibilityListener.Unbind();

            base.OnDetaching();
        }

        private void OnVisibilityChanged()
        {
            if (AssociatedObject != null)
            {
                AdornerCollection adorners = Interactions.GetAdorners(AssociatedObject);

                if (!IsExclusive && Visibility == Visibility.Visible && adorners.Any(x => x.IsExclusive && x.Visibility == Visibility.Visible))
                {
                    Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (AssociatedObject.Visibility == Visibility.Collapsed)
                    {
                        OnCollapsed();
                    }
                    else
                    {
                        OnVisible();
                    }

                    OnVisibilityChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called when the adorner is shown when and becomes visible.
        /// </summary>
        protected virtual void OnVisible()
        {
        }

        /// <summary>
        /// Called when the adorner is shown when and becomes invisible.
        /// </summary>
        protected virtual void OnCollapsed()
        {
        }

        #endregion
    }
}
