﻿// ==========================================================================
// Win2DRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using Windows.UI;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderNode : Win2DRenderable, IRenderNode, IResourceHolder
    {
        private readonly Win2DTextRenderer textRenderer;
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
            get { return textRenderer; }
        }

        public virtual Vector2 RenderPositionOffset
        {
            get { return bodyGeometry.RenderPositionOffset; }
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

            textRenderer = new Win2DTextRenderer(node);
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
            session.DrawRectangle(RenderBounds, Colors.Green);

            if (Parent != null)
            {
                session.DrawRectangle(RenderBoundsWithParent, Colors.Blue);
            }
#endif
            bodyGeometry.Render(this, session, Resources.FindColor(Node), renderControls);

            if (bodyGeometry.HasText)
            {
                textRenderer.Render(session);
            }

            if (renderControls)
            {
                if (Node.HasChildren)
                {
                    button.Render(session);
                }
            }
        }

        public void MoveToLayout(Vector2 position, AnchorPoint anchor)
        {
            layoutPosition = position;

            Vector2 offset = RenderPositionOffset;

            targetLayoutPosition = new Vector2(
                layoutPosition.X + offset.X,
                layoutPosition.Y + offset.Y - (0.5f * RenderSize.Y));

            if (anchor == AnchorPoint.Right)
            {
                targetLayoutPosition.X -= RenderSize.X;
            }
            else if (anchor == AnchorPoint.Center)
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

        public void Measure(CanvasDrawingSession session)
        {
            textRenderer.Measure(session);

            if (bodyGeometry != null)
            {
                UpdateSize(bodyGeometry.Measure(this, session, textRenderer.RenderSize));
            }
        }

        public void Arrange(CanvasDrawingSession session)
        {
            ArrangeBody(session);
            ArrangeText();
            ArrangeHull(session);
            ArrangePath(session);
            ArrangeButton();
        }

        public void ArrangeHull(CanvasDrawingSession session)
        {
            if (hullGeometry != null)
            {
                hullGeometry.Arrange(this, session);
            }
        }

        public void ArrangePath(CanvasDrawingSession session)
        {
            if (pathGeometry != null)
            {
                pathGeometry.Arrange(this, session);
            }
        }

        private void ArrangeBody(CanvasDrawingSession session)
        {
            UpdateBounds();

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

            bodyGeometry.Arrange(this, session);
        }

        private void ArrangeText()
        {
            if (bodyGeometry.HasText)
            {
                textRenderer.Arrange(bodyGeometry.TextRenderPosition);
            }
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

        public virtual void ClearResources()
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

        public void ComputeBody(CanvasDrawingSession session)
        {
            IBodyGeometry body = CreateBody(session, bodyGeometry);

            if (body != null)
            {
                ClearBody();

                bodyGeometry = body;
            }
        }

        public void ComputePath(CanvasDrawingSession session)
        {
            IPathGeometry path = CreatePath(session, pathGeometry);

            if (path != null)
            {
                ClearPath();

                pathGeometry = path;
            }
        }

        public void ComputeHull(CanvasDrawingSession session)
        {
            IHullGeometry hull = CreateHull(session, hullGeometry);

            if (hull != null)
            {
                ClearHull();

                hullGeometry = hull;
            }
        }

        protected abstract IBodyGeometry CreateBody(CanvasDrawingSession session, IBodyGeometry current);

        protected abstract IHullGeometry CreateHull(CanvasDrawingSession session, IHullGeometry current);

        protected abstract IPathGeometry CreatePath(CanvasDrawingSession session, IPathGeometry current);

        public Win2DAdornerRenderNode CreateAdorner()
        {
            return new Win2DAdornerRenderNode(Node, Renderer, bodyGeometry, RenderBounds, textRenderer.RenderSize);
        }
    }
}
