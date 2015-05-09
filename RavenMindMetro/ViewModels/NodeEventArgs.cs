// ==========================================================================
// NodeEventArgs.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using RavenMind.Model;

namespace RavenMind.ViewModels
{
    public sealed class NodeEventArgs : EventArgs
    {
        #region Properties

        public Node Node { get; private set; }

        #endregion

        #region Constructors

        public NodeEventArgs(Node node)
        {
            Node = node;
        }

        #endregion
    }
}
