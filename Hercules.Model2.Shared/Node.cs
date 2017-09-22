// ==========================================================================
// Node.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model2
{
    public sealed class Node : Cloneable<Node>
    {
        private readonly Guid id;
        private readonly Document document;
        private int index;
        private string text = string.Empty;
        private bool isCollapsed;
        private NodeSide side;
        private NodeShape shape = NodeShape.Ellipse;
        private Guid? parentId;

        public Document Document
        {
            get { return document; }
        }

        public Guid Id
        {
            get { return id; }
        }

        public Guid? ParentId
        {
            get { return parentId; }
        }

        public bool IsRoot
        {
            get { return parentId.HasValue; }
        }

        public bool IsCollapsed
        {
            get { return isCollapsed; }
        }

        public int Index
        {
            get { return index; }
        }

        public string Text
        {
            get { return text; }
        }

        public NodeSide Side
        {
            get { return side; }
        }

        public NodeShape Shape
        {
            get { return shape; }
        }

        public object LayoutData { get; set; }
        public object RenderData { get; set; }

        public Node(Guid id, Document document)
        {
            this.id = id;

            this.document = document;
        }

        public Node WithPosition(Guid newParentId, int newIndex, NodeSide newSide)
        {
            if (newParentId == parentId && newIndex == index && newSide == side)
            {
                return this;
            }

            return Clone(c => { c.parentId = newParentId; c.index = newIndex; side = c.side; });
        }

        public Node WithShape(NodeShape value)
        {
            if (value == shape || !Enum.IsDefined(typeof(NodeShape), value))
            {
                return this;
            }

            return Clone(c => c.shape = value);
        }

        public Node WithCollapsed(bool value)
        {
            if (value == isCollapsed)
            {
                return this;
            }

            return Clone(c => c.isCollapsed = value);
        }

        public Node WithText(string value)
        {
            if (value == text || value == null)
            {
                return this;
            }

            return Clone(c => c.text = value);
        }
    }
}
