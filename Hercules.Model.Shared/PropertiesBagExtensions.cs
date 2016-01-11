// ==========================================================================
// PropertiesBagExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;

namespace Hercules.Model
{
    public static class PropertiesBagExtensions
    {
        public static bool TryParseString(this PropertiesBag properties, string propertyName, out string value)
        {
            bool result = false;

            value = null;

            if (properties.Contains(propertyName))
            {
                value = properties[propertyName].ToString();

                result = true;
            }

            return result;
        }

        public static bool TryParseNullableInt32(this PropertiesBag properties, string propertyName, out int? value)
        {
            bool result = false;

            value = null;

            if (properties.Contains(propertyName))
            {
                try
                {
                    value = properties[propertyName].ToNullableInt32(CultureInfo.CurrentCulture);

                    result = true;
                }
                catch (InvalidCastException)
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseEnum<TEnum>(this PropertiesBag properties, string propertyName, out TEnum value) where TEnum : struct
        {
            bool result = false;

            value = default(TEnum);

            if (properties.Contains(propertyName))
            {
                string enumValue = properties[propertyName].ToString();

                if (Enum.TryParse(enumValue, out value))
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool TryParseInt32(this PropertiesBag properties, string propertyName, out int value)
        {
            bool result = false;

            value = 0;

            if (properties.Contains(propertyName))
            {
                try
                {
                    value = properties[propertyName].ToInt32(CultureInfo.CurrentCulture);

                    result = true;
                }
                catch (InvalidCastException)
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
