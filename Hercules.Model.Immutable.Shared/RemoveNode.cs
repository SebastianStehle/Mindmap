// ==========================================================================
// RemoveNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public sealed class RemoveNode : NodeAction
    {
        public RemoveNode(Guid nodeId)
            : base(nodeId)
        {
        }
    }
}
