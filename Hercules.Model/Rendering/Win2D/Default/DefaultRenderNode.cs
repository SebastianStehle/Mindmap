// ==========================================================================
// DefaultRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;

namespace Hercules.Model.Rendering.Win2D.Default
{
    public abstract class DefaultRenderNode : Win2DRenderNode
    {
        protected static readonly Color PathColor = Color.FromArgb(255, 50, 50, 50);
        protected static readonly Vector2 ImageSizeLarge = new Vector2(64, 64);
        protected static readonly Vector2 ImageSizeSmall = new Vector2(32, 32);
        protected static readonly float ImageMargin = 10;
        protected static readonly CanvasStrokeStyle SelectionStrokeStyle = new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };

        private readonly ExpandButton button;

        protected ExpandButton Button
        {
            get { return button; }
        }

        protected DefaultRenderNode(NodeBase node, DefaultRenderer renderer)
            : base(node, renderer)
        {
            button = new ExpandButton(node);
        }

        public override bool HandleClick(Vector2 hitPosition)
        {
            return button.HitTest(hitPosition) || base.HandleClick(hitPosition);
        }

        protected override void ArrangeInternal(CanvasDrawingSession session)
        {
            Vector2 buttonPosition;

            if (Node.NodeSide == NodeSide.Left)
            {
                buttonPosition = new Vector2(
                    RenderPosition.X - 2,
                    RenderPosition.Y + RenderSize.Y * 0.5f);
            }
            else
            {
                buttonPosition = new Vector2(
                    RenderPosition.X + RenderSize.X + 2,
                    RenderPosition.Y + RenderSize.Y * 0.5f);
            }

            button.Arrange(buttonPosition);

            base.ArrangeInternal(session);
        }
    }
}
