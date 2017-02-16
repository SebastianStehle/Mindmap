// ==========================================================================
// ScreenshotMaker.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using GP.Utils;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering.Utils
{
    public static class ScreenshotMaker
    {
        private const int MaxSize = 5000;
        private const float DpiWithPixelMapping = 96;

        public static async Task RenderScreenshotAsync(Win2DScene scene, ICanvasResourceCreator device, Stream stream, Vector3 background, float? dpi = null, float padding = 20)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNull(device, nameof(device));
            Guard.GreaterThan(padding, 0, nameof(padding));

            var sceneBounds = scene.RenderBounds;

            var w = sceneBounds.Size.X + (2 * padding);
            var h = sceneBounds.Size.Y + (2 * padding);

            var dpiValue = dpi ?? DisplayInformation.GetForCurrentView().LogicalDpi;

            var dpiFactor = dpiValue / DpiWithPixelMapping;

            var wPixels = (int)(w * dpiFactor);
            var hPixels = (int)(h * dpiFactor);

            var maxPixels = Math.Min(MaxSize, device.Device.MaximumBitmapSizeInPixels - 100);

            var wDiff = wPixels - maxPixels;
            var hDiff = hPixels - maxPixels;

            if (wDiff > 0 || hDiff > 0)
            {
                if (wDiff > hDiff)
                {
                    dpiValue = (int)(dpiValue * ((float)maxPixels / wPixels));
                }
                else
                {
                    dpiValue = (int)(dpiValue * ((float)maxPixels / hPixels));
                }
            }

            using (var target = new CanvasRenderTarget(device, w, h, dpiValue))
            {
                using (var session = target.CreateDrawingSession())
                {
                    session.Clear(background.ToColor());

                    session.Transform =
                        Matrix3x2.CreateTranslation(
                            -sceneBounds.Position.X + padding,
                            -sceneBounds.Position.Y + padding);

                    scene.Render(session, false, Rect2.Infinite);
                }

                await target.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png).AsTask();
            }
        }
    }
}
