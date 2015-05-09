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
    /// <summary>
    /// A custom collection that sync the selectable properties of the items with the collections.
    /// </summary>
    /// <typeparam name="T">The selectable items.</typeparam>
    public sealed class SelectingCollection<T> : CollectionBase<T> where T : class, ISelectable
    {
        #region Events

        /// <summary>
        /// Occurs when the selection has been changed.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Raises the <see cref="E:SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Fields

        private T selectedItem;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>A reference to the selected item.</value>
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

        #endregion

        #region Methods

        /// <summary>
        /// Handles the item before it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected override void HandleItemAdding(T newItem, int index)
        {
            newItem.SelectionChanged += new EventHandler(Item_SelectionChanged);

            if (newItem.IsSelected)
            {
                SelectedItem = newItem;
            }

            base.HandleItemAdding(newItem, index);
        }

        /// <summary>
        /// Handles the item before it will be removed from the collection. 
        /// </summary>
        /// <param name="oldItem">The old item before it will be removed from the collection. Cannot be null.</param>
        protected override void HandleItemRemoving(T oldItem)
        {
            oldItem.SelectionChanged -= new EventHandler(Item_SelectionChanged);

            if (oldItem.IsSelected)
            {
                SelectedItem = null;
            }

            base.HandleItemRemoving(oldItem);
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

        #endregion
    }
}
