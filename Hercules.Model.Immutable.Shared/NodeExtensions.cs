// ==========================================================================
// NodeExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;

// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.Model
{
    public static class NodeExtensions
    {
        public static IList<Node> AllChildren(this NodeBase nodeBase, Document document)
        {
            List<Node> allChildren = new List<Node>();

            RootNode root = nodeBase as RootNode;

            if (root != null)
            {
                foreach (Node child in document.LeftMainNodes())
                {
                    allChildren.Add(child);

                    AddChildren(allChildren, child, document);
                }

                foreach (Node child in document.RightMainNodes())
                {
                    allChildren.Add(child);

                    AddChildren(allChildren, child, document);
                }
            }

            Node node = nodeBase as Node;

            if (node != null)
            {
                AddChildren(allChildren, node, document);
            }

            return allChildren;
        }

        private static void AddChildren(ICollection<Node> allChildren, Node node, Document document)
        {
            foreach (Node child in document.Children(node))
            {
                allChildren.Add(child);

                AddChildren(allChildren, child, document);
            }
        }

        public static IReadOnlyList<Node> RetrieveParentCollection(this Node node, Document document)
        {
            IReadOnlyList<Node> result = null;

            NodeBase parent = document.Parent(node);

            if (parent is RootNode)
            {
                result = document.RightMainNodes().Contains(node) ? document.RightMainNodes() : document.LeftMainNodes();
            }

            return result ?? document.Children((Node)parent);
        }
    }
}
