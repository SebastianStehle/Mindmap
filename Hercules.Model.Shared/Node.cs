// ==========================================================================
// Node.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hercules.Model
{
    public class Node : NodeBase
    {
        private readonly List<Node> children = new List<Node>();
        private NodeShape? shape;

        public IReadOnlyList<Node> Children
        {
            get { return children; }
        }

        public override bool HasChildren
        {
            get { return children.Count > 0; }
        }

        public NodeShape? Shape
        {
            get
            {
                return shape;
            }
            protected set
            {
                if (!Equals(shape, value))
                {
                    shape = value;
                    OnPropertyChanged(nameof(Shape));
                }
            }
        }

        public Node(Guid id)
            : base(id)
        {
        }

        internal void ChangeShape(NodeShape? newShape)
        {
            Shape = newShape;
        }

        public override void Insert(Node child, int? index, NodeSide side)
        {
            Add(children, child, index, NodeSide);

            OnPropertyChanged("HasChildren");
        }

        public override bool Remove(Node child, out int oldIndex)
        {
            bool isRemoved = Remove(children, child, out oldIndex);

            if (isRemoved)
            {
                OnPropertyChanged("HasChildren");
            }

            return isRemoved;
        }

        public override bool HasChild(Node child)
        {
            return child != null && (children.Contains(child) || children.Any(n => n.HasChild(child)));
        }
    }
}
