// ==========================================================================
// NodeEventArgs.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;
using System;

namespace Hercules.Model
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
            Guard.NotNull(node, nameof(node));

            this.node = node;
        }
    }
}
