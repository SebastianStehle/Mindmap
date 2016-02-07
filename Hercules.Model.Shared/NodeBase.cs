// ==========================================================================
// NodeBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Hercules.Model
{
    public abstract class NodeBase : DocumentObjectWithId
    {
        private Document document;
        private NodeBase parent;
        private NodeSide nodeSide;
        private IconPosition iconPosition;
        private IconSize iconSize;
        private INodeIcon icon;
        private INodeColor color = ThemeColor.Default;
        private string text;
        private bool isShowingHull;
        private bool isCollapsed;
        private bool isSelected;
        private bool isChecked;
        private bool isCheckable = true;

        public object LayoutData { get; set; }

        public object RenderData { get; set; }

        public abstract bool HasChildren { get; }

        public Document Document
        {
            get { return document; }
        }

        public NodeBase Parent
        {
            get { return parent; }
        }

        public string Text
        {
            get
            {
                return text;
            }
            protected set
            {
                if (!Equals(text, value))
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public INodeColor Color
        {
            get
            {
                return color;
            }
            protected set
            {
                if (!Equals(color, value))
                {
                    color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public INodeIcon Icon
        {
            get
            {
                return icon;
            }
            protected set
            {
                if (!Equals(icon, value))
                {
                    icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        public IconPosition IconPosition
        {
            get
            {
                return iconPosition;
            }
            protected set
            {
                if (!Equals(iconPosition, value))
                {
                    iconPosition = value;
                    OnPropertyChanged(nameof(IconPosition));
                }
            }
        }

        public IconSize IconSize
        {
            get
            {
                return iconSize;
            }
            protected set
            {
                if (!Equals(iconSize, value))
                {
                    iconSize = value;
                    OnPropertyChanged(nameof(IconSize));
                }
            }
        }

        public NodeSide NodeSide
        {
            get
            {
                return nodeSide;
            }
            protected set
            {
                if (!Equals(nodeSide, value))
                {
                    nodeSide = value;
                    OnPropertyChanged(nameof(NodeSide));
                }
            }
        }

        public bool IsCollapsed
        {
            get
            {
                return isCollapsed;
            }
            protected set
            {
                if (!Equals(isCollapsed, value))
                {
                    isCollapsed = value;
                    OnPropertyChanged(nameof(IsCollapsed));
                }
            }
        }

        public bool IsCheckable
        {
            get
            {
                return isCheckable;
            }
            protected set
            {
                if (!Equals(isCheckable, value))
                {
                    isCheckable = value;
                    OnPropertyChanged(nameof(IsCheckable));
                }
            }
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            protected set
            {
                if (!Equals(isChecked, value))
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            protected set
            {
                if (!Equals(isSelected, value))
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(isSelected));
                }
            }
        }

        public bool IsShowingHull
        {
            get
            {
                return isShowingHull;
            }
            protected set
            {
                if (!Equals(isShowingHull, value))
                {
                    isShowingHull = value;
                    OnPropertyChanged(nameof(IsShowingHull));
                }
            }
        }

        protected NodeBase(Guid id)
            : base(id)
        {
        }

        internal void ChangeIsSelected(bool newIsSelected)
        {
            IsSelected = newIsSelected;
        }

        internal void ChangeIsCollapsed(bool newIsCollapsed)
        {
            IsCollapsed = newIsCollapsed;
        }

        internal void ChangeIsChecked(bool newIsChecked)
        {
            IsChecked = newIsChecked;
        }

        internal void ChangeIsCheckable(bool newIsCheckable)
        {
            IsCheckable = newIsCheckable;
        }

        internal void ChangeIsShowingHull(bool newIsShowingHull)
        {
            IsShowingHull = newIsShowingHull;
        }

        internal void ChangeColor(INodeColor newColor)
        {
            Color = newColor;
        }

        internal void ChangeIcon(INodeIcon newIcon)
        {
            Icon = newIcon;
        }

        internal void ChangeIconSize(IconSize newIconSize)
        {
            IconSize = newIconSize;
        }

        public void ChangeIconPosition(IconPosition newIconPosition)
        {
            IconPosition = newIconPosition;
        }

        internal void ChangeText(string newText)
        {
            Text = newText;
        }

        internal void LinkToDocument(Document newDocument)
        {
            document = newDocument;
        }

        internal void UnlinkFromDocument()
        {
            document = null;
        }

        internal void LinkToParent(NodeBase newParent)
        {
            parent = newParent;
        }

        internal void UnlinkFromParent()
        {
            parent = null;
        }

        public abstract bool HasChild(Node child);

        public abstract bool Remove(Node child, out int oldIndex);

        public abstract void Insert(Node child, int? index, NodeSide side);

        private static void ChangeSide(Node node, NodeSide side)
        {
            node.NodeSide = side;

            foreach (Node child in node.Children)
            {
                ChangeSide(child, side);
            }
        }

        protected void Add(List<Node> collection, Node child, int? index, NodeSide side)
        {
            if (index.HasValue)
            {
                collection.Insert(index.Value, child);
            }
            else
            {
                collection.Add(child);
            }

            ChangeSide(child, side);

            child.LinkToParent(this);

            if (Document != null)
            {
                Document.Add(child);
            }
        }

        protected bool Remove(List<Node> collection, Node child, out int oldIndex)
        {
            oldIndex = collection.IndexOf(child);

            if (oldIndex >= 0)
            {
                collection.Remove(child);

                ChangeSide(child, NodeSide.Auto);

                child.UnlinkFromParent();

                if (Document != null)
                {
                    Document.Remove(child);
                }

                return true;
            }

            return false;
        }

        public void Select()
        {
            if (Document != null)
            {
                Document.Select(this);
            }
        }
    }
}
