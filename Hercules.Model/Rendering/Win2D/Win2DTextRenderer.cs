// ==========================================================================
// Win2DTextRenderer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.UI;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace Hercules.Model.Rendering.Win2D
{
    public sealed class Win2DTextRenderer
    {
        private readonly Vector2 padding = new Vector2(2, 2);
        private readonly NodeBase node;
        private readonly CanvasTextFormat textFormat;
        private readonly float fontSize;
        private readonly float minWidth;
        private CanvasTextLayout textLayout;
        private Vector2 renderSize;
        private Vector2 renderPosition;
        private float minSize;

        public bool HideText { get; set; }

        public float FontSize
        {
            get { return fontSize; }
        }

        public Rect2 Bounds
        {
            get { return new Rect2(renderPosition, renderSize); }
        }

        public Vector2 RenderPosition
        {
            get { return renderPosition; }
        }

        public Vector2 RenderSize
        {
            get { return renderSize; }
        }

        public string OverrideText { get; set; }

        public Win2DTextRenderer(float fontSize, NodeBase node, float minWidth)
        {
            this.fontSize = fontSize;
            this.minWidth = minWidth;
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
            renderSize.X = (float)Math.Round(Math.Max(renderSize.X, minWidth));
            renderSize.Y = (float)Math.Round(Math.Max(renderSize.Y, minSize));

            if (renderSize.Y % 2 == 1)
            {
                renderSize.Y += 1;
            }

            renderSize += 2 * padding;
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
