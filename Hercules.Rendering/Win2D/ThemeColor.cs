// ==========================================================================
// ThemeColor.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;

namespace Hercules.Rendering.Win2D
{
    public sealed class ThemeColor
    {
        public static readonly ThemeColor White = new ThemeColor(Colors.White, Colors.White, Colors.White);

        private readonly Color normal;
        private readonly Color dark;
        private readonly Color light;
        private ICanvasBrush normalBrush;
        private ICanvasBrush darkBrush;
        private ICanvasBrush lightBrush;

        public Color Normal
        {
            get
            {
                return normal;
            }
        }

        public Color Dark
        {
            get
            {
                return dark;
            }
        }

        public Color Light
        {
            get
            {
                return light;
            }
        }

        public ThemeColor(Color normal, Color dark, Color light)
        {
            this.dark = dark;
            this.light = light;
            this.normal = normal;
        }

        public ICanvasBrush NormalBrush(CanvasDrawingSession session)
        {
            Guard.NotNull(session, nameof(session));

            return normalBrush ?? (normalBrush = new CanvasSolidColorBrush(session.Device, normal));
        }

        public ICanvasBrush DarkBrush(CanvasDrawingSession session)
        {
            Guard.NotNull(session, nameof(session));

            return darkBrush ?? (darkBrush = new CanvasSolidColorBrush(session.Device, dark));
        }

        public ICanvasBrush LightBrush(CanvasDrawingSession session)
        {
            Guard.NotNull(session, nameof(session));

            return lightBrush ?? (lightBrush = new CanvasSolidColorBrush(session.Device, light));
        }
    }
}
