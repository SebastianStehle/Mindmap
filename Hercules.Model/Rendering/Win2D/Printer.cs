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
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Printing;

namespace Hercules.Model.Rendering.Win2D
{
    internal static class Printer
    {
        public static IPrintDocumentSource Print(Scene scene, float padding)
        {
            Guard.NotNull(scene, nameof(scene));
            Guard.GreaterThan(padding, 0, nameof(padding));

            CanvasPrintDocument printDocument = new CanvasPrintDocument();

            Rect2 sceneBounds = scene.Bounds;

            Action<CanvasDrawingSession, PrintPageDescription> renderForPrint = (session, page) =>
            {
                session.Clear(Colors.White);

                Vector2 size = page.PageSize.ToVector2();

                float ratio = sceneBounds.Width / sceneBounds.Height;

                float targetSizeX = Math.Min(size.X - (2 * padding), sceneBounds.Width);
                float targetSizeY = targetSizeX / ratio;

                if (targetSizeY > page.PageSize.Height)
                {
                    targetSizeY = Math.Min(size.Y - (2 * padding), sceneBounds.Height);
                    targetSizeX = targetSizeY * ratio;
                }

                float zoom = targetSizeX / sceneBounds.Width;

                session.Transform =
                    Matrix3x2.CreateTranslation(
                        -sceneBounds.Position.X,
                        -sceneBounds.Position.Y) *
                    Matrix3x2.CreateScale(zoom) *
                    Matrix3x2.CreateTranslation(
                         0.5f * (size.X - targetSizeX),
                         0.5f * (size.Y - targetSizeY));

                scene.Render(session, RenderFlags.Plain, Rect2.Infinite);
            };

            printDocument.Preview += (sender, args) =>
            {
                sender.SetPageCount(1);

                renderForPrint(args.DrawingSession, args.PrintTaskOptions.GetPageDescription(1));
            };

            printDocument.Print += (sender, args) =>
            {
                using (CanvasDrawingSession session = args.CreateDrawingSession())
                {
                    renderForPrint(session, args.PrintTaskOptions.GetPageDescription(1));
                }
            };

            return printDocument;
        }
    }
}
