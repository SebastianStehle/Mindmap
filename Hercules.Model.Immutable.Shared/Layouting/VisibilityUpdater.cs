// ==========================================================================
// VisibilityUpdater.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Hercules.Model.Rendering;

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

            bool rootIsCollapsed = document.Root().IsCollapsed;

            UpdateVisibility(document, rootIsCollapsed, document.LeftMainNodes());
            UpdateVisibility(document, rootIsCollapsed, document.RightMainNodes());
        }

        private void UpdateVisibility(Document document, bool isCollapsed, IEnumerable<Node> children)
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

                UpdateVisibility(document, isCollapsed || child.IsCollapsed, document.Children(child));
            }
        }
    }
}
