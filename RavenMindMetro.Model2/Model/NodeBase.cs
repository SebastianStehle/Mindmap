// ==========================================================================
// Node.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RavenMind.Model
{
    public abstract class NodeBase : ISelectable, INotifyPropertyChanged
    {
        private readonly NodeId nodeId;
        private Document document;
        private NodeBase parent;
        private IconSize iconSize;
        private NodeSide side;
        private string text;
        private string iconKey;
        private int color;
        private bool isSelected;

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectionChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public object Tag { get; set; }

        public NodeId NodeId
        {
            get
            {
                return nodeId;
            }
        }

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
        
        public NodeSide Side
        {
            get
            {
                return side;
            }
            protected set
            {
                if (side != value)
                {
                    side = value;
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

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnSelectionChanged(EventArgs.Empty);
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public NodeBase(NodeId id)
        {
            color = Assets.AssetProvider.DefaultColor;

            this.nodeId = id;
        }

        public ChangeColorCommand Apply(ChangeColorCommand command)
        {
            PreprocessCommand(command);

            IsSelected = true;

            int oldColor = color;

            Color = command.Color;

            return new ChangeColorCommand { Color = oldColor, Node = this.NodeId };
        }

        public ChangeTextCommand Apply(ChangeTextCommand command)
        {
            PreprocessCommand(command);

            IsSelected = true;

            string oldText = text;

            Text = command.Text;

            return new ChangeTextCommand { Text = oldText, Node = this };
        }

        public ChangeIconKeyCommand Apply(ChangeIconKeyCommand command)
        {
            PreprocessCommand(command);

            IsSelected = true;

            string oldIconKey = iconKey;

            IconKey = command.IconKey;

            return new ChangeIconKeyCommand { IconKey = oldIconKey, Node = this };
        }

        public ChangeIconSizeCommand Apply(ChangeIconSizeCommand command)
        {
            PreprocessCommand(command);

            IsSelected = true;

            IconSize oldIconSize = iconSize;

            IconSize = command.IconSize;

            return new ChangeIconSizeCommand { IconSize = oldIconSize, Node = this };
        }
        
        public abstract InsertChildCommand Apply(RemoveChildCommand command);

        public abstract RemoveChildCommand Apply(InsertChildCommand command);

        protected void PreprocessCommand<T>(T command) where T :  DocumentCommandBase
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            Node node = this as Node;

            if (!command.Node.Equals(NodeId))
            {
                throw new ArgumentException("NodeId doesnt fit to the current id.");
            }
        }

        private void ChangeSide(Node node, NodeSide side)
        {
            node.Side = side;

            foreach (Node child in node.Children)
            {
                ChangeSide(child, side);
            }
        }

        protected RemoveChildCommand Add(List<Node> collection, InsertChildCommand command, NodeSide side)
        {
            Node child = (Node)command.NewNode.LinkedNode;

            if (command.Index.HasValue)
            {
                collection.Insert(command.Index.Value, child);
            }
            else
            {
                collection.Add(child);
            }

            ChangeSide(child, side);

            command.NewNode.LinkedNode.Parent = this;

            if (Document != null)
            {
                Document.Add(child);
            }

            command.NewNode.LinkedNode.IsSelected = true;

            return new RemoveChildCommand { Node = this, OldNode = command.NewNode };
        }

        protected InsertChildCommand Remove(List<Node> collection, RemoveChildCommand command)
        {
            Node child = (Node)command.OldNode.LinkedNode;

            int oldIndex = collection.IndexOf(child);

            collection.Remove(child);

            ChangeSide(child, NodeSide.Undefined);

            command.OldNode.LinkedNode.Parent = null;

            if (Document != null)
            {
                Document.Remove(child);
            }

            IsSelected = true;

            return new InsertChildCommand { Node = this, NewNode = command.OldNode, Index = oldIndex };
        }
    }    
}
