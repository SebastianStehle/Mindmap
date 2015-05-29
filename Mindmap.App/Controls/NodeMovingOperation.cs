// ==========================================================================
// NodeMovingOperation.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using MindmapApp.Model;
using MindmapApp.Model.Layouting;
using GreenParrot.Windows.UI;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MindmapApp.Controls
{
    public sealed class NodeMovingOperation
    {
        private readonly Point initialPosition;
        private readonly TranslateTransform transform;
        private readonly Mindmap mindmap;
        private readonly Node nodeMoving;
        private readonly NodeControl nodeControl;
        private bool isCompleted;
        private Image clone;

        public static NodeMovingOperation Start(Mindmap mindmap, NodeControl nodeControl)
        {
            if (nodeControl != null)
            {
                if (nodeControl != null)
                {
                    Node movingNode = nodeControl.AssociatedNode as Node;

                    if (movingNode != null && movingNode.IsSelected && !nodeControl.IsTextEditing)
                    {
                        return new NodeMovingOperation(mindmap, nodeControl, movingNode);
                    }
                }
            }

            return null;
        }

        public NodeMovingOperation(Mindmap mindmap)
        {
            this.mindmap = mindmap;
        }

        public NodeMovingOperation(Mindmap mindmap, NodeControl nodeControl, Node nodeMoving)
        {
            this.mindmap = mindmap;
            this.nodeMoving = nodeMoving;
            this.nodeControl = nodeControl;

            Rect bounds = mindmap.GetBounds(nodeControl.AssociatedNode);

            initialPosition = bounds.Position();

            transform = new TranslateTransform
            {
                X = initialPosition.X,
                Y = initialPosition.Y
            };

            RenderAndSet();
        }

        private void RenderAndSet()
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();

            bitmap.RenderAsync(nodeControl).AsTask().ContinueWith(async t =>
            {
                await mindmap.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if (!isCompleted)
                    {
                        clone = new Image { Width = bitmap.PixelWidth, Height = bitmap.PixelHeight, Source = bitmap, RenderTransform = transform, Stretch = Stretch.Fill };

                        mindmap.AddAdorner(clone);
                    }
                });
            });
        }

        public void Move(Point translation)
        {
            double dx = translation.X / mindmap.ScrollViewer.ZoomFactor;
            double dy = translation.Y / mindmap.ScrollViewer.ZoomFactor;

            transform.X += dx;
            transform.Y += dy;

            AttachTarget target = mindmap.CalculateAttachTarget(nodeMoving, new Rect(transform.Position(), nodeControl.RenderSize));

            if (target != null)
            {
                mindmap.ShowPreviewElement(target.Position, target.Parent, target.Anchor);
            }
            else
            {
                mindmap.ShowPreviewElement(null, null, AnchorPoint.Center);
            }
        }

        public void Complete()
        {
            try
            {
                if (MathHelper.LengthSquared(transform.Position(), initialPosition) > 100)
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
                }
            }
            finally
            {
                mindmap.ShowPreviewElement(null, null, AnchorPoint.Center);
                mindmap.ClearAdorners();

                isCompleted = true;

                clone = null;
            }
        }
    }
}
