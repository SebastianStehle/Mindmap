// ==========================================================================
// NodeBase.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using GP.Utils;
using PropertyChanged;

// ReSharper disable InvertIf

namespace Hercules.Model
{
    [ImplementPropertyChanged]
    public abstract class NodeBase : DocumentObjectWithId
    {
        private Document document;
        private NodeBase parent;

        [DoNotNotify]
        public object LayoutData { get; set; }

        [DoNotNotify]
        public object RenderData { get; set; }

        [NotifyUI]
        public string Text { get; protected set; }

        [NotifyUI]
        public string Notes { get; protected set; }

        [NotifyUI]
        public INodeIcon Icon { get; protected set; }

        [NotifyUI]
        public INodeColor Color { get; protected set; } = ThemeColor.Default;

        [NotifyUI]
        public NodeSide NodeSide { get; protected set; }

        [NotifyUI]
        public IconSize IconSize { get; protected set; }

        [NotifyUI]
        public IconPosition IconPosition { get; protected set; }

        [NotifyUI]
        public CheckableMode CheckableMode { get; protected set; }

        [NotifyUI]
        public bool IsChecked { get; protected set; }

        [NotifyUI]
        public bool IsSelected { get; protected set; }

        [NotifyUI]
        public bool IsCollapsed { get; protected set; }

        [NotifyUI]
        public bool IsShowingHull { get; protected set; }

        [NotifyUI]
        public bool IsNotesEnabled { get; protected set; }

        public Document Document
        {
            get { return document; }
        }

        public NodeBase Parent
        {
            get { return parent; }
        }

        public bool HasNotes
        {
            get { return IsNotesEnabled && !string.IsNullOrWhiteSpace(Notes); }
        }

        public bool IsCheckable
        {
            get { return (document?.IsCheckableDefault == true || CheckableMode == CheckableMode.Enabled) && CheckableMode != CheckableMode.Disabled; }
        }

        public abstract bool HasChildren { get; }

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

        internal void ChangeIsShowingHull(bool newIsShowingHull)
        {
            IsShowingHull = newIsShowingHull;
        }

        internal void ChangeIsNotesEnabled(bool newIsNotesEnabled)
        {
            IsNotesEnabled = newIsNotesEnabled;
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

        internal void ChangeCheckableMode(CheckableMode newCheckableMode)
        {
            CheckableMode = newCheckableMode;
        }

        internal void ChangeText(string newText)
        {
            Text = newText;
        }

        internal void ChangeNotes(string newNotes)
        {
            Notes = newNotes;
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

        public abstract void Remove(Node child, out int oldIndex);

        public abstract void Insert(Node child, int? index, NodeSide side);

        private static void ChangeSide(Node node, NodeSide side)
        {
            node.NodeSide = side;

            foreach (var child in node.Children)
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
