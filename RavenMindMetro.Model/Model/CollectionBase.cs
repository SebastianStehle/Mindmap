// ==========================================================================
// CollectionBase.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RavenMind.Model
{
    /// <summary>
    /// A custom collection that sync the order index of the items with the collections.
    /// </summary>
    /// <typeparam name="T">The orderable items.</typeparam>
    public class CollectionBase<T> : ObservableCollection<T>
    {
        #region Methods

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            List<T> items = new List<T>(this);

            foreach (T item in items)
            {
                HandleItemRemoving(item);
            }

            base.ClearItems();

            foreach (T item in items)
            {
                HandleItemRemoved(item);
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            HandleItemAdding(item, index);

            base.InsertItem(index, item);
            HandleItemAdded(item, index);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            T oldItem = this[index];

            HandleItemRemoving(oldItem);
            base.RemoveItem(index);
            HandleItemRemoved(oldItem);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];

            HandleItemRemoving(oldItem);
            HandleItemAdding(item, index);
            base.SetItem(index, item);
            HandleItemRemoved(oldItem);
            HandleItemAdded(item, index);
        }

        /// <summary>
        /// Handles the item before it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected virtual void HandleItemAdding(T newItem, int index)
        {
        }

        /// <summary>
        /// Handles the item after it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected virtual void HandleItemAdded(T newItem, int index)
        {
        }

        /// <summary>
        /// Handles the item before it will be removed from the collection. 
        /// </summary>
        /// <param name="oldItem">The old item before it will be removed from the collection. Cannot be null.</param>
        protected virtual void HandleItemRemoving(T oldItem)
        {
        }

        /// <summary>
        /// Handles the item before it will be removed from the collection. 
        /// </summary>
        /// <param name="oldItem">The old item before it will be removed from the collection. Cannot be null.</param>
        protected virtual void HandleItemRemoved(T oldItem)
        {
        }

        #endregion
    }
}
