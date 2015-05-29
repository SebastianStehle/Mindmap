// ==========================================================================
// CommandProperties.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GreenParrot.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MindmapApp.Model
{
    public sealed class CommandProperties : Dictionary<string, string>
    {
        public CommandProperties()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Set(string key, string value)
        {
            SetValue(key, value, x => x);
        }

        public string GetString(string key)
        {
            return ReadString(key);
        }

        public void Set(string key, int? value)
        {
            SetValue(key, value, x => x.HasValue ? x.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }

        public int? GetNullableInteger(string key)
        {
            return ParseValue(key, v => string.IsNullOrEmpty(v) ? (int?)null : (int?)int.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, int value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public int GetInteger(string key)
        {
            return ParseValue(key, v => int.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, float value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public float GetReal(string key)
        {
            return ParseValue(key, v => float.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, Guid value)
        {
            SetValue(key, value, x => x.ToString());
        }

        public Guid GetGuid(string key)
        {
            return ParseValue(key, v => Guid.Parse(v));
        }

        public void Set(string key, TimeSpan value)
        {
            SetValue(key, value, x => x.ToString());
        }

        public TimeSpan GetTimeSpan(string key)
        {
            return ParseValue(key, v => TimeSpan.Parse(v, CultureInfo.InvariantCulture));
        }

        public void Set(string key, DateTime value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public void Set(string key, object value)
        {
            SetValue(key, value, x => string.Format(CultureInfo.InvariantCulture, "{0}", x));
        }

        public DateTime GetDateTime(string key)
        {
            return ParseValue(key, v => DateTime.Parse(v, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
        }

        public void Set(string key, DateTimeOffset value)
        {
            SetValue(key, value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        public object Get(string key, Type type)
        {
            string value = ReadString(key);

            if (type == typeof(string))
            {
                return value;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (type == typeof(Guid))
                    {
                        return Guid.Parse(value);
                    }
                    else
                    {
                        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
        }

        public DateTimeOffset GetDateTimeOffset(string key)
        {
            return ParseValue(key, v => DateTimeOffset.Parse(v, CultureInfo.InvariantCulture));
        }

        private void SetValue<T>(string key, T value, Func<T, string> write)
        {
            Guard.NotNullOrEmpty(key, "key");

            this[key] = write(value);
        }

        private string ReadString(string key)
        {
            Guard.NotNullOrEmpty(key, "key");

            return this[key];
        }

        private T ParseValue<T>(string key, Func<string, T> parse)
        {
            Guard.NotNullOrEmpty(key, "key");

            T result = default(T);

            string value = this[key];

            try
            {
                result = parse(value);
            }
            catch
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "The value in the header is not a valid {0}", typeof(T)));
            }

            return result;
        }
    }
}
