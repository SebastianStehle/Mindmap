// ==========================================================================
// NotesButton.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Globalization;
using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace Hercules.Win2D.Rendering.Utils
{
    public sealed class NotesButton
    {
        private static readonly string Glyph;
        private static readonly Vector2 Size = new Vector2(16, 16);
        private static readonly Vector2 Padding = new Vector2(4, 4);
        private static readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            FontFamily = "Segoe MDL2 Assets",
            FontSize = 16,
            FontWeight = Windows.UI.Text.FontWeights.ExtraBold
        };

        private Rect2 renderBounds;
        private Vector2 renderPosition;

        static NotesButton()
        {
            int code = int.Parse("E70F", NumberStyles.HexNumber);

            Glyph = char.ConvertFromUtf32(code);
        }

        public HitResult HitTest(Win2DRenderNode renderNode, Vector2 hitPosition)
        {
            return renderBounds.Contains(hitPosition) ? new HitResult(renderNode, HitTarget.NotesButton) : null;
        }

        public void Arrange(Vector2 rightBottomPosition)
        {
            renderBounds = new Rect2(
                new Vector2(
                    rightBottomPosition.X,
                    rightBottomPosition.Y - (2 * Padding.Y) - Size.Y),
                Size + (2 * Padding));

            renderPosition = renderBounds.Position + Padding;
        }

        public void Render(Win2DRenderable renderable, CanvasDrawingSession session)
        {
#if DRAW_OUTLINE
            session.DrawRectangle(renderBounds.ToRect(), Colors.Turquoise);
#endif
            Color color = renderable.Node.HasNotes ? Colors.Black : Colors.Gray;

            session.DrawText(Glyph, renderPosition.X, renderPosition.Y, color, TextFormat);
        }
    }
}
