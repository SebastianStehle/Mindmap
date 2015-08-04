using GP.Windows;
using Hercules.Model;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;

namespace Hercules.App.Controls.Default
{
    public sealed class ExpandButton
    {
        private readonly NodeBase node;
        private float radius = 20;
        private Rect2 bounds;
        private Vector2 center;

        public ExpandButton(NodeBase node)
        {
            Guard.NotNull(node, nameof(Node));

            this.node = node;
        }

        public void Arrange(Vector2 center, float radius = 10)
        {
            this.radius = radius;
            this.center = center;

            bounds = Rect2.Inflate(new Rect2(center, Vector2.Zero), radius, radius);
        }

        public bool HitTest(Vector2 mousePosition)
        {
            if (bounds.Contains(mousePosition))
            {
                node.Document.MakeTransaction("Toggle", c =>
                {
                    c.Apply(new ToggleCollapseCommand(node));
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
                session.FillCircle(center, radius, Colors.White);
                session.DrawCircle(center, radius, Colors.DarkGray, 1);

                float halfRadius = 0.5f * radius;

                Vector2 left = new Vector2(
                    center.X - halfRadius, 
                    center.Y);
                Vector2 right = new Vector2(
                    center.X + halfRadius,
                    center.Y);

                session.DrawLine(left, right, Colors.DarkGray, 2f);

                if (node.IsCollapsed)
                {
                    Vector2 top = new Vector2(
                        center.X,
                        center.Y - halfRadius);
                    Vector2 bottom = new Vector2(
                        center.X,
                        center.Y + halfRadius);

                    session.DrawLine(top, bottom, Colors.DarkGray, 2f);
                }
            }
        }
    }
}
