// ==========================================================================
// VisibilityListener.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Helper class that can be used to listen to changes of the visibility property of any framework element.
    /// </summary>
    /// <remarks>
    /// The <see cref="FrameworkElement"/> class does not provide any events to notify a listener when tis visibility has changed. 
    /// Use this class to get an information when the visibility has changed.
    /// </remarks>
    public sealed class VisibilityListener : FrameworkElement
    {
        #region Fields

        private Action visibilityChangedAction;

        #endregion

        #region Properties

        /// <summary>
        /// Defines the <c>OverrideVisibility</c> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverrideVisibilityProperty =
            DependencyProperty.Register("OverrideVisibility", typeof(Visibility), typeof(VisibilityListener), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnOverrideVisibilityChanged)));
        private static void OnOverrideVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var owner = o as VisibilityListener;
            if (owner != null)
            {
                owner.OnOverrideVisibilityChanged();
            }
        }

        private void OnOverrideVisibilityChanged()
        {
            Action action = visibilityChangedAction;

            if (action != null)
            {
                action();
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Methods

        /// <summary>
        /// Binds the visibility listener to listen to all changes of the specified element.
        /// </summary>
        /// <param name="frameworkElement">The framework element. Cannot be null.</param>
        /// <param name="action">The action to invoke when the visibility changed. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="frameworkElement"/> is null.
        ///     - or -
        ///     <paramref name="action"/> is null.
        /// </exception>
        public void Bind(UIElement frameworkElement, Action action)
        {
            if (frameworkElement == null)
            {
                throw new ArgumentNullException("frameworkElement");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Binding binding = new Binding { Source = frameworkElement, Path = new PropertyPath("Visibility") };

            SetValue(OverrideVisibilityProperty, frameworkElement.Visibility);
            SetBinding(OverrideVisibilityProperty, binding);

            this.visibilityChangedAction = action;
        }

        /// <summary>
        /// Unbinds this instance from the framework element where it is binded to.
        /// </summary>
        public void Unbind()
        {
            ClearValue(OverrideVisibilityProperty);

            this.visibilityChangedAction = null;
        }

        #endregion
    }
}
