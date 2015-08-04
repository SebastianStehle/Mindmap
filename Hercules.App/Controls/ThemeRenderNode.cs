using System.Numerics;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.App.Controls
{
    public abstract class ThemeRenderNode : IRenderNode
    {
        private readonly ThemeRenderer renderer;
        private Vector2 renderPosition;
        private Vector2 size;
        private NodeBase node;
        private bool isVisible = true;

        public ThemeRenderNode Parent { get; internal set; }

        public ThemeRenderer Renderer
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

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
        }

        public Rect2 Bounds
        {
            get
            {
                return new Rect2(renderPosition, size);
            }
        }

        public Vector2 Position
        {
            get
            {
                return renderPosition;
            }
        }

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public abstract Vector2 AnchorPosition { get; }

        protected ThemeRenderNode(NodeBase node, ThemeRenderer renderer)
        {
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(node, nameof(node));

            this.renderer = renderer;

            this.node = node;
        }

        public void MoveTo(Vector2 position, AnchorPoint anchor)
        {
            renderPosition = position;

            renderPosition.Y -= 0.5f * size.Y;

            if (anchor == AnchorPoint.Right)
            {
                renderPosition.X -= size.X;
            }
            else if (anchor == AnchorPoint.Center)
            {
                renderPosition.X -= 0.5f * size.X;
            }
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

        public void Measure(CanvasDrawingSession session)
        {
            size = MeasureInternal(session);
        }

        public abstract bool HitTest(Vector2 position);

        protected abstract void RenderPathInternal(CanvasDrawingSession session);

        protected abstract void RenderInternal(CanvasDrawingSession session, ThemeColor color);

        protected abstract Vector2 MeasureInternal(CanvasDrawingSession session);
    }
}
