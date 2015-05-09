// ==========================================================================
// Layout.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using Windows.Foundation;

namespace RavenMind.Model
{
    /// <summary>
    /// The default layout system for the mindmaps.
    /// </summary>
    [Export]
    [Export(typeof(ILayout))]
    public sealed class Layout : ILayout
    {
        #region Fields

        private readonly Dictionary<Node, Rect> nodeBounds = new Dictionary<Node, Rect>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the horizontal margin.
        /// </summary>
        /// <value>
        /// The horizontal margin.
        /// </value>
        public double HorizontalMargin { get; set; }

        /// <summary>
        /// Gets or sets the margin around single elements.
        /// </summary>
        /// <value>
        /// The margin around single elements.
        /// </value>
        public double ElementMargin { get; set; }

        /// <summary>
        /// Enables or disables animations.
        /// </summary>
        /// <value>
        /// A value indicating if animating the changes.
        /// </value>
        public bool IsAnimating { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the bounds including the children of the specified node.
        /// </summary>
        /// <param name="node">The node to get the bounds for. Cannot be null.</param>
        /// <returns>
        /// The bounds of the node.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        public Rect? GetBounds(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            Rect temp;

            if (nodeBounds.TryGetValue(node, out temp))
            {
                return temp;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the layout of the document.
        /// </summary>
        /// <param name="document">The document to update the layout for. Cannot be null.</param>
        /// <param name="views">A dictionary to get access to the views. Cannot be null.</param>
        /// <param name="availableSize">The available size.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="document"/> is null.
        ///     - or -
        ///     <paramref name="views"/> is null.
        /// </exception>
        public void UpdateLayout(Document document, Func<NodeBase, INodeView> views, Size availableSize)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (document == null)
            {
                throw new ArgumentNullException("views");
            }

            nodeBounds.Clear();

            foreach (NodeBase node in document.Nodes)
            {
                INodeView element = views(node);

                node.Tag = new LayoutData(element);
            }

            ArrangeRoot(document.Root, availableSize);
        }

        private void ArrangeRoot(RootNode root, Size availableSize)
        {
            LayoutData data = (LayoutData)root.Tag;

            Size nodeSizeLeft  = UpdateSizeWithChildren(data.SizeWithChildren, root.LeftChildren);
            Size nodeSizeRight = UpdateSizeWithChildren(data.SizeWithChildren, root.RightChildren);

            double x = 0.5 * (availableSize.Width  - data.NodeView.Size.Width);
            double y = 0.5 * (availableSize.Height - data.NodeView.Size.Height);

            Rect rect = new Rect(new Point(x, y), data.NodeView.Size);

            data.NodeView.SetPosition(new Point { X = rect.X, Y = rect.Y }, IsAnimating);

            ArrangeNodes(root.LeftChildren,  rect, nodeSizeLeft, true);
            ArrangeNodes(root.RightChildren, rect, nodeSizeRight, false);
        }

        private Size MeasureNode(Node node)
        {
            LayoutData data = (LayoutData)node.Tag;

            data.SizeWithChildren = UpdateSizeWithChildren(data.SizeWithChildren, node.Children);

            return data.SizeWithChildren;
        }

        private Size UpdateSizeWithChildren(Size sizeWithChildren, ReadOnlyCollection<Node> children)
        {
            if (children.Count > 0)
            {
                double childrensWidth = 0;
                double childrensHeight = 0;

                foreach (Node child in children)
                {
                    Size size = MeasureNode(child);

                    childrensWidth += size.Height;
                    childrensHeight = Math.Max(size.Width, childrensHeight);
                }

                sizeWithChildren.Width += HorizontalMargin;
                sizeWithChildren.Width += childrensHeight;
                sizeWithChildren.Height = childrensWidth;
            }

            sizeWithChildren.Height += 2 * ElementMargin;

            return sizeWithChildren;
        }

        private void ArrangeNodes(ReadOnlyCollection<Node> children, Rect parentRect, Size parentSizeWithChildren, bool isLeft)
        {
            double x = 0, y = parentRect.Y + (0.5 * parentRect.Height) - (0.5 * parentSizeWithChildren.Height) + ElementMargin;

            if (isLeft)
            {
                x = parentRect.Left - HorizontalMargin;
            }
            else
            {
                x = parentRect.Right + HorizontalMargin;
            }

            foreach (Node child in children)
            {
                LayoutData data = (LayoutData)child.Tag;

                Point childPosition = new Point(x, y + (0.5 * (data.SizeWithChildren.Height - data.NodeView.Size.Height)));

                if (isLeft)
                {
                    nodeBounds[child] = new Rect(new Point(x - data.SizeWithChildren.Width, y), data.SizeWithChildren);

                    childPosition.X -= data.NodeView.Size.Width;
                }
                else
                {
                    nodeBounds[child] = new Rect(new Point(x, y), data.SizeWithChildren);
                }

                data.NodeView.SetPosition(childPosition, IsAnimating);

                ArrangeNodes(child.Children, new Rect(childPosition, data.NodeView.Size), data.SizeWithChildren, isLeft);

                y += data.SizeWithChildren.Height;
            }
        }

        #endregion
    }
}
