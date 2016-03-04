// ==========================================================================
// Win2DRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.UI;
using GP.Utils.Mathematics;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Win2D.Rendering.Parts;
using Microsoft.Graphics.Canvas;

// ReSharper disable InvertIf

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderNode : Win2DRenderable, IRenderNode
    {
        private IBodyPart bodyGeometry;
        private IPathPart pathGeometry;
        private IHullPart hullGeometry;
        private Vector2 animationTargetPosition = MathHelper.PositiveInfinityVector2;
        private Vector2 layoutPosition;
        private Vector2 targetLayoutPosition;
        private Rect2 renderBoundsWithParent;
        private DateTime? animatingEndUtc;
        private bool isVisible = true;

        public Win2DRenderNode Parent { get; set; }

        public Rect2 RenderBoundsWithParent
        {
            get { return renderBoundsWithParent; }
        }

        public Vector2 LayoutPosition
        {
            get { return layoutPosition; }
        }

        public Vector2 TargetLayoutPosition
        {
            get { return targetLayoutPosition; }
        }

        public Win2DTextRenderer TextRenderer
        {
            get { return bodyGeometry.TextRenderer; }
        }

        public virtual float VerticalPathRenderOffset
        {
            get { return bodyGeometry.VerticalPathOffset; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
        }

        protected Win2DRenderNode(NodeBase node, Win2DRenderer renderer)
            : base(node, renderer)
        {
        }

        public void Hide()
        {
            if (isVisible)
            {
                isVisible = false;

                animationTargetPosition = MathHelper.PositiveInfinityVector2;
            }
        }

        public void Show()
        {
            if (!isVisible)
            {
                isVisible = true;

                animationTargetPosition = MathHelper.PositiveInfinityVector2;
            }
        }

        public void RenderPath(CanvasDrawingSession session)
        {
            pathGeometry?.Render(this, session, Resources.FindColor(Node), false);
        }

        public void RenderHull(CanvasDrawingSession session)
        {
            hullGeometry?.Render(this, session, Resources.FindColor(Node), false);
        }

        public void Render(CanvasDrawingSession session, bool renderControls)
        {
#if DRAW_OUTLINE
            session.DrawRectangle(RenderBounds.ToRect(), Colors.Green);

            if (Parent != null)
            {
                session.DrawRectangle(RenderBoundsWithParent.ToRect(), Colors.Blue);
            }
#endif
            bodyGeometry.Render(this, session, Resources.FindColor(Node), renderControls);
        }

        public void MoveToLayout(Vector2 position, NodeSide anchor)
        {
            layoutPosition = position;

            targetLayoutPosition = new Vector2(
                layoutPosition.X,
                layoutPosition.Y - (0.5f * RenderSize.Y));

            if (anchor == NodeSide.Right)
            {
                targetLayoutPosition.X -= RenderSize.X;
            }
            else if (anchor == NodeSide.Auto)
            {
                targetLayoutPosition.X -= 0.5f * RenderSize.X;
            }
        }

        public bool AnimateRenderPosition(bool isAnimating, DateTime utcNow, float animationSpeed)
        {
            if (IsVisible)
            {
                if (isAnimating && animationTargetPosition != MathHelper.PositiveInfinityVector2)
                {
                    if (animationTargetPosition != targetLayoutPosition)
                    {
                        animatingEndUtc = utcNow.AddMilliseconds(animationSpeed);
                    }
                }
                else
                {
                    animatingEndUtc = null;
                }

                animationTargetPosition = targetLayoutPosition;

                float fractionComplete = 1;

                if (animatingEndUtc.HasValue)
                {
                    float timeRemaining = (float)(animatingEndUtc.Value - utcNow).TotalMilliseconds;

                    fractionComplete -= Math.Min(1, Math.Max(0, timeRemaining / animationSpeed));
                }

                UpdatePosition(
                    new Vector2(
                        MathHelper.Interpolate(fractionComplete, RenderPosition.X, targetLayoutPosition.X),
                        MathHelper.Interpolate(fractionComplete, RenderPosition.Y, targetLayoutPosition.Y)));

                return !MathHelper.AboutEqual(targetLayoutPosition, RenderPosition);
            }

            animationTargetPosition = MathHelper.PositiveInfinityVector2;

            return false;
        }

        public void Measure(ICanvasResourceCreator resourceCreator)
        {
            if (bodyGeometry != null)
            {
                UpdateSize(bodyGeometry.Measure(this, resourceCreator));
            }
        }

        public void ArrangeHull(ICanvasResourceCreator resourceCreator)
        {
            hullGeometry?.Arrange(this, resourceCreator);
        }

        public void ArrangePath(ICanvasResourceCreator resourceCreator)
        {
            pathGeometry?.Arrange(this, resourceCreator);
        }

        public void ArrangeBody(ICanvasResourceCreator resourceCreator)
        {
            if (Parent != null)
            {
                double minX = Math.Min(RenderPosition.X, Parent.RenderPosition.X);
                double minY = Math.Min(RenderPosition.Y, Parent.RenderPosition.Y);

                double maxX = Math.Max(RenderPosition.X + RenderSize.X, Parent.RenderPosition.X + Parent.RenderSize.X);
                double maxY = Math.Max(RenderPosition.Y + RenderSize.Y, Parent.RenderPosition.Y + Parent.RenderSize.Y);

                renderBoundsWithParent = new Rect2((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
            }
            else
            {
                renderBoundsWithParent = RenderBounds;
            }

            bodyGeometry.Arrange(this, resourceCreator);
        }

        public virtual HitResult HitTest(Vector2 hitPosition)
        {
            return bodyGeometry?.HitTest(this, hitPosition) ?? (RenderBounds.Contains(hitPosition) ? new HitResult(this, HitTarget.Node) : null);
        }

        public override void ClearResources()
        {
            ClearPath();
            ClearHull();
            ClearBody();
        }

        private void ClearBody()
        {
            bodyGeometry?.ClearResources();
            bodyGeometry = null;
        }

        private void ClearHull()
        {
            hullGeometry?.ClearResources();
            hullGeometry = null;
        }

        private void ClearPath()
        {
            pathGeometry?.ClearResources();
            pathGeometry = null;
        }

        public void ComputeBody(ICanvasResourceCreator resourceCreator)
        {
            IBodyPart body = CreateBody(resourceCreator, bodyGeometry);

            if (body != null)
            {
                ClearBody();

                bodyGeometry = body;
            }
        }

        public void ComputePath(ICanvasResourceCreator resourceCreator)
        {
            IPathPart path = CreatePath(resourceCreator, pathGeometry);

            if (path != null)
            {
                ClearPath();

                pathGeometry = path;
            }
        }

        public void ComputeHull(ICanvasResourceCreator resourceCreator)
        {
            IHullPart hull = CreateHull(resourceCreator, hullGeometry);

            if (hull != null)
            {
                ClearHull();

                hullGeometry = hull;
            }
        }

        protected abstract IBodyPart CreateBody(ICanvasResourceCreator resourceCreator, IBodyPart current);

        protected abstract IHullPart CreateHull(ICanvasResourceCreator resourceCreator, IHullPart current);

        protected abstract IPathPart CreatePath(ICanvasResourceCreator resourceCreator, IPathPart current);

        public Win2DAdornerRenderNode CreateAdorner()
        {
            return new Win2DAdornerRenderNode(Node, Renderer, bodyGeometry.Clone(), RenderBounds);
        }
    }
}
