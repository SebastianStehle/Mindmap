// ==========================================================================
// CollectionItemEventArgs.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    public sealed class CollectionItemEventArgs<TItem> : EventArgs
    {
        private readonly TItem item;
        private readonly int index;

        public TItem Item
        {
            get
            {
                return item;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public CollectionItemEventArgs(TItem item, int index)
        {
            this.item = item;
            this.index = index;
        }
    }
}
