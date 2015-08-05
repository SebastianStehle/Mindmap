// ==========================================================================
// TextRenderer.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class Win2DTextRenderer
    {
        private readonly Vector2 padding = new Vector2(5, 5);
        private readonly NodeBase node;
        private readonly CanvasTextFormat textFormat;
        private CanvasTextLayout textLayout;
        private Vector2 renderSize;
        private Vector2 renderPosition;
        private float minSize;

        public bool HideText { get; set; }

        public Rect2 Bounds
        {
            get
            {
                return new Rect2(renderPosition, renderSize);
            }
        }

        public Vector2 Position
        {
            get
            {
                return renderPosition;
            }
        }

        public Vector2 Size
        {
            get
            {
                return renderSize;
            }
        }

        public string OverrideText { get; set; }

        public Win2DTextRenderer(float fontSize, NodeBase node)
        {
            this.node = node;

            textFormat = new CanvasTextFormat { FontSize = fontSize, WordWrapping = CanvasWordWrapping.NoWrap, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center };
        }

        public void Measure(CanvasDrawingSession session)
        {
            minSize = textFormat.FontSize * 2;

            string text = OverrideText ?? node.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                textLayout = new CanvasTextLayout(session, text, textFormat, 0.0f, 0.0f);

                renderSize = new Vector2(
                    (float)textLayout.DrawBounds.Width,
                    (float)textLayout.DrawBounds.Height);
            }
            else
            {
                renderSize = Vector2.Zero;
            }

            renderSize.X = (float)Math.Round(Math.Max(renderSize.X, minSize));
            renderSize.Y = (float)Math.Round(Math.Max(renderSize.Y, minSize));

            renderSize += padding;
        }

        public void Arrange(Vector2 position)
        {
            renderPosition = position;
        }

        public void Render(CanvasDrawingSession session)
        {
            string text = OverrideText ?? node.Text;
#if DRAW_OUTLINE
            session.DrawRectangle(Bounds, Colors.Red);
#endif
            if (!HideText && !string.IsNullOrWhiteSpace(text))
            {
                session.DrawText(text, Bounds, Colors.Black, textFormat);
            }
        }
    }
}
