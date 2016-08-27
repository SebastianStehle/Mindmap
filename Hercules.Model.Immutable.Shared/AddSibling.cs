// ==========================================================================
// AddChild.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    public sealed class AddSibling : NodeAction
    {
        private readonly Guid parentId;

        public AddSibling(Guid nodeId, Guid parentId)
            : base(nodeId)
        {
            this.parentId = parentId;
        }

        public Guid ParentId
        {
            get { return parentId; }
        }
    }
}
