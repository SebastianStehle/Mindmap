// ==========================================================================
// IntToBrushConverter.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SE.Metro.UI
{
    /// <summary>
    /// Converts an integer to a solid color brush.
    /// </summary>
    public class IntToBrushConverter : IValueConverter
    {
        private readonly double offsetH;
        private readonly double offsetV;
        private readonly double offsetS;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntToBrushConverter"/>
        /// </summary>
        public IntToBrushConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntToBrushConverter"/> with the offsets.
        /// </summary>
        /// <param name="h">The hue offset.</param>
        /// <param name="s">The saturation offset.</param>
        /// <param name="v">The value offset.</param>
        public IntToBrushConverter(double h, double s, double v)
        {
            offsetH = h;
            offsetV = v;
            offsetS = s;
        }

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
            if (value is int)
            {
                return new SolidColorBrush(ColorsHelper.ConvertToColor((int)value, offsetH, offsetS, offsetV));
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
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
            throw new NotImplementedException();
        }
    }
}
