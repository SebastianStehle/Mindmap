// ==========================================================================
// MockupSelectableOrderedItem.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using RavenMind.Model;

namespace RavenMind.Mockups
{
    public sealed class MockupSelectableOrderedItem : IOrdered, ISelectable
    {
        public event EventHandler<OrderedEventArgs> OrderChanged;
        public event EventHandler SelectionChanged;

        private void OnOrderChanged(OrderedEventArgs e)
        {
            EventHandler<OrderedEventArgs> eventHandler = OrderChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        private void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public bool IsNotified { get; private set; }

        private int orderIndex;
        public int OrderIndex
        {
            get
            {
                return orderIndex;
            }
            set
            {
                if (orderIndex != value)
                {
                    int oldIndex = orderIndex;

                    orderIndex = value;
                    OnOrderChanged(new OrderedEventArgs(value, oldIndex));
                }
            }
        }

        private bool isSelectable;
        public bool IsSelected
        {
            get
            {
                return isSelectable;
            }
            set
            {
                if (isSelectable != value)
                {
                    isSelectable = value;
                    OnSelectionChanged(EventArgs.Empty);
                }
            }
        }

        public void NotifyReordered(int oldIndex)
        {
            IsNotified = true;
        }
    }
}
