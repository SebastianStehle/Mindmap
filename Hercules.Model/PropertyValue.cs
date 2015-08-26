// ==========================================================================
// PropertyValue.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hercules.Model
{
    public sealed class PropertyValue
    {
        private readonly object rawValue;

        private static readonly HashSet<Type> AllowedTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(bool),
            typeof(bool?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(Guid),
            typeof(Guid?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?)
        };

        public object RawValue
        {
            get
            {
                return rawValue;
            }
        }

        public PropertyValue(object rawValue)
        {
            if (rawValue != null && !AllowedTypes.Contains(rawValue.GetType()))
            {
                throw new ArgumentException("The type is not supported.", nameof(rawValue));
            }

            this.rawValue = rawValue;
        }

        public override string ToString()
        {
            return rawValue?.ToString();
        }

        public bool ToBoolean(CultureInfo culture)
        {
            return ToOrParseValue(culture, bool.Parse);
        }

        public bool? ToNullableBoolean(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, bool.Parse);
        }

        public float ToSingle(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => float.Parse(x, culture));
        }

        public float? ToNullableSingle(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => float.Parse(x, culture));
        }

        public double ToDouble(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => double.Parse(x, culture));
        }

        public double? ToNullableDouble(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => double.Parse(x, culture));
        }

        public int ToInt32(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => int.Parse(x, culture));
        }

        public int? ToNullableInt32(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => int.Parse(x, culture));
        }

        public long ToInt64(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => long.Parse(x, culture));
        }

        public long? ToNullableInt64(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => long.Parse(x, culture));
        }

        public TimeSpan ToTimeSpan(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => TimeSpan.Parse(x, culture));
        }

        public TimeSpan? ToNullableTimeSpan(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => TimeSpan.Parse(x, culture));
        }

        public DateTime ToDateTime(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => DateTime.Parse(x, culture));
        }

        public DateTime? ToNullableDateTime(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => DateTime.Parse(x, culture));
        }

        public Guid ToGuid(CultureInfo culture)
        {
            return ToOrParseValue(culture, Guid.Parse);
        }

        public Guid? ToNullableGuid(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, Guid.Parse);
        }

        public DateTimeOffset ToDateTimeOffset(CultureInfo culture)
        {
            return ToOrParseValue(culture, x => DateTimeOffset.Parse(x, culture));
        }

        public DateTimeOffset? ToNullableDateTimeOffset(CultureInfo culture)
        {
            return ToNullableOrParseValue(culture, x => DateTimeOffset.Parse(x, culture));
        }

        private T? ToNullableOrParseValue<T>(CultureInfo culture, Func<string, T> parser) where T : struct
        {
            T? result = null;

            object value = rawValue;

            if (value != null)
            {
                Type valueType = value.GetType();

                if (valueType == typeof(T))
                {
                    result = (T)value;
                }
                else if (valueType == typeof(string))
                {
                    result = Parse(parser, valueType, value);
                }
                else
                {
                    result = Convert<T>(culture, value, valueType);
                }
            }

            return result;
        }

        private T ToOrParseValue<T>(CultureInfo culture, Func<string, T> parser)
        {
            T result = default(T);

            object value = rawValue;

            if (value != null)
            {
                Type valueType = value.GetType();

                if (valueType == typeof(T))
                {
                    result = (T)value;
                }
                else if (valueType == typeof(string))
                {
                    result = Parse(parser, valueType, value);
                }
                else
                {
                    result = Convert<T>(culture, value, valueType);
                }
            }

            return result;
        }

        private static T Convert<T>(CultureInfo culture, object value, Type valueType)
        {
            Type requestedType = typeof(T);

            try
            {
                return (T)System.Convert.ChangeType(value, requestedType, culture);
            }
            catch (OverflowException)
            {
                string message = $"The property has type '{valueType}' and cannot be casted to '{requestedType}' because it is either too small or large.";

                throw new InvalidCastException(message);
            }
            catch (InvalidCastException)
            {
                string message = $"The property has type '{valueType}' and cannot be casted to '{requestedType}'.";

                throw new InvalidCastException(message);
            }
        }

        private static T Parse<T>(Func<string, T> parser, Type valueType, object value)
        {
            Type requestedType = typeof(T);

            try
            {
                return parser(value.ToString());
            }
            catch (Exception e)
            {
                string message = $"The property has type '{valueType}' and cannot be casted to '{requestedType}'.";

                throw new InvalidCastException(message, e);
            }
        }
    }
}
