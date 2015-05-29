// ==========================================================================
// NotNullNodeThicknessConverter.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MindmapApp.Controls
{
    public sealed class NotNullNodeThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? new Thickness(10, 8, 10, 0) : new Thickness(38, 8, 10, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
