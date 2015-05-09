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
    internal sealed class NodeData
    {
        #region Fields

        private readonly INodeView nodeView;

        #endregion

        #region Properties

        public Size SizeWithChildren { get; set; }

        public INodeView NodeView
        {
            get { return nodeView; }
        }

        #endregion

        #region Constructors

        public NodeData(INodeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            nodeView = view;

            SizeWithChildren = nodeView.Size;
        }

        #endregion
    }
}
