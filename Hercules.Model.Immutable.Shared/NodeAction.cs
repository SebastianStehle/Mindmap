// ==========================================================================
// NodeAction.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public abstract class NodeAction : IAction
    {
        private readonly Guid nodeId;

        public Guid NodeId
        {
            get { return nodeId; }
        }

        protected NodeAction(Guid nodeId)
        {
            this.nodeId = nodeId;
        }
    }
}
