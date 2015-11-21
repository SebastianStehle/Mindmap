// ==========================================================================
// Win2DResourceManager.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
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
        private readonly Dictionary<INodeIcon, Win2DIcon> cachedIcons = new Dictionary<INodeIcon, Win2DIcon>();
        private readonly List<Win2DColor> colors = new List<Win2DColor>();
        private readonly ICanvasControl canvas;

        public IReadOnlyList<IRenderColor> Colors
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
            colors.AddRange(newColors.Select(x => new Win2DColor(x)));
        }

        public void ClearResources()
        {
            foreach (ICanvasBrush brush in cachedColors.Values)
            {
                brush.Dispose();
            }

            cachedColors.Clear();

            foreach (Win2DIcon icon in cachedIcons.Values)
            {
                icon.Dispose();
            }

            cachedIcons.Clear();
        }

        public ICanvasBrush ThemeNormalBrush(int colorIndex)
        {
            IRenderColor color = ResolveColor(colorIndex);

            return ThemeNormalBrush(color);
        }

        public ICanvasBrush ThemeNormalBrush(IRenderColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Normal, 1);
        }

        public ICanvasBrush ThemeDarkBrush(int colorIndex)
        {
            IRenderColor color = ResolveColor(colorIndex);

            return Brush(color.Darker, 1);
        }

        public ICanvasBrush ThemeDarkBrush(IRenderColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Darker, 1);
        }

        public ICanvasBrush ThemeLightBrush(int colorIndex)
        {
            IRenderColor color = ResolveColor(colorIndex);

            return Brush(color.Lighter, 1);
        }

        public ICanvasBrush ThemeLightBrush(IRenderColor color)
        {
            Guard.NotNull(color, nameof(color));

            return Brush(color.Lighter, 1);
        }

        private IRenderColor ResolveColor(int colorIndex)
        {
            if (colorIndex > 0 && colorIndex < colors.Count - 1)
            {
                return colors[colorIndex];
            }

            return colors[0];
        }

        public ICanvasBrush Brush(Color color, float opacity)
        {
            return cachedColors.GetOrCreateDefault(new Tuple<Color, float>(color, opacity), () =>
            {
                CanvasSolidColorBrush brush = new CanvasSolidColorBrush(canvas.Device, color) { Opacity = opacity };

                return brush;
            });
        }

        public ICanvasImage Image(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            return cachedIcons.GetOrCreateDefault(node.Icon, () => Win2DIcon.Create(node.Icon, canvas))?.Bitmap;
        }

        public Win2DIcon FindIcon(NodeBase node)
        {
            return cachedIcons.GetOrCreateDefault(node.Icon, () => Win2DIcon.Create(node.Icon, canvas));
        }

        public Win2DColor FindColor(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            ThemeColor themeColor = node.Color as ThemeColor;

            return themeColor != null ? colors[themeColor.Index] : new Win2DColor((ValueColor)node.Color);
        }
    }
}
