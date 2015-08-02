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
    public sealed class MockupSelectableOrderedItem : ISelectable
    {
        public event EventHandler SelectionChanged;

        private void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public bool IsNotified { get; private set; }

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
