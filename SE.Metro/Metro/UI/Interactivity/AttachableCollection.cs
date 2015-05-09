// ==========================================================================
// AttachableCollection.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Represents a collection of IAttachedObject with a shared AssociatedObject and provides change notifications to its contents when that AssociatedObject changes.
    /// </summary>
    public abstract class AttachableCollection<T> : Collection<T>, IAttachedObject where T : DependencyObject, IAttachedObject
    {
        #region Fields

        private FrameworkElement associatedObject;

        #endregion

        #region Properties

        /// <summary>
        /// The object on which the collection is hosted.
        /// </summary>
        protected FrameworkElement AssociatedObject
        {
            get
            {
                return associatedObject;
            }
        }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        FrameworkElement IAttachedObject.AssociatedObject
        {
            get
            {
                return this.AssociatedObject;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        /// <exception cref="InvalidOperationException">The IAttachedObject is already attached to a different object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dependencyObject"/> is null.</exception>
        public void Attach(FrameworkElement dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            if (dependencyObject != AssociatedObject)
            {
                if (AssociatedObject != null)
                {
                    throw new InvalidOperationException();
                }

                associatedObject = dependencyObject;

                OnAttached();
            }
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
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected abstract void OnAttached();

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected abstract void OnDetaching();

        /// <summary>
        /// Called when a new item is added to the collection.
        /// </summary>
        /// <param name="item">The new item.</param>
        internal abstract void ItemAdded(T item);

        /// <summary>
        /// Called when an item is removed from the collection.
        /// </summary>
        /// <param name="item">The removed item.</param>
        internal abstract void ItemRemoved(T item);

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection&lt;T&gt;"/>.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T item in this)
            {
                ItemRemoved(item);
            }
            base.ClearItems();
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection&lt;T&gt;"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than zero.
        ///     -or-
        ///     <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.
        /// </exception>
        protected override void InsertItem(int index, T item)
        {
            ItemAdded(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the  <see cref="T:System.Collections.ObjectModel.Collection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than zero.
        ///     -or-
        ///     <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.
        /// </exception>
        protected override void RemoveItem(int index)
        {
            ItemRemoved(this[index]);
            base.RemoveItem(index);
        } 

        /// <summary>
        /// eplaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than zero.
        ///     -or-
        ///     <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection.Count"/>.
        /// </exception>
        protected override void SetItem(int index, T item)
        {
            ItemRemoved(this[index]);
            ItemAdded(item);
            base.SetItem(index, item);
        }

        #endregion
    }
}
