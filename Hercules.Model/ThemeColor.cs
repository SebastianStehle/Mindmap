// ==========================================================================
// ThemeColor.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Windows;

namespace Hercules.Model
{
    public sealed class ThemeColor : INodeColor, IEquatable<ThemeColor>
    {
        public static ThemeColor Default = new ThemeColor(0);
        private const string PropertyIndex = "Index";
        private const string PropertyIndexOld = "Color";
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

            properties.Set(PropertyIndex, index);
        }

        public static INodeColor TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            int value;

            if (properties.TryParseInt32(PropertyIndex, out value) ||
                properties.TryParseInt32(PropertyIndexOld, out value))
            {
                return new ThemeColor(value);
            }

            return null;
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
