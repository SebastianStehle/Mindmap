﻿// ==========================================================================
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
        public static NodeBase FindRightOf(this NodeBase node)
        {
            NodeBase result = null;

            var selectedRoot = node as RootNode;

            if (selectedRoot != null)
            {
                if (!selectedRoot.IsCollapsed)
                {
                    result = TrySelectMiddle(selectedRoot.RightChildren);
                }
            }
            else
            {
                var normalNode = node as Node;

                if (normalNode != null && normalNode.NodeSide == NodeSide.Right)
                {
                    if (!normalNode.IsCollapsed)
                    {
                        result = TrySelectMiddle(normalNode.Children);
                    }
                }
                else if (normalNode != null && normalNode.NodeSide == NodeSide.Left)
                {
                    result = normalNode.Parent;
                }
            }

            return result;
        }

        public static NodeBase FindLeftOf(this NodeBase node)
        {
            NodeBase result = null;

            var selectedRoot = node as RootNode;

            if (selectedRoot != null)
            {
                if (!selectedRoot.IsCollapsed)
                {
                    result = TrySelectMiddle(selectedRoot.LeftChildren);
                }
            }
            else
            {
                var normalNode = node as Node;

                if (normalNode != null && normalNode.NodeSide == NodeSide.Left)
                {
                    if (!normalNode.IsCollapsed)
                    {
                        result = TrySelectMiddle(normalNode.Children);
                    }
                }
                else if (normalNode != null && normalNode.NodeSide == NodeSide.Right)
                {
                    result = normalNode.Parent;
                }
            }

            return result;
        }

        public static NodeBase FindTopOf(this NodeBase node)
        {
            NodeBase result = null;

            var normalNode = node as Node;

            if (normalNode != null)
            {
                var parentCollection = normalNode.RetrieveParentCollection();

                var currentIndex = parentCollection.IndexOf(normalNode);

                if (currentIndex > 0)
                {
                    result = parentCollection[currentIndex - 1];
                }
                else
                {
                    var normalParent = node.Parent as Node;

                    if (normalParent != null)
                    {
                        var grandParentCollection = normalParent.RetrieveParentCollection();

                        var currentParentIndex = grandParentCollection.IndexOf(normalParent);

                        for (var i = currentParentIndex - 1; i >= 0; i--)
                        {
                            if ((result = grandParentCollection[i].Children.LastOrDefault()) != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static NodeBase FindBottomOf(this NodeBase node)
        {
            NodeBase result = null;

            var normalNode = node as Node;

            if (normalNode != null)
            {
                var parentCollection = normalNode.RetrieveParentCollection();

                var currentIndex = parentCollection.IndexOf(normalNode);

                if (currentIndex < parentCollection.Count - 1)
                {
                    result = parentCollection[currentIndex + 1];
                }
                else
                {
                    var normalParent = node.Parent as Node;

                    if (normalParent != null)
                    {
                        var grandParentCollection = normalParent.RetrieveParentCollection();

                        var currentParentIndex = grandParentCollection.IndexOf(normalParent);

                        for (var i = currentParentIndex + 1; i < grandParentCollection.Count; i++)
                        {
                            if ((result = grandParentCollection[i].Children.FirstOrDefault()) != null)
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
