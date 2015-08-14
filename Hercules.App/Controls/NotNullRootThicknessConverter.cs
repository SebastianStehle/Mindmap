// ==========================================================================
// NotNullRootThicknessConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hercules.App.Controls
{
    public sealed class NotNullRootThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? new Thickness(10, 25, 10, 0) : new Thickness(38, 25, 10, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
