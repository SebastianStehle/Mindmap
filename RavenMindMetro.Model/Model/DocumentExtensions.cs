// ==========================================================================
// DocumentExtensions.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Linq;

namespace RavenMind.Model
{
    /// <summary>
    /// Provides extension methodods to deal with documents.
    /// </summary>
    public static class DocumentExtensions
    {
        /// <summary>
        /// Selects the first node at the right side of the current selected node.
        /// </summary>
        /// <param name="document">The target document. Cannot be null.</param>
        /// <returns>The resulting and selected node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <remarks>
        /// When the selected node is a root node or at the right side the first child is chosen, otherwise the parent is selected.
        /// </remarks>
        public static NodeBase SelectRightOfSelectedNode(this Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                RootNode selectedRoot = document.SelectedNode as RootNode;

                if (selectedRoot != null)
                {
                    TrySelectMiddle(selectedRoot.RightChildren, ref result);
                }
                else
                {
                    Node normalNode = document.SelectedNode as Node;

                    if (normalNode.Side == NodeSide.Right)
                    {
                        TrySelectMiddle(normalNode.Children, ref result);
                    }
                    else if (normalNode.Side == NodeSide.Left)
                    {
                        result = normalNode.Parent;
                    }
                }
            }

            result.IsSelected = true;

            return result;
        }

        /// <summary>
        /// Selects the first node at the left side of the current selected node.
        /// </summary>
        /// <param name="document">The target document. Cannot be null.</param>
        /// <returns>The resulting and selected node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <remarks>
        /// When the selected node is a root node or at the left side the parent is chosen, otherwise the first child is selected.
        /// </remarks>
        public static NodeBase SelectLeftOfSelectedNode(this Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                RootNode selectedRoot = document.SelectedNode as RootNode;

                if (selectedRoot != null)
                {
                    TrySelectMiddle(selectedRoot.LeftChildren, ref result);
                }
                else
                {
                    Node normalNode = document.SelectedNode as Node;

                    if (normalNode.Side == NodeSide.Left)
                    {
                        TrySelectMiddle(normalNode.Children, ref result);
                    }
                    else if (normalNode.Side == NodeSide.Right)
                    {
                        result = normalNode.Parent;
                    }
                }
            }

            result.IsSelected = true;

            return result;
        }

        /// <summary>
        /// Selects the first node at the top side of the current selected node.
        /// </summary>
        /// <param name="document">The target document. Cannot be null.</param>
        /// <returns>The resulting and selected node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <remarks>
        /// When the selected node is not a root node, the first top node with the same x-position is selected.
        /// </remarks>
        public static NodeBase SelectedTopOfSelectedNode(this Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                Node normalNode = document.SelectedNode as Node;

                if (normalNode != null)
                {
                    NodeCollection parentCollection = normalNode.RetrieveParentCollection();

                    int currentIndex = parentCollection.IndexOf(normalNode);

                    if (currentIndex > 0)
                    {
                        result = parentCollection[currentIndex - 1];
                    }
                    else
                    {
                        Node normalParent = document.SelectedNode.Parent as Node;

                        if (normalParent != null)
                        {
                            NodeCollection grandParentCollection = normalParent.RetrieveParentCollection();

                            int currentParentIndex = grandParentCollection.IndexOf(normalParent);

                            for (int i = currentParentIndex - 1; i >= 0; i--)
                            {
                                if (TrySelectLast(grandParentCollection[i].Children, ref result))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            result.IsSelected = true;

            return result;
        }

        /// <summary>
        /// Selects the first node at the top side of the current selected node.
        /// </summary>
        /// <param name="document">The target document. Cannot be null.</param>
        /// <returns>The resulting and selected node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null.</exception>
        /// <remarks>
        /// When the selected node is not a root node, the first top node with the same x-position is selected.
        /// </remarks>
        public static NodeBase SelectedBottomOfSelectedNode(this Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                Node normalNode = document.SelectedNode as Node;

                if (normalNode != null)
                {
                    NodeCollection parentCollection = normalNode.RetrieveParentCollection();

                    int currentIndex = parentCollection.IndexOf(normalNode);

                    if (currentIndex < parentCollection.Count - 1)
                    {
                        result = parentCollection[currentIndex + 1];
                    }
                    else
                    {
                        Node normalParent = document.SelectedNode.Parent as Node;

                        if (normalParent != null)
                        {
                            NodeCollection grandParentCollection = normalParent.RetrieveParentCollection();

                            int currentParentIndex = grandParentCollection.IndexOf(normalParent);

                            for (int i = currentParentIndex + 1; i < grandParentCollection.Count; i++)
                            {
                                if (TrySelectFirst(grandParentCollection[i].Children, ref result))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            result.IsSelected = true;

            return result;
        }

        private static bool TrySelectLast(NodeCollection nodes, ref NodeBase node)
        {
            bool result = true;

            if (nodes.Count > 0)
            {
                node = nodes.Last();
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static bool TrySelectMiddle(NodeCollection nodes, ref NodeBase node)
        {
            bool result = true;

            if (nodes.Count > 0)
            {
                node = nodes[nodes.Count / 2];
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static bool TrySelectFirst(NodeCollection nodes, ref NodeBase node)
        {
            bool result = true;

            if (nodes.Count > 0)
            {
                node = nodes.First();
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static bool TrySelectRoot(this Document document, ref NodeBase node)
        {
            bool result = true;

            if (document.SelectedNode == null || document.Nodes.Count == 0)
            {
                node = document.Root;
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
