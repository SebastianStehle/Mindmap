// ==========================================================================
// NodeSideExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model
{
    public static class NodeSideExtensions
    {
        public static NodeSide OppositeSide(this NodeSide side)
        {
            if (side == NodeSide.Right)
            {
                return NodeSide.Left;
            }

            return side == NodeSide.Left ? NodeSide.Right : NodeSide.Auto;
        }
    }
}