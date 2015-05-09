// ==========================================================================
// AdornerCollectionBase.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Represents a collection of adorners with a shared AssociatedObject and 
    /// provides change notifications to its contents when that AssociatedObject changes. 
    /// </summary>
    public class AdornerCollectionBase : AttachableCollection<Adorner>, IAttachedObject
    {
        #region Properties

        private AdornerLayer adornerLayer;
        /// <summary>
        /// Gets the adorner layer where this collection is assigned to.
        /// </summary>
        /// <value>The adorner layer.</value>
        public AdornerLayer AdornerLayer
        {
            get { return adornerLayer; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;

            AttachAdorners();
        }

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;

            DetachAdorners();
        }

        /// <summary>
        /// Called when a new item is added to the collection.
        /// </summary>
        /// <param name="item">The new item.</param>
        internal override void ItemAdded(Adorner item)
        {
            if (AssociatedObject != null)
            {
                if (adornerLayer != null)
                {
                    item.Attach(AssociatedObject);

                    adornerLayer.Add(item);
                }
            }
        }

        /// <summary>
        /// Called when an item is removed from the collection.
        /// </summary>
        /// <param name="item">The removed item.</param>
        internal override void ItemRemoved(Adorner item)
        {
            if (AssociatedObject != null)
            {
                if (adornerLayer != null)
                {
                    item.Detach();

                    adornerLayer.Remove(item);
                }
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AttachAdorners();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachAdorners();
        }

        private void AttachAdorners()
        {
            if (AssociatedObject != null)
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);

                if (adornerLayer != null)
                {
                    foreach (Adorner adorner in this.OrderBy(x => x.IsExclusive))
                    {
                        if (adorner != null)
                        {
                            adorner.Attach(AssociatedObject);

                            adornerLayer.Add(adorner);
                        }
                    }
                }

                OnAdornersAttached();
            }
        }

        private void DetachAdorners()
        {
            if (AssociatedObject != null)
            {
                if (adornerLayer != null)
                {
                    foreach (Adorner adorner in this)
                    {
                        if (adorner != null)
                        {
                            adorner.Detach();

                            adornerLayer.Remove(adorner);
                        }
                    }
                }

                adornerLayer = null;
            }
        }

        /// <summary>
        /// Called when the addorners has been attached.
        /// </summary>
        protected virtual void OnAdornersAttached()
        {
        }

        #endregion
    }
}
