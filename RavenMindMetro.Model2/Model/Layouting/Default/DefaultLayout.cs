// ==========================================================================
// Layout.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Composition;
using Windows.Foundation;

namespace RavenMind.Model.Layouting.Default
{
    [Export]
    [Export(typeof(ILayout))]
    public sealed class DefaultLayout : ILayout
    {
        public double HorizontalMargin { get; set; }

        public double ElementMargin { get; set; }

        public void UpdateLayout(Document document, IRenderer renderer, Size availableSize)
        {
            LayoutProcess process = new LayoutProcess(document, this, renderer, availableSize);

            process.UpdateLayout();
        }

        public AttachTarget CalculateAttachTarget(Document document, IRenderer renderer, Node movingNode, Rect movementBounds, Point mindmapCenter)
        {
            PreviewCalculationProcess process = new PreviewCalculationProcess(document, this, renderer, movingNode, movementBounds, mindmapCenter);

            return process.CalculateAttachTarget();
        }
    }
}
