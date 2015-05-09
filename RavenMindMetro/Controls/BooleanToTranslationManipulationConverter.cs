// ==========================================================================
// BooleanToTranslationManipulationConverter.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace RavenMind.Controls
{
    public sealed class BooleanToTranslationManipulationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? ManipulationModes.TranslateX | ManipulationModes.TranslateY : ManipulationModes.System;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
