// ==========================================================================
// Layout.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.Foundation;

namespace Mindmap.Model.Layouting.Default
{
    public sealed class DefaultLayout : ILayout
    {
        public double HorizontalMargin { get; set; }

        public double ElementMargin { get; set; }

        public void UpdateLayout(Document document, IRenderer renderer)
        {
            LayoutProcess process = new LayoutProcess(document, this, renderer);

            process.UpdateLayout();
        }

        public AttachTarget CalculateAttachTarget(Document document, IRenderer renderer, Node movingNode, Rect movementBounds)
        {
            PreviewCalculationProcess process = new PreviewCalculationProcess(document, this, renderer, movingNode, movementBounds);

            return process.CalculateAttachTarget();
        }
    }
}
