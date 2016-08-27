// ==========================================================================
// NodeNavigationExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using GP.Utils;

// ReSharper disable InvertIf

namespace Hercules.Model
{
    public static class NodeNavigationExtensions
    {
        public static NodeBase FindRightOf(this NodeBase node, Document document)
        {
            NodeBase result = null;

            RootNode selectedRoot = node as RootNode;

            if (selectedRoot != null)
            {
                if (!selectedRoot.IsCollapsed)
                {
                    result = TrySelectMiddle(document.RightMainNodes());
                }
            }
            else
            {
                Node normalNode = (Node)node;

                NodeSide side = normalNode.Side(document);

                if (!normalNode.IsCollapsed && side == NodeSide.Right)
                {
                    result = TrySelectMiddle(document.Children(normalNode));
                }
                else if (side == NodeSide.Left)
                {
                    result = document.Parent(node);
                }
            }

            return result;
        }

        public static NodeBase FindLeftOf(this NodeBase node, Document document)
        {
            NodeBase result = null;

            RootNode selectedRoot = node as RootNode;

            if (selectedRoot != null)
            {
                if (!selectedRoot.IsCollapsed)
                {
                    result = TrySelectMiddle(document.LeftMainNodes());
                }
            }
            else
            {
                Node normalNode = (Node)node;

                NodeSide side = normalNode.Side(document);

                if (!normalNode.IsCollapsed && side == NodeSide.Left)
                {
                    result = TrySelectMiddle(document.Children(normalNode));
                }
                else if (side == NodeSide.Right)
                {
                    result = document.Parent(node);
                }
            }

            return result;
        }

        public static NodeBase FindTopOf(this NodeBase node, Document document)
        {
            NodeBase result = null;

            Node normalNode = node as Node;

            if (normalNode != null)
            {
                IReadOnlyList<Node> parentCollection = normalNode.RetrieveParentCollection(document);

                int currentIndex = parentCollection.IndexOf(normalNode);

                if (currentIndex > 0)
                {
                    result = parentCollection[currentIndex - 1];
                }
                else
                {
                    Node normalParent = document.Parent(node) as Node;

                    if (normalParent != null)
                    {
                        IReadOnlyList<Node> grandParentCollection = normalParent.RetrieveParentCollection(document);

                        int currentParentIndex = grandParentCollection.IndexOf(normalParent);

                        for (int i = currentParentIndex - 1; i >= 0; i--)
                        {
                            if ((result = document.Children(grandParentCollection[i]).LastOrDefault()) != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static NodeBase FindBottomOf(this NodeBase node, Document document)
        {
            NodeBase result = null;

            Node normalNode = node as Node;

            if (normalNode != null)
            {
                IReadOnlyList<Node> parentCollection = normalNode.RetrieveParentCollection(document);

                int currentIndex = parentCollection.IndexOf(normalNode);

                if (currentIndex < parentCollection.Count - 1)
                {
                    result = parentCollection[currentIndex + 1];
                }
                else
                {
                    Node normalParent = document.Parent(node) as Node;

                    if (normalParent != null)
                    {
                        IReadOnlyList<Node> grandParentCollection = normalParent.RetrieveParentCollection(document);

                        int currentParentIndex = grandParentCollection.IndexOf(normalParent);

                        for (int i = currentParentIndex + 1; i < grandParentCollection.Count; i++)
                        {
                            if ((result = document.Children(grandParentCollection[i]).FirstOrDefault()) != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static NodeBase TrySelectMiddle(this IReadOnlyList<Node> nodes)
        {
            NodeBase result = null;

            if (nodes.Count > 0)
            {
                result = nodes[nodes.Count / 2];
            }

            return result;
        }
    }
}
