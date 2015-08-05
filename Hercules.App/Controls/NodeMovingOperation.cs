// ==========================================================================
// NodeMovingOperation.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;
using Hercules.Model.Layouting;
using GP.Windows.UI;
using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Hercules.Model.Rendering.Win2D;
using System.Numerics;

namespace Hercules.App.Controls
{
    public sealed class NodeMovingOperation
    {
        private readonly Vector2 initialPosition;
        private readonly Node nodeMoving;
        private readonly Win2DRenderNode renderNode;
        private readonly Win2DRenderer renderer;
        private readonly Win2DRenderNode clone;

        public static NodeMovingOperation Start(Win2DRenderer renderer, Win2DRenderNode renderNode)
        {
            if (renderNode != null)
            {
                Node movingNode = renderNode.Node as Node;

                if (movingNode != null && movingNode.IsSelected)
                {
                    return new NodeMovingOperation(renderer, renderNode, movingNode);
                }
            }

            return null;
        }

        internal NodeMovingOperation(Win2DRenderer renderer, Win2DRenderNode renderNode, Node nodeMoving)
        {
            this.renderer = renderer;
            this.renderNode = renderNode;
            this.nodeMoving = nodeMoving;
            
            initialPosition = renderNode.Position;

            clone = renderNode.CloneUnlinked();

            renderer.AddCustomNode(clone);
        }

        public void Move(Vector2 translation)
        {
            clone.MoveBy(translation);

            renderer.Invalidate();
            
            /*
            AttachTarget target = mindmap.CalculateAttachTarget(nodeMoving, new Rect(transform.Position(), nodeControl.RenderSize));

            if (target != null)
            {
                mindmap.ShowPreviewElement(target.Position, target.Parent, target.Anchor);
            }
            else
            {
                mindmap.ShowPreviewElement(null, null, AnchorPoint.Center);
            }*/
        }

        public void Complete()
        {
            try
            {
                /*if (MathHelper.LengthSquared(transform.Position(), initialPosition) > 100)
                {
                    AttachTarget target = mindmap.CalculateAttachTarget(nodeMoving, new Rect(transform.Position(), new Size(clone.Width, clone.Height)));

                    if (target != null)
                    {
                        mindmap.Document.MakeTransaction("MoveNode", d =>
                        {
                            d.Apply(new RemoveChildCommand(nodeMoving.Parent, nodeMoving));

                            d.Apply(new InsertChildCommand(target.Parent, target.Index, target.NodeSide, nodeMoving));
                        });
                    }
                }*/
            }
            finally
            {
                renderer.RemoveCustomNode(clone);
            }

            renderer.Invalidate();
        }
    }
}
