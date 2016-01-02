// ==========================================================================
// ValueToIntConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using System.Reflection;
using Windows.UI.Xaml.Data;
using Converter = System.Convert;

namespace GP.Windows.UI
{
    /// <summary>
    /// Converts the value to an integer.
    /// </summary>
    /// <remarks>
    /// The value is incremented by one, if the value is a class or nullable.
    /// </remarks>
    public sealed class ValueToIntConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int result = 0;
            int offset = 0;

            if (parameter is int)
            {
                offset = (int)parameter;
            }
            else if (parameter is string)
            {
                int.TryParse((string)parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out offset);
            }

            if (value != null)
            {
                result = (int)Converter.ChangeType(value, typeof(int), CultureInfo.InvariantCulture);

                TypeInfo typeInfo = value.GetType().GetTypeInfo();

                if (typeInfo.IsClass || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    result++;
                }

                result += offset;
            }

            return result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  
        /// This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
