// ==========================================================================
// Document.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    /// <summary>
    /// Represents the whole mindmap.
    /// </summary>
    public sealed class Document
    {
        #region Fields

        private readonly SelectingCollection<NodeBase> nodes = new SelectingCollection<NodeBase>();
        private readonly HashSet<NodeBase> nodesHashSet = new HashSet<NodeBase>();
        private readonly ReadOnlyObservableCollection<NodeBase> nodesCollection;
        private readonly IUndoRedoManager undoRedoManager = new UndoRedoManager();
        private readonly PropertyManager propertyManager = new PropertyManager();
        private RootNode root;
        private bool isChangeTracking;
        private CompositeUndoRedoAction compositeAction;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selection has been changed.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                nodes.SelectionChanged += value;
            }
            remove
            {
                nodes.SelectionChanged -= value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the undo redo manager that is responsible for this document.
        /// </summary>
        /// <value>The undo redo manager for this document.</value>
        public IUndoRedoManager UndoRedoManager
        {
            get
            {
                return undoRedoManager;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the document is change tracking at the moment.
        /// </summary>
        /// <value>A value indicating if the document is change tracking at the moment.</value>
        public bool IsChangeTracking
        {
            get
            {
                return isChangeTracking;
            }
        }

        /// <summary>
        /// Gets or sets the root node of the mindmap.
        /// </summary>
        /// <value>
        /// The root node of the mindmap.
        /// </value>
        public RootNode Root
        {
            get
            {
                if (root == null)
                {
                    Root = new RootNode();
                }

                return root;
            }
            set
            {
                if (value.Document != null)
                {
                    throw new InvalidOperationException("Root node is already part of another document.");
                }

                if (root != value)
                {
                    undoRedoManager.Reset();

                    if (root != null && TryRemoveNode(root))
                    {
                        RegisterEvents(root);

                        foreach (Node child in root.LeftChildren.Union(root.RightChildren))
                        {
                            TryAdd(child);
                        }
                    }

                    root = value;

                    if (root != null && TryAddNode(root))
                    {
                        RegisterEvents(root);

                        foreach (Node child in root.LeftChildren.Union(root.RightChildren))
                        {
                            TryAdd(child);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique id of the document.
        /// </summary>
        /// <value>
        /// The unique id of the document.
        /// </value>
        [XmlAttribute]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        /// <value>
        /// The nameof the document.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the flat list of nodes.
        /// </summary>
        /// <value>
        /// The flat list of nodes.
        /// </value>
        [XmlIgnore]
        public ReadOnlyObservableCollection<NodeBase> Nodes
        {
            get { return nodesCollection; }
        }

        /// <summary>
        /// Gets or sets the selected node of the mindmap.
        /// </summary>
        /// <value>
        /// The selected of the mindmap.
        /// </value>
        [XmlIgnore]
        public NodeBase SelectedNode
        {
            get
            {
                return nodes.SelectedItem;
            }
            set
            {
                nodes.SelectedItem = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
        {
            Id = Guid.NewGuid();

            nodesCollection = new ReadOnlyObservableCollection<NodeBase>(nodes);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts a new transaction to track all changes.
        /// </summary>
        public void StartChangeTracking()
        {
            if (!isChangeTracking)
            {
                compositeAction = new CompositeUndoRedoAction();

                propertyManager.Clear();
            }

            isChangeTracking = true;
        }

        /// <summary>
        /// Stops tracking changes.
        /// </summary>
        public void StopChangeTracking()
        {
            if (isChangeTracking)
            {
                isChangeTracking = false;

                var changedProperies = propertyManager.CalculateChangedProperties();

                foreach (var property in changedProperies)
                {
                    compositeAction.Actions.Add(new PropertyChangedUndoRedoAction(property.Item1, property.Item2.PropertyName, property.Item2.NewValue, property.Item2.OldValue));
                }

                if (compositeAction.Actions.Count > 0)
                {
                    undoRedoManager.RegisterExecutedAction(compositeAction);
                }
            }
        }

        internal void TryAdd(Node node)
        {
            if (node != null)
            {
                if (TryAddNode(node))
                {
                    RegisterEvents(node);

                    if (node.Parent != null && node.Side == NodeSide.Undefined)
                    {
                        Node normalParent = node.Parent as Node;

                        if (normalParent != null)
                        {
                            node.Side = normalParent.Side;
                        }
                        else if (Root.LeftChildren.Contains(node))
                        {
                            node.Side = NodeSide.Left;
                        }
                        else
                        {
                            node.Side = NodeSide.Right;
                        }
                    }
                }

                foreach (Node child in node.Children)
                {
                    TryAdd(child);
                }
            }
        }

        internal void TryRemove(Node node)
        {
            if (node != null)
            {
                if (TryRemoveNode(node))
                {
                    UnregisterEvents(node);

                    node.Side = NodeSide.Undefined;
                }

                foreach (Node child in node.Children)
                {
                    TryRemove(child);
                }
            }
        }

        private bool TryAddNode<TNode>(TNode node) where TNode : NodeBase
        {
            bool result = false;

            if (!nodesHashSet.Contains(node))
            {
                node.UndoRedoPropertyChanged += node_UndoRedoPropertyChanged;
                node.Document = this;
                nodes.Add(node);
                nodesHashSet.Add(node);

                result = true;
            }

            return result;
        }

        private bool TryRemoveNode<TNode>(TNode node) where TNode : NodeBase
        {
            bool result = false;

            if (nodesHashSet.Contains(node))
            {
                node.UndoRedoPropertyChanged -= node_UndoRedoPropertyChanged;
                node.Document = null;
                nodes.Remove(node);
                nodesHashSet.Remove(node);

                result = true;
            }

            return result;
        }

        private void RegisterEvents(RootNode node)
        {
            node.LeftChildren.CollectionChanged  += Children_CollectionChanged;
            node.RightChildren.CollectionChanged += Children_CollectionChanged;
        }

        private void RegisterEvents(Node node)
        {
            node.Children.CollectionChanged += Children_CollectionChanged;
        }

        private void UnregisterEvents(RootNode node)
        {
            node.LeftChildren.CollectionChanged  -= Children_CollectionChanged;
            node.RightChildren.CollectionChanged -= Children_CollectionChanged;
        }

        private void UnregisterEvents(Node node)
        {
            node.Children.CollectionChanged -= Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (isChangeTracking && e.Action != NotifyCollectionChangedAction.Move && e.Action != NotifyCollectionChangedAction.Reset)
            {
                IList list = (IList)sender;

                if (e.OldItems != null)
                {
                    foreach (Node oldItem in e.OldItems)
                    {
                        AddOldItemAction(list, oldItem);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (Node newItem in e.NewItems)
                    {
                        AddNewItemAction(list, newItem);
                    }
                }
            }
        }

        private void AddNewItemAction(IList list, Node newItem)
        {
            int oldIndex = newItem.OrderIndex;

            Func<Node, Node> addAction = x => { HandleUndoRedoInsert(list, x, oldIndex); return x; };
            Func<Node, Node> remAction = x => { HandleUndoRedoRemove(list, x); return x; };

            compositeAction.Actions.Add(new DelegateUndoRedoAction<Node, Node>(newItem, remAction, addAction));
        }

        private void AddOldItemAction(IList list, Node oldItem)
        {
            int oldIndex = oldItem.OrderIndex;

            Func<Node, Node> addAction = x => { HandleUndoRedoInsert(list, x, oldIndex); return x; };
            Func<Node, Node> remAction = x => { HandleUndoRedoRemove(list, x); return x; };

            compositeAction.Actions.Add(new DelegateUndoRedoAction<Node, Node>(oldItem, addAction, remAction));
        }

        private static void HandleUndoRedoInsert(IList list, Node node, int oldIndex)
        {
            list.Insert(oldIndex - 1, node); 

            node.IsSelected = true;
        }

        private static void HandleUndoRedoRemove(IList list, Node node)
        {
            NodeBase parent = node.Parent;

            list.Remove(node);

            parent.IsSelected = true;
        }

        private void node_UndoRedoPropertyChanged(object sender, UndoRedoPropertyChangedEventArgs e)
        {
            if (isChangeTracking)
            {
                propertyManager.Register(sender, e);
            }
        }

        #endregion
    }
}
