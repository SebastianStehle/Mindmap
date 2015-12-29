// ==========================================================================
// NodeMovingOperation.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Numerics;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Hercules.Win2D.Rendering;

// ReSharper disable ArrangeThisQualifier

namespace Hercules.App.Controls
{
    public sealed class NodeMovingOperation
    {
        private readonly Win2DRenderNode renderNode;
        private readonly Win2DRenderer renderer;
        private readonly Document document;
        private readonly Mindmap mindmap;
        private readonly Vector2 initialPosition;
        private readonly Node targetNode;
        private readonly ILayout layout;
        private IAdornerRenderNode movingNode;

        public Mindmap Mindmap
        {
            get
            {
                return mindmap;
            }
        }

        public static NodeMovingOperation Start(Mindmap mindmap, Win2DRenderNode renderNode)
        {
            if (renderNode != null)
            {
                Node movingNode = renderNode.Node as Node;

                if (movingNode != null && movingNode.IsSelected && mindmap.Layout != null && mindmap.Document != null)
                {
                    return new NodeMovingOperation(mindmap, renderNode, movingNode);
                }
            }

            return null;
        }

        internal NodeMovingOperation(Mindmap mindmap, Win2DRenderNode renderNode, Node targetNode)
        {
            this.mindmap = mindmap;
            this.layout = mindmap.Layout;
            this.document = mindmap.Document;
            this.renderer = mindmap.Renderer;
            this.renderNode = renderNode;
            this.targetNode = targetNode;

            initialPosition = renderNode.RenderPosition;
        }

        public void Move(Vector2 translation)
        {
            if (movingNode == null)
            {
                movingNode = renderer.CreateAdorner(renderNode);
            }

            movingNode.MoveBy(translation);

            if (movingNode != null)
            {
                if (movingNode.RenderBounds.Width > 0 && movingNode.RenderBounds.Height > 0)
                {
                    AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, targetNode, movingNode.RenderBounds);

                    if (target != null)
                    {
                        renderer.ShowPreviewElement(target.Position, target.Parent, target.Anchor);
                    }
                    else
                    {
                        renderer.HidePreviewElement();
                    }
                }

                renderer.Invalidate();
            }
        }

        public void Complete()
        {
            try
            {
                if (movingNode != null && (initialPosition - movingNode.RenderPosition).LengthSquared() > 100)
                {
                    AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, targetNode, movingNode.RenderBounds);

                    if (target != null)
                    {
                        targetNode.MoveTransactional(target.Parent, target.Index, target.NodeSide);
                    }
                }
            }
            finally
            {
                Cancel();
            }
        }

        public void Cancel()
        {
            if (movingNode != null)
            {
                renderer.RemoveAdorner(movingNode);
            }

            renderer.HidePreviewElement();
            renderer.Invalidate();
        }
    }
}
