// ==========================================================================
// ScreenshotMaker.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Utils
{
    public static class ScreenshotMaker
    {
        private const float MaxAllowedSize = 5000;
        private const float DpiWithPixelMapping = 96;

        public static async Task RenderScreenshotAsync(Win2DScene scene, ICanvasResourceCreator device, IRandomAccessStream stream, Color background, float? dpi = null, float padding = 20)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(device, nameof(device));
            Guard.GreaterThan(padding, 0, nameof(padding));

            Rect2 sceneBounds = scene.RenderBounds;

            float w = sceneBounds.Size.X + (2 * padding);
            float h = sceneBounds.Size.Y + (2 * padding);

            float dpiValue = dpi ?? DisplayInformation.GetForCurrentView().LogicalDpi;

            float dpiFactor = dpiValue / DpiWithPixelMapping;

            if (w * dpiFactor > MaxAllowedSize || h * dpiFactor > MaxAllowedSize)
            {
                dpiValue = DpiWithPixelMapping;
            }

            using (CanvasRenderTarget target = new CanvasRenderTarget(device, w, h, dpiValue))
            {
                using (CanvasDrawingSession session = target.CreateDrawingSession())
                {
                    session.Clear(background);

                    session.Transform =
                        Matrix3x2.CreateTranslation(
                            -sceneBounds.Position.X + padding,
                            -sceneBounds.Position.Y + padding);

                    scene.Render(session, true, Rect2.Infinite);
                }

                await target.SaveAsync(stream, CanvasBitmapFileFormat.Png).AsTask();
            }
        }
    }
}
