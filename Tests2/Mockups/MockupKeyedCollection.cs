// ==========================================================================
// MockupKeyedCollection.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.ObjectModel;

namespace RavenMind.Mockups
{
    public class MockupKeyedCollection : KeyedCollection<int, int>
    {
        protected override int GetKeyForItem(int item)
        {
            return item;
        }
    }
}
