// ==========================================================================
// NodeData.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.Foundation;

namespace RavenMind.Model
{
    internal sealed class LayoutData
    {
        private readonly INodeView nodeView;

        public Size SizeWithChildren { get; set; }

        public INodeView NodeView
        {
            get
            {
                return nodeView;
            }
        }

        public LayoutData(INodeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            nodeView = view;

            SizeWithChildren = nodeView.Size;
        }
    }
}
