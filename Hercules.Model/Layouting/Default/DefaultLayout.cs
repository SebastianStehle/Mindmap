// ==========================================================================
// Layout.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;
using Windows.Foundation;

namespace Hercules.Model.Layouting.Default
{
    public sealed class DefaultLayout : ILayout
    {
        public double HorizontalMargin { get; set; }

        public double ElementMargin { get; set; }

        public void UpdateLayout(Document document, IRenderer renderer)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));

            LayoutProcess process = new LayoutProcess(document, this, renderer);

            process.UpdateLayout();
        }

        public void UpdateVisibility(Document document, IRenderer renderer)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));

            VisibilityUpdater updater = new VisibilityUpdater(document, renderer);

            updater.UpdateVisibility();
        }

        public AttachTarget CalculateAttachTarget(Document document, IRenderer renderer, Node movingNode, Rect movementBounds)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));
            Guard.NotNull(movingNode, nameof(movingNode));

            PreviewCalculationProcess process = new PreviewCalculationProcess(document, this, renderer, movingNode, movementBounds);

            return process.CalculateAttachTarget();
        }
    }
}
