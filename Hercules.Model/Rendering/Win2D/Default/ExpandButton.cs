using System.Numerics;
using GP.Windows;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public sealed class ExpandButton
    {
        private readonly NodeBase node;
        private float renderRadius = 20;
        private Rect2 renderBounds;
        private Vector2 renderCenter;

        public ExpandButton(NodeBase node)
        {
            Guard.NotNull(node, nameof(Node));

            this.node = node;
        }

        public void Arrange(Vector2 center, float radius = 10)
        {
            renderRadius = radius;
            renderCenter = center;

            renderBounds = Rect2.Inflate(new Rect2(center, Vector2.Zero), radius, radius);
        }

        public bool HitTest(Vector2 mousePosition)
        {
            if (renderBounds.Contains(mousePosition))
            {
                string transactionName = ResourceManager.GetString("ExpandCollapseTransactionName");

                node.Document.MakeTransaction(transactionName, commands =>
                {
                    commands.Apply(new ToggleCollapseCommand(node));
                });

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Render(CanvasDrawingSession session)
        {
            if (node.HasChildren)
            {
                RenderCircle(session);

                float halfRadius = 0.5f * renderRadius;

                RenderHorizontal(session, halfRadius);

                if (node.IsCollapsed)
                {
                    RenderVertical(session, halfRadius);
                }
            }
        }

        private void RenderCircle(CanvasDrawingSession session)
        {
            session.FillCircle(renderCenter, renderRadius, Colors.White);

            session.DrawCircle(renderCenter, renderRadius, Colors.DarkGray);
        }

        private void RenderVertical(CanvasDrawingSession session, float halfRadius)
        {
            Vector2 verticalTop = new Vector2(
                renderCenter.X,
                renderCenter.Y - halfRadius);
            Vector2 verticalBottom = new Vector2(
                renderCenter.X,
                renderCenter.Y + halfRadius);

            session.DrawLine(verticalTop, verticalBottom, Colors.DarkGray, 2f);
        }

        private void RenderHorizontal(CanvasDrawingSession session, float halfRadius)
        {
            Vector2 horizontalLeft = new Vector2(
                renderCenter.X - halfRadius,
                renderCenter.Y);

            Vector2 horizontalRight = new Vector2(
                renderCenter.X + halfRadius,
                renderCenter.Y);

            session.DrawLine(horizontalLeft, horizontalRight, Colors.DarkGray, 2f);
        }
    }
}
