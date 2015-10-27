// ==========================================================================
// ValueColor.cs
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
    public sealed class CustomColor : IColor, IEquatable<CustomColor>
    {
        private const string PropertyKey = "ColorValue";
        private readonly int color;

        public int Color
        {
            get { return color; }
        }

        public CustomColor(int color)
        {
            Guard.GreaterEquals(color, 0, nameof(color));

            this.color = color;
        }

        public void Save(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            properties.Set(PropertyKey, color);
        }

        public static IColor TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            return properties.Contains(PropertyKey) ? new CustomColor(properties[PropertyKey].ToInt32(CultureInfo.CurrentCulture)) : null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomColor);
        }

        public bool Equals(IColor other)
        {
            return Equals(other as CustomColor);
        }

        public bool Equals(CustomColor other)
        {
            return other != null && other.color == color;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
    }
}