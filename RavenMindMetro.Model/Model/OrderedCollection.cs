// ==========================================================================
// OrderedCollection.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// A custom collection that sync the order index of the items with the collections.
    /// </summary>
    /// <typeparam name="T">The orderable items.</typeparam>
    public class OrderedCollection<T> : CollectionBase<T> where T : class, IOrdered
    {
        #region Fields

        private bool preventEventUpdates;

        #endregion

        #region Methods

#if !SILVERLIGHT
        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            try
            {
                preventEventUpdates = true;

                base.MoveItem(oldIndex, newIndex);

                UpdateIndicesAfter(Math.Min(oldIndex, newIndex));
            }
            finally
            {
                preventEventUpdates = false;
            }
        }
#endif

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            try
            {
                preventEventUpdates = true;

                base.InsertItem(index, item);
                UpdateIndicesAfter(index);
            }
            finally
            {
                preventEventUpdates = false;
            }
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            try
            {
                preventEventUpdates = true;

                base.RemoveItem(index);
                UpdateIndicesAfter(index);
            }
            finally
            {
                preventEventUpdates = false;
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            try
            {
                preventEventUpdates = true;

                base.SetItem(index, item);
            }
            finally
            {
                preventEventUpdates = false;
            }
        }

        private void Item_OrderChanged(object sender, OrderedEventArgs e)
        {
            if (!preventEventUpdates)
            {
                try
                {
                    preventEventUpdates = true;

                    T item = sender as T;

                    if (item != null)
                    {
                        int oldIndex = e.OldIndex;

                        if (item.OrderIndex < 1)
                        {
                            item.OrderIndex = 1;
                        }
                        else if (item.OrderIndex > Count)
                        {
                            item.OrderIndex = Count;
                        }

                        if (oldIndex != item.OrderIndex)
                        {
                            int newIndex = item.OrderIndex - 1;

                            Move(oldIndex - 1, newIndex);
                        }

                        UpdateIndicesAfter(Math.Max(item.OrderIndex, oldIndex) - 1);

                        item.NotifyReordered(e.OldIndex);
                    }
                }
                finally
                {
                    preventEventUpdates = false;
                }
            }
        }

        /// <summary>
        /// Handles the item before it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected override void HandleItemAdding(T newItem, int index)
        {
            newItem.OrderChanged += Item_OrderChanged;
            newItem.OrderIndex = index + 1;

            base.HandleItemAdding(newItem, index);
        }

        /// <summary>
        /// Handles the item before it will be removed from the collection. 
        /// </summary>
        /// <param name="oldItem">The old item before it will be removed from the collection. Cannot be null.</param>
        protected override void HandleItemRemoving(T oldItem)
        {
            oldItem.OrderChanged -= Item_OrderChanged;

            base.HandleItemRemoving(oldItem);
        }
        
        private void UpdateIndicesAfter(int index)
        {
            for (int i = index; i < Count; i++)
            {
                T item = this[i];

                if (item != null)
                {
                    item.OrderIndex = i + 1;
                }
            }
        }

        #endregion
    }
}
