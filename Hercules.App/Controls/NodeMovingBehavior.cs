// ==========================================================================
// NodeMovingBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System.Numerics;
using GP.Windows.UI.Interactivity;
using Hercules.Model.Rendering.Win2D;
using Windows.UI.Xaml.Input;

namespace Hercules.App.Controls
{
    public sealed class NodeMovingBehavior : Behavior<Mindmap>
    {
        private NodeMovingOperation movingOperation;

        protected override void OnAttached()
        {
            AssociatedElement.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            AssociatedElement.ManipulationStarted += AssociatedElement_ManipulationStarted;
            AssociatedElement.ManipulationDelta += AssociatedElement_ManipulationDelta;
            AssociatedElement.ManipulationCompleted += AssociatedElement_ManipulationCompleted;
        }

        protected override void OnDetaching()
        {
            AssociatedElement.ManipulationStarted -= AssociatedElement_ManipulationStarted;
            AssociatedElement.ManipulationDelta -= AssociatedElement_ManipulationDelta;
            AssociatedElement.ManipulationCompleted -= AssociatedElement_ManipulationCompleted;
        }

        private void AssociatedElement_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (movingOperation != null)
            {
                movingOperation.Cancel();
                movingOperation = null;
            }

            Vector2 position = AssociatedElement.Renderer.GetMindmapPosition(e.Position.ToVector2());

            foreach (Win2DRenderNode renderNode in AssociatedElement.Renderer.RenderNodes)
            {
                if (renderNode.HitTest(position) && renderNode != AssociatedElement.TextEditingNode)
                {
                    movingOperation = NodeMovingOperation.Start(AssociatedElement, renderNode);
                    break;
                }
            }
        }

        private void AssociatedElement_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (movingOperation != null)
            {
                Vector2 translation = AssociatedElement.Renderer.GetMindmapSize(e.Delta.Translation.ToVector2());

                movingOperation.Move(translation);
            }
        }

        private void AssociatedElement_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (movingOperation != null)
            {
                movingOperation.Complete();
                movingOperation = null;
            }
        }
    }
}
