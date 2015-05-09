// ==========================================================================
// NotRootToVisibilityConverter.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RavenMind.Controls
{
    public sealed class NotRootToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value as Node != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
