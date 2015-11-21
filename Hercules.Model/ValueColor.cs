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
    public sealed class ValueColor : INodeColor, IEquatable<ValueColor>
    {
        private const string PropertyKeyForValue = "Value";
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

            properties.Set(PropertyKeyForValue, color);
        }

        public static INodeColor TryParse(PropertiesBag properties)
        {
            Guard.NotNull(properties, nameof(properties));

            INodeColor color = null;

            if (properties.Contains(PropertyKeyForValue))
            {
                try
                {
                    color = new ValueColor(properties[PropertyKeyForValue].ToInt32(CultureInfo.CurrentCulture));
                }
                catch (InvalidCastException)
                {
                    color = null;
                }
            }

            return color;
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