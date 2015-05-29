// ==========================================================================
// NodeSideToHorizontalAlignmentConverter.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MindmapApp.Controls
{
    public sealed class NodeSideToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return object.Equals(value, NodeSide.Left) ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
