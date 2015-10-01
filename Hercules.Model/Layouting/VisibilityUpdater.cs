// ==========================================================================
// VisibilityUpdater.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Hercules.Model.Layouting
{
    public sealed class VisibilityUpdater<TLayout> : LayoutOperation<TLayout> where TLayout : ILayout
    {
        public VisibilityUpdater(TLayout layout, IRenderScene scene, Document document)
            : base(layout, scene, document)
        {
        }

        public void UpdateVisibility()
        {
            Document document = Document;

            UpdateVisibility(document.Root.IsCollapsed, document.Root.LeftChildren);
            UpdateVisibility(document.Root.IsCollapsed, document.Root.RightChildren);
        }

        private void UpdateVisibility(bool isCollapsed, IEnumerable<Node> children)
        {
            foreach (Node child in children)
            {
                IRenderNode renderNode = Scene.FindRenderNode(child);
                
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
