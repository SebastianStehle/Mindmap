// ==========================================================================
// ThemeBrushConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Hercules.Model.Layouting;

namespace Hercules.App.Controls
{
    public sealed class ThemeBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ThemeColor color = (ThemeColor)value;

            return new SolidColorBrush(color.Normal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
