// ==========================================================================
// NodeEventArgs.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using SE.Metro;
using System;

namespace RavenMind.Model
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
