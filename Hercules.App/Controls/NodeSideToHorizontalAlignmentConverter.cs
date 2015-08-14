// ==========================================================================
// NodeSideToHorizontalAlignmentConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using Hercules.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hercules.App.Controls
{
    public sealed class NodeSideToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Equals(value, NodeSide.Left) ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
