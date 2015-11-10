// ==========================================================================
// RectangleNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;

namespace Hercules.Win2D.Rendering.Geometries
{
    public sealed class RectangleNode : RectangleNodeBase
    {
        public RectangleNode(NodeBase node, Win2DRenderer renderer) 
            : base(node, renderer, 0)
        {
        }

        protected override Win2DRenderNode CloneInternal()
        {
            return new RectangleNode(Node, Renderer);
        }
    }
}
