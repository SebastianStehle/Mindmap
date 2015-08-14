// ==========================================================================
// VisibilityUpdater.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System.Collections.Generic;
using GP.Windows;

namespace Hercules.Model.Layouting
{
    public sealed class VisibilityUpdater
    {
        private readonly Document document;
        private readonly IRenderer renderer;

        public VisibilityUpdater(Document document, IRenderer renderer)
        {
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(renderer, nameof(renderer));

            this.document = document;
            this.renderer = renderer;
        }

        public void UpdateVisibility()
        {
            UpdateVisibility(document.Root.IsCollapsed, document.Root.LeftChildren);
            UpdateVisibility(document.Root.IsCollapsed, document.Root.RightChildren);
        }

        private void UpdateVisibility(bool isCollapsed, IEnumerable<Node> children)
        {
            foreach (Node child in children)
            {
                IRenderNode renderNode = renderer.FindRenderNode(child);
                
                if (!isCollapsed)
                {
                    renderNode.Show();
                }
                else
                {
                    renderNode.Hide();
                }

                UpdateVisibility(isCollapsed || child.IsCollapsed, child.Children);
            }
        }
    }
}
