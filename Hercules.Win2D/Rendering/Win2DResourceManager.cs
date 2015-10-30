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
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using GP.Windows;
using GP.Windows.UI.Controls;
using Hercules.Model;
using Hercules.Model.Rendering;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DResourceManager
    {
        private readonly Dictionary<Tuple<Color, float>, ICanvasBrush> cachedColors = new Dictionary<Tuple<Color, float>, ICanvasBrush>();
        private readonly Dictionary<string, ImageContainer> cachedImages = new Dictionary<string, ImageContainer>();
        private readonly List<LayoutThemeColor> colors = new List<LayoutThemeColor>();
        private readonly ICanvasControl canvas;

        private sealed class ImageContainer
        {
            public CanvasBitmap Bitmap { get; private set; }

            public ImageContainer(string image, ICanvasControl canvasControl)
            {
                LoadFile(image, canvasControl.Device).ContinueWith(bitmap =>
                {
                    canvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.High, canvasControl.Invalidate).AsTask();

                    Bitmap = bitmap.Result;
                });
            }

            private static async Task<CanvasBitmap> LoadFile(string image, ICanvasResourceCreator device)
            {
                string uri = string.Format(CultureInfo.InvariantCulture, "ms-appx://{0}", image);

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(device, stream).AsTask();
                }
            }
        }

        public IReadOnlyList<LayoutThemeColor> Colors
        {
            get
            {
                return colors;
            }
        }

        public Win2DResourceManager(ICanvasControl canvas)
        {
            Guard.NotNull(canvas, nameof(canvas));

            this.canvas = canvas;
        }

        public void AddThemeColors(params int[] newColors)
        {
            colors.AddRange(newColors.Select(x => new LayoutThemeColor(x)));
        }

        public void ClearResources()
        {
            foreach (ICanvasBrush brush in cachedColors.Values)
            {
                brush.Dispose();
            }

            cachedColors.Clear();

            foreach (ImageContainer image in cachedImages.Values)
            {
                image.Bitmap?.Dispose();
            }

            cachedImages.Clear();
        }

        public LayoutThemeColor FindColor(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            ThemeColor themeColor = node.Color as ThemeColor;

            return themeColor != null ? colors[themeColor.Index] : new LayoutThemeColor((CustomColor)node.Color);
        }

        public ICanvasBrush ThemeNormalBrush(int colorIndex)
        {
            LayoutThemeColor color = colors[colorIndex];

            return ThemeNormalBrush(color);
        }

        public ICanvasBrush ThemeNormalBrush(LayoutThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Normal, 1);
        }

        public ICanvasBrush ThemeDarkBrush(int colorIndex)
        {
            LayoutThemeColor color = colors[colorIndex];

            return Brush(color.Darker, 1);
        }

        public ICanvasBrush ThemeDarkBrush(LayoutThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Darker, 1);
        }

        public ICanvasBrush ThemeLightBrush(int colorIndex)
        {
            LayoutThemeColor color = colors[colorIndex];

            return Brush(color.Lighter, 1);
        }

        public ICanvasBrush ThemeLightBrush(LayoutThemeColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Lighter, 1);
        }

        public ICanvasBrush Brush(Color color, float opacity)
        {
            return cachedColors.GetOrCreateDefault(new Tuple<Color, float>(color, opacity), () =>
            {
                CanvasSolidColorBrush brush = new CanvasSolidColorBrush(canvas.Device, color);

                brush.Opacity = opacity;

                return brush;
            });
        }

        public ICanvasImage Image(string image)
        {
            return cachedImages.GetOrCreateDefault(image, () => new ImageContainer(image, canvas)).Bitmap;
        }
    }
}
