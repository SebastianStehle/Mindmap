// ==========================================================================
// NodeExtensions.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace RavenMind.Model
{
    /// <summary>
    /// Provides extension methods for nodes.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Calculates the flat list of all children of the current node.
        /// </summary>
        /// <param name="node">The current node. Cannot be null.</param>
        /// <returns>
        /// The flat list of all children of the current node.
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        /// </returns>
        public static IList<Node> AllChildren(this Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            List<Node> allChildren = new List<Node>();

            AddChildren(allChildren, node);

            return allChildren;
        }

        private static void AddChildren(List<Node> allChildren, Node node)
        {
            foreach (Node child in node.Children)
            {
                allChildren.Add(child);

                AddChildren(allChildren, child);
            }
        }

        /// <summary>
        /// Gets the parent collection of the current node.
        /// </summary>
        /// <param name="node">The current node. Cannot be null.</param>
        /// <returns>The parent collection of the current node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        public static NodeCollection RetrieveParentCollection(this Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            NodeCollection result = null;

            RootNode parent = node.Parent as RootNode;

            if (parent != null)
            {
                if (parent.RightChildren.Contains(node))
                {
                    result = parent.RightChildren;
                }
                else
                {
                    result = parent.LeftChildren;
                }
            }

            if (result == null)
            {
                Node parentNormal = node.Parent as Node;

                if (parentNormal != null)
                {
                    result = parentNormal.Children;
                }
            }

            return result;
        }
    }
}
