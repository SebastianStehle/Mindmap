// ==========================================================================
// ThemeRenderNode.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using GP.Windows;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Model.Rendering.Win2D
{
    public abstract class Win2DRenderNode : IRenderNode
    {
        private readonly Win2DRenderer renderer;
        private readonly NodeBase node;
        private Vector2 position;
        private Vector2 renderPosition;
        private Vector2 renderSize;
        private Rect2 boundsWithParent;
        private bool isVisible = true;

        public Win2DRenderNode Parent { get; set; }

        public Win2DRenderer Renderer
        {
            get
            {
                return renderer;
            }
        }

        public NodeBase Node
        {
            get
            {
                return node;
            }
        }

        public Rect2 Bounds
        {
            get
            {
                return new Rect2(renderPosition, renderSize);
            }
        }

        public Rect2 BoundsWithParent
        {
            get
            {
                return boundsWithParent;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public Vector2 RenderPosition
        {
            get
            {
                return renderPosition;
            }
        }

        public Vector2 RenderSize
        {
            get
            {
                return renderSize;
            }
        }

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
        }

        public bool HideControls { get; set; }

        public abstract Win2DTextRenderer TextRenderer { get; }

        protected Win2DRenderNode(NodeBase node, Win2DRenderer renderer)
        {
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(node, nameof(node));

            this.renderer = renderer;

            this.node = node;
        }

        public void MoveBy(Vector2 offset)
        {
            renderPosition += offset;
        }

        public void MoveTo(Vector2 layoutPosition, AnchorPoint anchor)
        {
            this.position = layoutPosition;

            renderPosition = position;

            renderPosition.Y -= 0.5f * renderSize.Y;

            if (anchor == AnchorPoint.Right)
            {
                renderPosition.X -= renderSize.X;
            }
            else if (anchor == AnchorPoint.Center)
            {
                renderPosition.X -= 0.5f * renderSize.X;
            }
        }

        public Win2DRenderNode CloneUnlinked()
        {
            Win2DRenderNode clone = CloneInternal();

            clone.HideControls = true;
            clone.Parent = null;
            clone.renderSize = renderSize;
            clone.renderPosition = renderPosition;
            clone.boundsWithParent = boundsWithParent;

            return clone;
        }

        public void Hide()
        {
            isVisible = false;
        }

        public void Show()
        {
            isVisible = true;
        }

        public void RenderPath(CanvasDrawingSession session)
        {
            RenderPathInternal(session);
        }

        public void Render(CanvasDrawingSession session)
        {
            RenderInternal(session, renderer.FindColor(node));
        }

        public void Arrange(CanvasDrawingSession session)
        {
            ArrangeInternal(session);
                        
            if (Parent != null)
            {
                double minX = Math.Min(renderPosition.X, Parent.RenderPosition.X);
                double minY = Math.Min(renderPosition.Y, Parent.RenderPosition.Y);

                double maxX = Math.Max(renderPosition.X + renderSize.X, Parent.RenderPosition.X + Parent.RenderSize.X);
                double maxY = Math.Max(renderPosition.Y + renderSize.Y, Parent.RenderPosition.Y + Parent.RenderSize.Y);

                boundsWithParent = new Rect2((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
            }
            else
            {
                boundsWithParent = Bounds;
            }
        }

        public void Measure(CanvasDrawingSession session)
        {
            renderSize = MeasureInternal(session);
        }

        public virtual bool HandleClick(Vector2 position)
        {
            if (HitTest(position))
            {
                node.Select();

                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void ArrangeInternal(CanvasDrawingSession session)
        {
        }

        protected virtual void RenderPathInternal(CanvasDrawingSession session)
        {

        }

        public abstract bool HitTest(Vector2 position);

        protected abstract void RenderInternal(CanvasDrawingSession session, ThemeColor color);

        protected abstract Win2DRenderNode CloneInternal();

        protected abstract Vector2 MeasureInternal(CanvasDrawingSession session);
    }
}
