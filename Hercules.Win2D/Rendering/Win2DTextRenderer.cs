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
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DTextRenderer
    {
        private readonly Vector2 padding = new Vector2(2, 2);
        private readonly NodeBase node;
        private CanvasTextLayout textLayout;
        private CanvasTextFormat textFormat;
        private Vector2 renderSize;
        private Vector2 renderPosition;
        private float fontSize;
        private float minSize;

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

        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                if (Math.Abs(fontSize - value) > float.Epsilon)
                {
                    fontSize = value;

                    textFormat = null;
                }
            }
        }

        private CanvasTextFormat TextFormat
        {
            get
            {
                return textFormat ?? (textFormat = new CanvasTextFormat
                {
                    FontSize = fontSize,
                    WordWrapping = CanvasWordWrapping.NoWrap,
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center
                });
            }
        }

        public string OverrideText { get; set; }

        public Win2DTextRenderer(NodeBase node)
        {
            Guard.NotNull(node, nameof(node));

            this.node = node;
        }

        public void Measure(CanvasDrawingSession session)
        {
            minSize = TextFormat.FontSize * 2;

            string text = OverrideText ?? node.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                textLayout = new CanvasTextLayout(session, text, TextFormat, 0.0f, 0.0f);

                renderSize = new Vector2(
                    (float)textLayout.DrawBounds.Width,
                    (float)textLayout.DrawBounds.Height);
            }
            else
            {
                renderSize = Vector2.Zero;
            }

            renderSize.Y = Math.Max(renderSize.Y, minSize);

            MathHelper.Round(ref renderSize);

            if (Math.Abs((renderSize.Y % 2) - 1) < float.Epsilon)
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
            if (!string.IsNullOrWhiteSpace(text))
            {
                session.DrawText(text, Bounds, Colors.Black, TextFormat);
            }
        }
    }
}
