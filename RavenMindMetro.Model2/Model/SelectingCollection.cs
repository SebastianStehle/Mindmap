// ==========================================================================
// SelectingCollection.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    public sealed class SelectingCollection<T> : CollectionBase<T> where T : class, ISelectable
    {
        private T selectedItem;

        public event EventHandler SelectionChanged;

        private void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public T SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = false;
                    }

                    selectedItem = value;

                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = true;
                    }

                    OnSelectionChanged(EventArgs.Empty);
                }
            }
        }

        protected override void OnItemAdding(T newItem, int index)
        {
            newItem.SelectionChanged += new EventHandler(Item_SelectionChanged);

            if (newItem.IsSelected)
            {
                SelectedItem = newItem;
            }

            base.OnItemAdding(newItem, index);
        }

        protected override void OnItemRemoving(T oldItem, int index)
        {
            oldItem.SelectionChanged -= new EventHandler(Item_SelectionChanged);

            if (oldItem.IsSelected)
            {
                SelectedItem = null;
            }

            base.OnItemRemoving(oldItem, index);
        }

        private void Item_SelectionChanged(object sender, EventArgs e)
        {
            T item = sender as T;

            if (item != null)
            {
                if (SelectedItem == item && !item.IsSelected)
                {
                    SelectedItem = null;
                }

                if (item.IsSelected)
                {
                    SelectedItem = item;
                }
            }
        }
    }
}
