﻿// ==========================================================================
// Node.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Hercules.Model
{
    public abstract class NodeBase : DocumentObject
    {
        private Document document;
        private NodeBase parent;
        private IconSize iconSize;
        private NodeSide nodeSide;
        private int color;
        private string text;
        private string iconKey;
        private bool isCollapsed;
        private bool isSelected;

        public object LayoutData { get; set; }

        public object RenderData { get; set; }

        public abstract bool HasChildren { get; }

        public Document Document
        {
            get
            {
                return document;
            }
        }

        public NodeBase Parent
        {
            get
            {
                return parent;
            }
        }

        public int Color
        {
            get
            {
                return color;
            }
            protected set
            {
                if (color != value)
                {
                    color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            protected set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged("Text");
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
                if (iconSize != value)
                {
                    iconSize = value;
                    OnPropertyChanged("IconSize");
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
                if (nodeSide != value)
                {
                    nodeSide = value;
                    OnPropertyChanged("NodeSide");
                }
            }
        }

        public string IconKey
        {
            get
            {
                return iconKey;
            }
            protected set
            {
                if (iconKey != value)
                {
                    iconKey = value;
                    OnPropertyChanged("IconKey");
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
                if (isCollapsed != value)
                {
                    isCollapsed = value;
                    OnPropertyChanged("IsCollapsed");
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
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        protected NodeBase(Guid id)
            : base(id)
        {
        }

        internal void ChangeIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
        }

        internal void ChangeIsCollapsed(bool isCollapsed)
        {
            IsCollapsed = isCollapsed;
        }

        internal void ChangeColor(int newColor)
        {
            Color = newColor;
        }

        internal void ChangeIconKey(string newIconKey)
        {
            IconKey = newIconKey;
        }

        internal void ChangeIconSize(IconSize newIconSize)
        {
            IconSize = newIconSize;
        }

        internal void ChangeText(string newText)
        {
            Text = newText;
        }

        internal void LinkTo(Document newDocument)
        {
            document = newDocument;
        }

        internal void LinkTo(NodeBase newParent)
        {
            parent = newParent;
        }

        public abstract bool HasChild(Node child);

        public abstract bool Remove(Node child, out int oldIndex);

        public abstract void Insert(Node child, int? index, NodeSide side);

        private void ChangeSide(Node node, NodeSide side)
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

            child.LinkTo(this);

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

                ChangeSide(child, NodeSide.Undefined);

                child.LinkTo((NodeBase)null);

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