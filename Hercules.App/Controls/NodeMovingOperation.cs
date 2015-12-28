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
using Hercules.Win2D.Rendering;
// ReSharper disable ArrangeThisQualifier

namespace Hercules.App.Controls
{
    public sealed class NodeMovingOperation
    {
        private readonly Node targetNode;
        private readonly Win2DRenderer renderer;
        private readonly Win2DRenderNode renderNode;
        private readonly Document document;
        private readonly ILayout layout;
        private readonly Vector2 initialPosition;
        private readonly Mindmap mindmap;
        private Win2DRenderNode movingNode;

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
                movingNode = renderer.AddCustomNode(renderNode.CloneUnlinked());
            }

            movingNode.MoveBy(translation);

            renderer.Invalidate();

            if (movingNode.Bounds.Width > 0 && movingNode.Bounds.Height > 0)
            {
                AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, targetNode, movingNode.Bounds);

                if (target != null)
                {
                    renderer.ShowPreviewElement(target.Position, target.Parent, target.Anchor);
                }
                else
                {
                    renderer.HidePreviewElement();
                }
            }
        }

        public void Complete()
        {
            try
            {
                if (movingNode != null && (initialPosition - movingNode.Position).LengthSquared() > 100)
                {
                    AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, targetNode, movingNode.Bounds);

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
                renderer.RemoveCustomNode(movingNode);
            }

            renderer.HidePreviewElement();
            renderer.Invalidate();
        }
    }
}
