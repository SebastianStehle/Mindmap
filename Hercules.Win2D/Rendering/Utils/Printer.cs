// ==========================================================================
// Printer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.Graphics.Printing;
using Windows.UI;
using GP.Utils;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Printing;

namespace Hercules.Win2D.Rendering.Utils
{
    internal static class Printer
    {
        public static IPrintDocumentSource Print(Win2DScene scene, float padding)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.GreaterThan(padding, 0, nameof(padding));

            var printDocument = new CanvasPrintDocument();

            var sceneBounds = scene.RenderBounds;

            Action<CanvasDrawingSession, PrintPageDescription> renderForPrint = (session, page) =>
            {
                session.Clear(Colors.White);

                var size = page.PageSize.ToVector2();

                var ratio = sceneBounds.Width / sceneBounds.Height;

                var targetSizeX = Math.Min(size.X - (2 * padding), sceneBounds.Width);
                var targetSizeY = targetSizeX / ratio;

                if (targetSizeY > page.PageSize.Height)
                {
                    targetSizeY = Math.Min(size.Y - (2 * padding), sceneBounds.Height);
                    targetSizeX = targetSizeY * ratio;
                }

                var zoom = targetSizeX / sceneBounds.Width;

                session.Transform =
                    Matrix3x2.CreateTranslation(
                        -sceneBounds.Position.X,
                        -sceneBounds.Position.Y) *
                    Matrix3x2.CreateScale(zoom) *
                    Matrix3x2.CreateTranslation(
                         0.5f * (size.X - targetSizeX),
                         0.5f * (size.Y - targetSizeY));

                scene.Render(session, false, Rect2.Infinite);
            };

            printDocument.Preview += (sender, args) =>
            {
                sender.SetPageCount(1);

                renderForPrint(args.DrawingSession, args.PrintTaskOptions.GetPageDescription(1));
            };

            printDocument.Print += (sender, args) =>
            {
                using (var session = args.CreateDrawingSession())
                {
                    renderForPrint(session, args.PrintTaskOptions.GetPageDescription(1));
                }
            };

            return printDocument;
        }
    }
}
