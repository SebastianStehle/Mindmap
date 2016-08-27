// ==========================================================================
// SelectNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public sealed class SelectNode
    {
        private readonly Guid? nodeId;

        public Guid? NodeId
        {
            get { return nodeId; }
        }

        public SelectNode(Guid? nodeId)
        {
            this.nodeId = nodeId;
        }
    }
}
