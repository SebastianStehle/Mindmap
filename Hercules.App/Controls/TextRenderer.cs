using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Hercules.App.Controls
{
    public sealed class TextRenderer
    {
        private readonly Vector2 padding = new Vector2(5, 5);
        private readonly NodeBase node;
        private readonly CanvasTextFormat textFormat;
        private CanvasTextLayout textLayout;
        private Vector2 renderSize;
        private Vector2 renderPosition;
        private TextBox editTextBox;
        private float minSize;

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

        public TextRenderer(float fontSize, NodeBase node)
        {
            this.node = node;

            textFormat = new CanvasTextFormat { FontSize = fontSize, WordWrapping = CanvasWordWrapping.NoWrap, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center };
        }

        public void Measure(CanvasDrawingSession session)
        {
            minSize = textFormat.FontSize * 2;

            string text = editTextBox != null ? editTextBox.Text : node.Text;

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
            string text = editTextBox != null ? editTextBox.Text : node.Text;
#if DRAW_OUTLINE
            session.DrawRectangle(Bounds, Colors.Red);
#endif
            if (editTextBox == null)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    session.DrawText(text, Bounds, Colors.Black, textFormat);
                }
            }
            else
            {
                UpdateAlignment();
            }
        }

        private void UpdateAlignment()
        {
            bool isHandled = false;

            if (renderSize.X > minSize + padding.X)
            {
                if (node.NodeSide == NodeSide.Right)
                {
                    editTextBox.TextAlignment = TextAlignment.Left;

                    isHandled = true;
                }
                else if (node.NodeSide == NodeSide.Left)
                {
                    editTextBox.TextAlignment = TextAlignment.Left;

                    isHandled = true;
                }
            }
            
            if (!isHandled)
            {
                editTextBox.TextAlignment = TextAlignment.Center;
            }
        }

        public void BeginEdit(TextBox textBox)
        {
            editTextBox = textBox;

            textBox.FontWeight = textFormat.FontWeight;
            textBox.FontStyle = textFormat.FontStyle;
            textBox.FontSize = textFormat.FontSize;

            textBox.Text = node.Text ?? string.Empty;

            UpdateAlignment();
        }

        public void EndEdit()
        {
            editTextBox = null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OverrideText = ((TextBox)sender).Text;
        }
    }
}
