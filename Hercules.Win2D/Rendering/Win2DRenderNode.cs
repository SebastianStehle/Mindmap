// ==========================================================================
// Win2DRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Numerics;
using GP.Windows;
using GP.Windows.UI;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Hercules.Win2D.Rendering.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public abstract class Win2DRenderNode : IRenderNode
    {
        private static readonly Vector2 EmptyVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        private readonly Win2DRenderer renderer;
        private readonly NodeBase node;
        private readonly Win2DTextRenderer textRenderer;
        private readonly ExpandButton button;
        private IBodyGeometry bodyGeometry;
        private IPathGeometry pathGeometry;
        private IHullGeometry hullGeometry;
        private Vector2 animationTargetPosition = EmptyVector;
        private Vector2 position;
        private Vector2 renderPosition;
        private Vector2 renderSize;
        private Vector2 targetPosition;
        private Rect2 bounds;
        private Rect2 boundsWithParent;
        private DateTime? animatingEndUtc;
        private bool isVisible = true;

        public Win2DRenderNode Parent { get; set; }

        public Win2DRenderer Renderer
        {
            get { return renderer; }
        }

        public Win2DResourceManager Resources
        {
            get { return renderer.Resources; }
        }

        public Win2DScene Scene
        {
            get { return renderer.Scene; }
        }

        public NodeBase Node
        {
            get { return node; }
        }

        public Rect2 Bounds
        {
            get { return bounds; }
        }

        public Rect2 BoundsWithParent
        {
            get { return boundsWithParent; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 TargetPosition
        {
            get { return targetPosition; }
        }

        public Vector2 RenderPosition
        {
            get { return renderPosition; }
        }

        public Vector2 RenderSize
        {
            get { return renderSize; }
        }

        public Win2DTextRenderer TextRenderer
        {
            get { return textRenderer; }
        }

        public virtual Vector2 RenderPositionOffset
        {
            get { return bodyGeometry.RenderPositionOffset; }
        }

        public virtual float VerticalPathOffset
        {
            get { return bodyGeometry.VerticalPathOffset; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
        }

        public bool HideControls { get; set; }

        protected Win2DRenderNode(NodeBase node, Win2DRenderer renderer)
        {
            Guard.NotNull(node, nameof(node));
            Guard.NotNull(renderer, nameof(renderer));

            this.node = node;

            this.renderer = renderer;

            button = new ExpandButton(node);

            textRenderer = new Win2DTextRenderer(node);
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
            if (pathGeometry != null)
            {
                pathGeometry.Render(this, session);
            }
        }

        public void RenderHull(CanvasDrawingSession session)
        {
            if (hullGeometry != null)
            {
                pathGeometry.Render(this, session);
            }
        }

        public void Render(CanvasDrawingSession session, bool renderControls)
        {
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

        public void MoveToLayout(Vector2 layoutPosition, AnchorPoint anchor)
        {
            position = layoutPosition;

            Vector2 offset = RenderPositionOffset;

            targetPosition = new Vector2(
                position.X + offset.X,
                position.Y + offset.Y - (0.5f * renderSize.Y));

            if (anchor == AnchorPoint.Right)
            {
                targetPosition.X -= renderSize.X;
            }
            else if (anchor == AnchorPoint.Center)
            {
                targetPosition.X -= 0.5f * renderSize.X;
            }
        }

        public bool AnimateRenderPosition(bool isAnimating, DateTime utcNow, float animationSpeed)
        {
            if (IsVisible)
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
            textRenderer.Measure(session);

            if (bodyGeometry != null)
            {
                renderSize = bodyGeometry.Measure(this, session, textRenderer.RenderSize);
            }
        }

        public void Arrange(CanvasDrawingSession session)
        {
            ArrangeBody(session);
            ArrangeText();
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
            return Bounds.Contains(hitPosition);
        }

        public virtual bool HandleClick(Vector2 hitPosition)
        {
            if (button.HitTest(hitPosition))
            {
                return true;
            }

            if (HitTest(hitPosition))
            {
                node.Select();

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
    }
}
