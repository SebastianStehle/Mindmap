// ==========================================================================
// NodeExtensions.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Hercules.Model
{
    public static class NodeExtensions
    {
        public static IList<Node> AllChildren(this Node node)
        {
            Guard.NotNull(node, nameof(node));

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

        public static IReadOnlyList<Node> RetrieveParentCollection(this Node node)
        {
            Guard.NotNull(node, nameof(node));

            IReadOnlyList<Node> result = null;

            RootNode parent = node.Parent as RootNode;

            if (parent != null)
            {
                result = parent.RightChildren.Contains(node) ? parent.RightChildren : parent.LeftChildren;
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
