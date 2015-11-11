// ==========================================================================
// AttachmentIconBitmapConverter.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using GP.Windows;
using Hercules.Model;

namespace Hercules.App.Controls
{
    public sealed class AttachmentIconBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            AttachmentIcon icon = (AttachmentIcon)value;

            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.SetSourceAsync(icon.ToStream().AsRandomAccessStream()).AsTask().Forget();

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
