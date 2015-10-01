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
using Hercules.Model.Rendering.Win2D;
using Hercules.Model.Utils;

namespace Hercules.App.Controls
{
    public sealed class NodeMovingOperation
    {
        private readonly Node nodeMoving;
        private readonly Win2DRenderer renderer;
        private readonly Win2DRenderNode clone;
        private readonly Document document;
        private readonly ILayout layout;
        private readonly Vector2 initialPosition;

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

        internal NodeMovingOperation(Mindmap mindmap, Win2DRenderNode renderNode, Node nodeMoving)
        {
            this.layout = mindmap.Layout;
            this.document = mindmap.Document;
            this.renderer = mindmap.Renderer;
            this.nodeMoving = nodeMoving;
            
            clone = renderNode.CloneUnlinked();

            renderer.AddCustomNode(clone);

            initialPosition = renderNode.RenderPosition;
        }

        public void Move(Vector2 translation)
        {
            clone.MoveBy(translation);

            renderer.Invalidate();

            if (clone.Bounds.Width > 0 && clone.Bounds.Height > 0)
            {
                AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, nodeMoving, clone.Bounds);

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
                if ((initialPosition - clone.Position).LengthSquared() > 100)
                {
                    AttachTarget target = layout.CalculateAttachTarget(document, renderer.Scene, nodeMoving, clone.Bounds);

                    if (target != null)
                    {
                        string tansactionName = ResourceManager.GetString("TransactionName_MoveNode");

                        document.MakeTransaction(tansactionName, commands =>
                        {
                            commands.Apply(new RemoveChildCommand(nodeMoving.Parent, nodeMoving));

                            commands.Apply(new InsertChildCommand(target.Parent, target.Index, target.NodeSide, nodeMoving));
                        });
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
            renderer.HidePreviewElement();
            renderer.RemoveCustomNode(clone);
            renderer.Invalidate();
        }
    }
}
