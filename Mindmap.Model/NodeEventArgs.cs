// ==========================================================================
// NodeEventArgs.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GreenParrot.Windows;
using System;

namespace Mindmap.Model
{
    public sealed class NodeEventArgs : EventArgs
    {
        private readonly NodeBase node;

        public NodeBase Node
        {
            get
            {
                return node;
            }
        }

        public NodeEventArgs(NodeBase node)
        {
            Guard.NotNull(node, "node");

            this.node = node;
        }
    }
}
