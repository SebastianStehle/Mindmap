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
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderNode : Win2DRenderable, IRenderNode
    {
        private readonly ExpandButton button;
        private IBodyGeometry bodyGeometry;
        private IPathGeometry pathGeometry;
        private IHullGeometry hullGeometry;
        private Vector2 animationTargetPosition = MathHelper.PositiveInfinityVector;
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
            button = new ExpandButton(node);
        }

        public void Hide()
        {
            if (isVisible)
            {
                isVisible = false;

                animationTargetPosition = MathHelper.PositiveInfinityVector;
            }
        }

        public void Show()
        {
            if (!isVisible)
            {
                isVisible = true;

                animationTargetPosition = MathHelper.PositiveInfinityVector;
            }
        }

        public void RenderPath(CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                pathGeometry.Render(this, session);
            }
        }

        public void RenderHull(CanvasDrawingSession session)
        {
            if (hullGeometry != null)
            {
                hullGeometry.Render(this, session, Resources.FindColor(Node));
            }
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

            if (renderControls)
            {
                if (Node.HasChildren)
                {
                    button.Render(session);
                }
            }
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
                if (isAnimating && animationTargetPosition != MathHelper.PositiveInfinityVector)
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

            animationTargetPosition = MathHelper.PositiveInfinityVector;

            return true;
        }

        public void Measure(ICanvasResourceCreator resourceCreator)
        {
            if (bodyGeometry != null)
            {
                UpdateSize(bodyGeometry.Measure(this, resourceCreator));
            }
        }

        public void ArrangeBodyAndButton(ICanvasResourceCreator resourceCreator)
        {
            ArrangeBody(resourceCreator);

            ArrangeButton();
        }

        public void ArrangeHull(ICanvasResourceCreator resourceCreator)
        {
            if (hullGeometry != null)
            {
                hullGeometry.Arrange(this, resourceCreator);
            }
        }

        public void ArrangePath(ICanvasResourceCreator resourceCreator)
        {
            if (pathGeometry != null)
            {
                pathGeometry.Arrange(this, resourceCreator);
            }
        }

        private void ArrangeBody(ICanvasResourceCreator resourceCreator)
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

        private void ArrangeButton()
        {
            Vector2 buttonPosition;

            if (Node.NodeSide == NodeSide.Left)
            {
                buttonPosition = new Vector2(
                    RenderPosition.X - 2,
                    RenderPosition.Y + (RenderSize.Y * 0.5f));
            }
            else
            {
                buttonPosition = new Vector2(
                    RenderPosition.X + RenderSize.X + 2,
                    RenderPosition.Y + (RenderSize.Y * 0.5f));
            }

            button.Arrange(buttonPosition);
        }

        public virtual bool HitTest(Vector2 hitPosition)
        {
            return RenderBounds.Contains(hitPosition);
        }

        public virtual bool HandleClick(Vector2 hitPosition)
        {
            if (button.HitTest(hitPosition))
            {
                return true;
            }

            if (HitTest(hitPosition))
            {
                Node.Select();

                return true;
            }

            return false;
        }

        public override void ClearResources()
        {
            ClearPath();
            ClearHull();
            ClearBody();
        }

        private void ClearBody()
        {
            if (bodyGeometry != null)
            {
                bodyGeometry.ClearResources();
                bodyGeometry = null;
            }
        }

        private void ClearHull()
        {
            if (hullGeometry != null)
            {
                hullGeometry.ClearResources();
                hullGeometry = null;
            }
        }

        private void ClearPath()
        {
            if (pathGeometry != null)
            {
                pathGeometry.ClearResources();
                pathGeometry = null;
            }
        }

        public void ComputeBody(ICanvasResourceCreator resourceCreator)
        {
            IBodyGeometry body = CreateBody(resourceCreator, bodyGeometry);

            if (body != null)
            {
                ClearBody();

                bodyGeometry = body;
            }
        }

        public void ComputePath(ICanvasResourceCreator resourceCreator)
        {
            IPathGeometry path = CreatePath(resourceCreator, pathGeometry);

            if (path != null)
            {
                ClearPath();

                pathGeometry = path;
            }
        }

        public void ComputeHull(ICanvasResourceCreator resourceCreator)
        {
            IHullGeometry hull = CreateHull(resourceCreator, hullGeometry);

            if (hull != null)
            {
                ClearHull();

                hullGeometry = hull;
            }
        }

        protected abstract IBodyGeometry CreateBody(ICanvasResourceCreator resourceCreator, IBodyGeometry current);

        protected abstract IHullGeometry CreateHull(ICanvasResourceCreator resourceCreator, IHullGeometry current);

        protected abstract IPathGeometry CreatePath(ICanvasResourceCreator resourceCreator, IPathGeometry current);

        public Win2DAdornerRenderNode CreateAdorner()
        {
            return new Win2DAdornerRenderNode(Node, Renderer, bodyGeometry.Clone(), RenderBounds);
        }
    }
}
