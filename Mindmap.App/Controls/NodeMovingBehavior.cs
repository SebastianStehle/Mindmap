// ==========================================================================
// NodeMovingBehavior.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro.UI.Interactivity;
using Windows.UI.Xaml.Input;

namespace Mindmap.Controls
{
    public sealed class NodeMovingBehavior : Behavior<Mindmap>
    {
        private NodeMovingOperation movingOperation;

        protected override void OnAttached()
        {
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
            movingOperation = NodeMovingOperation.Start(AssociatedElement, e.OriginalSource as NodeControl);
        }

        private void AssociatedElement_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (movingOperation != null)
            {
                movingOperation.Move(e.Delta.Translation);
            }
        }

        private void AssociatedElement_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (movingOperation != null)
            {
                movingOperation.Complete();
            }
        }
    }
}
