// ==========================================================================
// Win2DAdornerRenderNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using GP.Windows;
using Hercules.Model;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DAdornerRenderNode : Win2DRenderable, IAdornerRenderNode, IResourceHolder
    {
        private readonly IBodyGeometry geometry;
        private readonly Vector2 textSize;
        private bool needsArrange;
        private bool needsMeasure = true;

        public Win2DAdornerRenderNode(NodeBase node, Win2DRenderer renderer, IBodyGeometry geometry, Rect2 bounds, Vector2 textSize)
            : base(node, renderer)
        {
            Guard.NotNull(geometry, nameof(geometry));

            this.geometry = geometry;

            this.textSize = textSize;

            UpdateSize(bounds.Size);
            UpdatePosition(bounds.Position);
        }

        public void ClearResources()
        {
            geometry.ClearResources();
        }

        public void MoveTo(Vector2 position)
        {
            UpdatePosition(position);
            UpdateBounds();

            needsArrange = true;
        }

        public void MoveBy(Vector2 offset)
        {
            UpdatePosition(RenderPosition + offset);
            UpdateBounds();

            needsArrange = true;
        }

        public void Measure(CanvasDrawingSession session)
        {
            if (needsMeasure)
            {
                geometry.Measure(this, session, textSize);

                needsMeasure = false;
            }
        }

        public void Arrange(CanvasDrawingSession session)
        {
            if (needsArrange)
            {
                geometry.Arrange(this, session);

                needsArrange = false;
            }
        }

        public void Render(CanvasDrawingSession session)
        {
            geometry.Render(this, session, Resources.FindColor(Node), false);
        }
    }
}
