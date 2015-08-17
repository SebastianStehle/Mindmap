// ==========================================================================
// Win2DRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Linq;
using System.Numerics;
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Hercules.Model.Rendering.Win2D
{
    public abstract class Win2DRenderNode : IRenderNode
    {
        private static readonly Vector2 EmptyVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        private readonly Win2DRenderer renderer;
        private readonly NodeBase node;
        private Vector2 animationTargetPosition = EmptyVector;
        private Vector2 position;
        private Vector2 renderPosition;
        private Vector2 renderSize;
        private Vector2 targetPosition;
        private Rect2 bounds;
        private Rect2 boundsWithParent;
        private DateTime? animatingEndUtc;
        private bool isMoved;
        private bool isVisible = true;

        public Win2DRenderNode Parent { get; set; }

        public Win2DRenderer Renderer
        {
            get
            {
                return renderer;
            }
        }

        public Win2DResourceManager Resources
        {
            get
            {
                return renderer.Resources;
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
                return bounds;
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

        public virtual Vector2 RenderPositionOffset
        {
            get
            {
                return Vector2.Zero;
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
            Guard.NotNull(node, nameof(node));
            Guard.NotNull(renderer, nameof(renderer));

            this.node = node;

            this.renderer = renderer;
        }

        public Win2DRenderNode CloneUnlinked()
        {
            Win2DRenderNode clone = CloneInternal();

            clone.Parent = null;
            clone.isMoved = true;
            clone.position = position;
            clone.renderSize = renderSize;
            clone.renderPosition = renderPosition;
            clone.targetPosition = targetPosition;
            clone.boundsWithParent = boundsWithParent;

            return clone;
        }

        public void Hide()
        {
            if (isVisible)
            {
                isVisible = false;

                animationTargetPosition = EmptyVector;
            }
        }

        public void Show()
        {
            if (!isVisible)
            {
                isVisible = true;

                animationTargetPosition = EmptyVector;
            }
        }

        public void RenderPath(CanvasDrawingSession session)
        {
            RenderPathInternal(session);
        }

        public void Render(CanvasDrawingSession session, bool renderControls)
        {
#if DRAW_OUTLINE
            session.DrawRectangle(Bounds, Colors.Green);

            if (Parent != null)
            {
                session.DrawRectangle(BoundsWithParent, Colors.Blue);
            }
#endif
            RenderInternal(session, Resources.FindColor(node), renderControls);
        }

        public void MoveBy(Vector2 offset)
        {
            renderPosition += offset;

            isMoved = true;
        }

        public void MoveTo(Vector2 position)
        {
            renderPosition = position;

            isMoved = true;
        }

        public void MoveToLayout(Vector2 layoutPosition, AnchorPoint anchor)
        {
            position = layoutPosition;

            Vector2 offset = RenderPositionOffset;

            targetPosition = new Vector2(
                position.X + offset.X,
                position.Y + offset.Y - 0.5f * renderSize.Y);

            if (anchor == AnchorPoint.Right)
            {
                targetPosition.X -= renderSize.X;
            }
            else if (anchor == AnchorPoint.Center)
            {
                targetPosition.X -= 0.5f * renderSize.X;
            }

            isMoved = false;
        }

        public bool AnimateRenderPosition(bool isAnimating, DateTime utcNow, float animationSpeed)
        {
            if (!isMoved && IsVisible)
            {
                if (isAnimating && animationTargetPosition != EmptyVector)
                {
                    if (animationTargetPosition != targetPosition)
                    {
                        animatingEndUtc = utcNow.AddMilliseconds(animationSpeed);
                    }
                }
                else
                {
                    animatingEndUtc = null;
                }

                animationTargetPosition = targetPosition;

                float fractionComplete = 1;

                if (animatingEndUtc.HasValue)
                {
                    float timeRemaining = (float)(animatingEndUtc.Value - utcNow).TotalMilliseconds;

                    fractionComplete -= Math.Min(1, Math.Max(0, timeRemaining / animationSpeed));
                }

                renderPosition = new Vector2(
                    MathHelper.Interpolate(fractionComplete, renderPosition.X, targetPosition.X),
                    MathHelper.Interpolate(fractionComplete, renderPosition.Y, targetPosition.Y));

                return !MathHelper.AboutEqual(targetPosition, renderPosition);
            }

            animationTargetPosition = EmptyVector;

            return true;
        }

        public void Measure(CanvasDrawingSession session)
        {
            renderSize = MeasureInternal(session);
        }

        public void Arrange(CanvasDrawingSession session)
        {
            bounds = new Rect2(renderPosition, renderSize);

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

            ArrangeInternal(session);
        }

        public virtual bool HitTest(Vector2 position)
        {
            return Bounds.Contains(position);
        }

        public virtual bool HandleClick(Vector2 hitPosition)
        {
            if (HitTest(hitPosition))
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

        protected abstract void RenderInternal(CanvasDrawingSession session, ThemeColor color, bool renderControls);

        protected abstract Win2DRenderNode CloneInternal();

        protected abstract Vector2 MeasureInternal(CanvasDrawingSession session);
    }
}
