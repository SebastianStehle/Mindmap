// ==========================================================================
// NodeCollection.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// A collection of nodes.
    /// </summary>
    public sealed class NodeCollection : OrderedCollection<Node>
    {
        #region Fields

        private readonly NodeBase parentNode;
        private readonly Func<NodeSide> nodeSide;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class with the parent node.
        /// </summary>
        /// <param name="parent">The parent node. Cannot be null.</param>
        /// <param name="side">The side function. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="parent"/> is null.
        ///     - or -
        ///     <paramref name="side"/> is side.
        /// </exception>
        public NodeCollection(NodeBase parent, Func<NodeSide> side)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            nodeSide = side;

            parentNode = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the item before it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected override void HandleItemAdding(Node newItem, int index)
        {
            if (newItem == null)
            {
                throw new InvalidOperationException("Node cannot be null.");
            }

            if (newItem.Parent != null)
            {
                throw new InvalidOperationException("Node is already part of another collection.");
            }

            newItem.Parent = parentNode;

            UpdateSide(newItem, nodeSide());

            base.HandleItemAdding(newItem, index);
        }

        /// <summary>
        /// Handles the item after it will be added to the collection
        /// </summary>
        /// <param name="newItem">The new item. Cannot be null.</param>
        /// <param name="index">The new index of the item.</param>
        protected override void HandleItemAdded(Node newItem, int index)
        {
            if (parentNode.Document != null)
            {
                parentNode.Document.TryAdd(newItem);
            }

            base.HandleItemAdded(newItem, index);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="oldItem">The element to remove.</param>
        protected override void HandleItemRemoved(Node oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Parent = null;

                UpdateSide(oldItem, NodeSide.Undefined);

                if (parentNode.Document != null)
                {
                    parentNode.Document.TryRemove(oldItem);
                }
            }

            base.HandleItemRemoved(oldItem);
        }

        private void UpdateSide(Node node, NodeSide side)
        {
            node.Side = nodeSide();

            foreach (Node child in node.Children)
            {
                UpdateSide(child, side);
            }
        }

        #endregion
    }
}
