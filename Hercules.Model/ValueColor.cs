// ==========================================================================
// CustomColor.cs
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
    public sealed class ValueColor : INodeColor, IEquatable<ValueColor>
    {
        private const string PropertyKey = "Value";
        private readonly int color;

        public int Color
        {
            get { return color; }
        }

        public ValueColor(int color)
        {
            Guard.GreaterEquals(color, 0, nameof(color));

            this.color = color;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey, color);
        }

        public static INodeColor TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            return properties.Contains(PropertyKey) ? new ValueColor(properties[PropertyKey].ToInt32(CultureInfo.CurrentCulture)) : null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValueColor);
        }

        public bool Equals(INodeColor other)
        {
            return Equals(other as ValueColor);
        }

        public bool Equals(ValueColor other)
        {
            return other != null && other.color == color;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
    }
}