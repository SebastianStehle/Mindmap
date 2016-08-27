// ==========================================================================
// CollectionExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Immutable;

namespace Hercules.Model
{
    public static class CollectionExtensions
    {
        public static ImmutableList<T> Insert<T>(this ImmutableList<T> list, T item, int? index)
        {
            if (index.HasValue && index < list.Count)
            {
                return list.Insert(index.Value, item);
            }
            else
            {
                return list.Add(item);
            }
        }
    }
}
