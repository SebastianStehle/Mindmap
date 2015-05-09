// ==========================================================================
// RootNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    /// <summary>
    /// Special class for the root node.
    /// </summary>
    public sealed class RootNode : NodeBase
    {
        #region Fields

        private readonly NodeCollection leftChildren;
        private readonly NodeCollection rightChildren;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the children at the right side of the root node.
        /// </summary>
        /// <value>
        /// The children at the right side.
        /// </value>
        public NodeCollection RightChildren
        {
            get { return rightChildren; }
        }

        /// <summary>
        /// Gets the children at the left side of the root node.
        /// </summary>
        /// <value>
        /// The children at the left side.
        /// </value>
        public NodeCollection LeftChildren
        {
            get { return leftChildren; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RootNode"/> class.
        /// </summary>
        public RootNode()
        {
            leftChildren = new NodeCollection(this, () => NodeSide.Left);
            rightChildren = new NodeCollection(this, () => NodeSide.Right);
        }

        #endregion
    }
}
