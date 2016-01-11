// ==========================================================================
// DocumentExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public static class DocumentExtensions
    {
        public static void RemoveSelectedNodeTransactional(this Document document)
        {
            document?.SelectedNode.RemoveTransactional();
        }

        public static void AddChildToSelectedNodeTransactional(this Document document)
        {
            document?.SelectedNode.AddChildTransactional();
        }

        public static void AddSibilingToSelectedNodeTransactional(this Document document)
        {
            document?.SelectedNode.AddSibilingTransactional();
        }

        public static NodeBase SelectRightOfSelectedNode(this Document document)
        {
            Guard.NotNull(document, nameof(document));

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

                    if (normalNode != null && normalNode.NodeSide == NodeSide.Right)
                    {
                        TrySelectMiddle(normalNode.Children, ref result);
                    }
                    else if (normalNode != null && normalNode.NodeSide == NodeSide.Left)
                    {
                        result = normalNode.Parent;
                    }
                }
            }

            result.Select();

            return result;
        }

        public static NodeBase SelectLeftOfSelectedNode(this Document document)
        {
            Guard.NotNull(document, nameof(document));

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

                    if (normalNode != null && normalNode.NodeSide == NodeSide.Left)
                    {
                        TrySelectMiddle(normalNode.Children, ref result);
                    }
                    else if (normalNode != null && normalNode.NodeSide == NodeSide.Right)
                    {
                        result = normalNode.Parent;
                    }
                }
            }

            result.Select();

            return result;
        }

        public static NodeBase SelectTopOfSelectedNode(this Document document)
        {
            Guard.NotNull(document, nameof(document));

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                Node normalNode = document.SelectedNode as Node;

                if (normalNode != null)
                {
                    IReadOnlyList<Node> parentCollection = normalNode.RetrieveParentCollection();

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
                            IReadOnlyList<Node> grandParentCollection = normalParent.RetrieveParentCollection();

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

            result.Select();

            return result;
        }

        public static NodeBase SelectBottomOfSelectedNode(this Document document)
        {
            Guard.NotNull(document, nameof(document));

            NodeBase result = document.SelectedNode;

            if (!document.TrySelectRoot(ref result))
            {
                Node normalNode = document.SelectedNode as Node;

                if (normalNode != null)
                {
                    IReadOnlyList<Node> parentCollection = normalNode.RetrieveParentCollection();

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
                            IReadOnlyList<Node> grandParentCollection = normalParent.RetrieveParentCollection();

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

            result.Select();

            return result;
        }

        public static bool TrySelectLast(IReadOnlyList<Node> nodes, ref NodeBase node)
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

        public static bool TrySelectMiddle(IReadOnlyList<Node> nodes, ref NodeBase node)
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

        public static bool TrySelectFirst(IReadOnlyList<Node> nodes, ref NodeBase node)
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

        public static bool TrySelectRoot(this Document document, ref NodeBase node)
        {
            bool result = true;

            if (document.SelectedNode == null || !document.Nodes.Any())
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
