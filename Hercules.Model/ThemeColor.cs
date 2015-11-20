// ==========================================================================
// ThemeColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using GP.Windows;

namespace Hercules.Model
{
    public sealed class ThemeColor : INodeColor, IEquatable<ThemeColor>
    {
        private const string PropertyKey_Index = "Index";
        private readonly int index;

        public int Index
        {
            get { return index; }
        }

        public ThemeColor(int index)
        {
            Guard.GreaterEquals(index, 0, nameof(index));

            this.index = index;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey_Index, index);
        }

        public static INodeColor TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            return properties.Contains(PropertyKey_Index) ? new ThemeColor(properties[PropertyKey_Index].ToInt32(CultureInfo.CurrentCulture)) : null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ThemeColor);
        }

        public bool Equals(INodeColor other)
        {
            return Equals(other as ThemeColor);
        }

        public bool Equals(ThemeColor other)
        {
            return other != null && other.index == index;
        }

        public override int GetHashCode()
        {
            return index.GetHashCode();
        }
    }
}
