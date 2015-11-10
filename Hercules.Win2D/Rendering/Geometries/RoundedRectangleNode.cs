// ==========================================================================
// RoundedRectangleNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model;

namespace Hercules.Win2D.Rendering.Geometries
{
    public sealed class RoundedRectangleNode : RectangleNodeBase
    {
        public RoundedRectangleNode(NodeBase node, Win2DRenderer renderer) 
            : base(node, renderer, 10)
        {
        }

        protected override Win2DRenderNode CloneInternal()
        {
            return new RoundedRectangleNode(Node, Renderer);
        }
    }
}
