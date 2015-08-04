using Hercules.Model;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using Windows.UI;

namespace Hercules.App.Controls
{
    public sealed class TextRenderer
    {
        private readonly Vector2 padding = new Vector2(5, 5);
        private readonly NodeBase node;
        private readonly CanvasTextFormat textFormat;
        private CanvasTextLayout textLayout;
        private Vector2 textSize;
        private Vector2 textOffset;
        private Vector2 position;
        private bool isVisible;

        public Vector2 TextSize
        {
            get
            {
                return textSize;
            }
        }

        public string OverrideText { get; set; }

        public TextRenderer(float fontSize, NodeBase node)
        {
            this.node = node;

            textFormat = new CanvasTextFormat { FontSize = fontSize, WordWrapping = CanvasWordWrapping.NoWrap, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center };
        }

        public void Measure(CanvasDrawingSession session)
        {
            float minSize = textFormat.FontSize * 2;

            string text = OverrideText ?? node.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                textLayout = new CanvasTextLayout(session, text, textFormat, 0.0f, 0.0f);

                textSize = new Vector2(
                    (float)textLayout.DrawBounds.Width,
                    (float)textLayout.DrawBounds.Height);

                textOffset = new Vector2(
                    (float)-textLayout.DrawBounds.X,
                    (float)-textLayout.DrawBounds.Y);
            }

            textSize.X = Math.Max(textSize.X, minSize);
            textSize.Y = Math.Max(textSize.Y, minSize);

            textSize += padding;
        }

        public void Arrange(Vector2 position)
        {
            this.position = position;
        }

        public void Render(CanvasDrawingSession session)
        {
            string text = (OverrideText ?? node.Text) ?? string.Empty;

            session.DrawText(text, position.X, position.Y, textSize.X, textSize.Y, Colors.Black, textFormat);
        }
    }
}
