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
    public sealed class Win2DAdornerRenderNode : Win2DRenderable, IAdornerRenderNode
    {
        private readonly IBodyGeometry geometry;
        private bool needsArrange;
        private bool needsMeasure = true;

        public Win2DAdornerRenderNode(NodeBase node, Win2DRenderer renderer, IBodyGeometry geometry, Rect2 bounds)
            : base(node, renderer)
        {
            Guard.NotNull(geometry, nameof(geometry));

            this.geometry = geometry;

            UpdateSize(bounds.Size);
            UpdatePosition(bounds.Position);
        }

        public override void ClearResources()
        {
            geometry.ClearResources();
        }

        public void MoveTo(Vector2 position)
        {
            UpdatePosition(position);

            needsArrange = true;
        }

        public void MoveBy(Vector2 offset)
        {
            UpdatePosition(RenderPosition + offset);

            needsArrange = true;
        }

        public void Measure(ICanvasResourceCreator resourceCreator)
        {
            if (needsMeasure)
            {
                geometry.Measure(this, resourceCreator);

                needsMeasure = false;
            }
        }

        public void Arrange(ICanvasResourceCreator resourceCreator)
        {
            if (needsArrange)
            {
                geometry.Arrange(this, resourceCreator);

                needsArrange = false;
            }
        }

        public void Render(CanvasDrawingSession session)
        {
            geometry.Render(this, session, Resources.FindColor(Node), false);
        }
    }
}
