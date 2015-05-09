// ==========================================================================
// AdornerCollection.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Represents a collection of adorners with a shared AssociatedObject and 
    /// provides change notifications to its contents when that AssociatedObject changes. 
    /// </summary>
    public sealed class AdornerCollection : AdornerCollectionBase, IAttachedObject
    {
        #region Fields

        private Matrix previousMatrix;
        private Size previousSize;

        #endregion

        #region Methods

        /// <summary>
        /// Called when a new item is added to the collection.
        /// </summary>
        /// <param name="item">The new item.</param>
        internal override void ItemAdded(Adorner item)
        {
            item.Invalidated += adorner_Invalidated;
            item.VisibilityChanged += adorner_VisibilityChanged;

            base.ItemAdded(item);
        }

        /// <summary>
        /// Called when an item is removed from the collection.
        /// </summary>
        /// <param name="item">The removed item.</param>
        internal override void ItemRemoved(Adorner item)
        {
            item.Invalidated -= adorner_Invalidated;
            item.VisibilityChanged -= adorner_VisibilityChanged;

            base.ItemRemoved(item);
        }

        private void adorner_Invalidated(object sender, EventArgs e)
        {
            Transform(false);
        }

        private void adorner_VisibilityChanged(object sender, EventArgs e)
        {
            Adorner adorner = (Adorner)sender;

            if (adorner.IsExclusive && adorner.Visibility == Visibility.Visible)
            {
                foreach (Adorner otherAdorner in this)
                {
                    if (otherAdorner != null && otherAdorner != adorner)
                    {
                        otherAdorner.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Transform(bool force)
        {
            FrameworkElement associatedObject = AssociatedObject as FrameworkElement;

            if (associatedObject != null && AdornerLayer != null)
            {
                MatrixTransform transformation = (MatrixTransform)associatedObject.TransformToVisual(AdornerLayer);

                if (transformation != null)
                {
                    if (force || transformation.Matrix != previousMatrix || associatedObject.RenderSize != previousSize)
                    {
                        Transform(transformation, associatedObject);
                    }
                }
            }
        }

        private void Transform(MatrixTransform transformation, FrameworkElement associatedObject)
        {
            foreach (Adorner adorner in this)
            {
                if (adorner != null && adorner.Container != null)
                {
                    adorner.Container.Width  = associatedObject.ActualWidth;
                    adorner.Container.Height = associatedObject.ActualHeight;

                    adorner.Container.RenderTransform = transformation;
                }
            }

            previousMatrix = transformation.Matrix;

            previousSize = associatedObject.RenderSize;
        }

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                FrameworkElement frameworkElement = AssociatedObject as FrameworkElement;

                if (frameworkElement != null)
                {
                    frameworkElement.LayoutUpdated += AssociatedObject_LayoutUpdated;
                }
            }

            Transform(false);

            base.OnAttached();
        }

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                FrameworkElement frameworkElement = AssociatedObject as FrameworkElement;

                if (frameworkElement != null)
                {
                    frameworkElement.LayoutUpdated -= AssociatedObject_LayoutUpdated;
                }
            }

            base.OnDetaching();
        }

        /// <summary>
        /// Called when the addorners has been attached.
        /// </summary>
        protected override void OnAdornersAttached()
        {
            Transform(true);

            base.OnAdornersAttached();
        }

        /// <summary>
        /// Invalidates the adorners by updating the transformation.
        /// </summary>
        public void Invalidate()
        {
            Transform(false);
        }

        private void AssociatedObject_LayoutUpdated(object sender, object e)
        {
            Transform(false);
        }

        #endregion
    }
}
