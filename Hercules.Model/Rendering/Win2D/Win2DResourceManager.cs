// ==========================================================================
// Win2DResourceManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using GP.Windows;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using GP.Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class Win2DResourceManager
    {
        private readonly Dictionary<Tuple<Color, float>, ICanvasBrush> cachedColors = new Dictionary<Tuple<Color, float>, ICanvasBrush>();
        private readonly Dictionary<string, ImageContainer> cachedImages = new Dictionary<string, ImageContainer>();
        private readonly List<ThemeColor> colors = new List<ThemeColor>();
        private CanvasControl canvasControl;

        private sealed class ImageContainer
        {
            public CanvasBitmap Bitmap;

            public ImageContainer(string image, CanvasDevice device, CanvasControl canvas)
            {
                LoadFile(image, device).ContinueWith(bitmap =>
                {
                    canvas.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        canvas.Invalidate();
                    }).AsTask();

                    Bitmap = bitmap.Result;
                });
            }

            private async Task<CanvasBitmap> LoadFile(string image, CanvasDevice device)
            {
                string uri = string.Format(CultureInfo.InvariantCulture, "ms-appx:///{0}", image);

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

                using (var stream = await file.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(device, stream).AsTask();
                }
            }
        }

        public IReadOnlyList<ThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        public void AddThemeColors(params int[] newColors)
        {
            foreach (int color in newColors)
            {
                colors.Add(new ThemeColor(
                    ColorsHelper.ConvertToColor(color, 0, 0, 0),
                    ColorsHelper.ConvertToColor(color, 0, 0.2, -0.3),
                    ColorsHelper.ConvertToColor(color, 0, -0.2, 0.2)));
            }
        }

        public void Initialize(CanvasControl canvas)
        {
            canvasControl = canvas;

            cachedColors.Clear();
            cachedImages.Clear();
        }

        public ThemeColor FindColor(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            return colors[node.Color];
        }

        public ICanvasBrush ThemeNormalBrush(int colorIndex)
        {
            ThemeColor color = colors[colorIndex];

            return ThemeNormalBrush(color);
        }

        public ICanvasBrush ThemeNormalBrush(ThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Normal, 1);
        }

        public ICanvasBrush ThemeDarkBrush(int colorIndex)
        {
            ThemeColor color = colors[colorIndex];

            return Brush(color.Dark, 1);
        }

        public ICanvasBrush ThemeDarkBrush(ThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Dark, 1);
        }

        public ICanvasBrush ThemeLightBrush(int colorIndex)
        {
            ThemeColor color = colors[colorIndex];

            return Brush(color.Light, 1);
        }

        public ICanvasBrush ThemeLightBrush(ThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Light, 1);
        }

        public ICanvasBrush Brush(Color color, float opacity)
        {
            if (canvasControl != null)
            {
                return cachedColors.GetOrCreateDefault(new Tuple<Color, float>(color, opacity), () =>
                {
                    CanvasSolidColorBrush brush = new CanvasSolidColorBrush(canvasControl.Device, color);

                    brush.Opacity = opacity;

                    return brush;
                });
            }
            else
            {
                return null;
            }
        }
        
        public ICanvasImage Image(string image)
        {
            if (canvasControl != null)
            {
                return cachedImages.GetOrCreateDefault(image, () => new ImageContainer(image, canvasControl.Device, canvasControl)).Bitmap;
            }
            else
            {
                return null;
            }
        }
    }
}
