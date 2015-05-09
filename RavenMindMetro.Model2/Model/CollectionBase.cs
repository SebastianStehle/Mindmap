// ==========================================================================
// CollectionBase.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RavenMind.Model
{
    public class CollectionBase<T> : ObservableCollection<T>
    {
        public event EventHandler<CollectionItemEventArgs<T>> ItemAdded;

        protected virtual void OnItemAdded(CollectionItemEventArgs<T> e)
        {
            EventHandler<CollectionItemEventArgs<T>> eventHandler = ItemAdded;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public event EventHandler<CollectionItemEventArgs<T>> ItemRemoved;

        protected virtual void OnItemRemoved(CollectionItemEventArgs<T> e)
        {
            EventHandler<CollectionItemEventArgs<T>> eventHandler = ItemRemoved;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        
        protected override void ClearItems()
        {
            List<T> items = new List<T>(this);

            for (int i = 0; i < items.Count; i++)
            {
                HandleItemRemoving(items[i], i);
            }

            base.ClearItems();

            for (int i = 0; i < items.Count; i++)
            {
                HandleItemRemoved(items[i], i);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            HandleItemAdding(item, index);

            base.InsertItem(index, item);

            HandleItemAdded(item, index);
        }

        protected override void RemoveItem(int index)
        {
            T oldItem = this[index];

            HandleItemRemoving(oldItem, index);

            base.RemoveItem(index);

            HandleItemRemoved(oldItem, index);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];

            HandleItemRemoving(oldItem, index);
            HandleItemAdding(item, index);

            base.SetItem(index, item);

            HandleItemRemoved(oldItem, index);
            HandleItemAdded(item, index);
        }

        private void HandleItemAdding(T newItem, int index)
        {
            OnItemAdding(newItem, index);
        }

        private void HandleItemAdded(T newItem, int index)
        {
            OnItemAdded(newItem, index);
            OnItemAdded(new CollectionItemEventArgs<T>(newItem, index));
        }

        private void HandleItemRemoving(T oldItem, int index)
        {
            OnItemRemoving(oldItem, index);
        }

        private void HandleItemRemoved(T oldItem, int index)
        {
            OnItemRemoved(oldItem, index);
            OnItemRemoved(new CollectionItemEventArgs<T>(oldItem, index));
        }

        protected virtual void OnItemAdding(T newItem, int index)
        {
        }

        protected virtual void OnItemAdded(T newItem, int index)
        {
        }

        protected virtual void OnItemRemoving(T oldItem, int index)
        {
        }

        protected virtual void OnItemRemoved(T oldItem, int index)
        {
        }
    }
}
