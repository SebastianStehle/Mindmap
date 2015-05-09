// ==========================================================================
// Document.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mindmap.Model
{
    public sealed class Document
    {
        private readonly SelectingCollection<NodeBase> nodes = new SelectingCollection<NodeBase>();
        private readonly Dictionary<NodeId, NodeBase> nodesHashSet = new Dictionary<NodeId, NodeBase>();
        private readonly ReadOnlyObservableCollection<NodeBase> nodesCollection;
        private readonly RootNode root;
        private readonly IUndoRedoManager undoRedoManager = new UndoRedoManager();
        private readonly HashSet<Node> nodesToAdd = new HashSet<Node>();
        private readonly HashSet<Node> nodesToRemove = new HashSet<Node>();
        private readonly List<DocumentCommandBase> commands = new List<DocumentCommandBase>();
        private CompositeUndoRedoAction transaction;
        private Guid id;
        private string name;

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

        public event EventHandler<CollectionItemEventArgs<NodeBase>> NodeRemoved
        {
            add
            {
                nodes.ItemRemoved += value;
            }
            remove
            {
                nodes.ItemRemoved -= value;
            }
        }

        public event EventHandler<CollectionItemEventArgs<NodeBase>> NodeAdded
        {
            add
            {
                nodes.ItemAdded += value;
            }
            remove
            {
                nodes.ItemAdded -= value;
            }
        }

        public IUndoRedoManager UndoRedoManager
        {
            get
            {
                return undoRedoManager;
            }
        }

        public bool IsChangeTracking
        {
            get
            {
                return transaction != null;
            }
        }

        public RootNode Root
        {
            get
            {
                return root;
            }
        }

        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
        
        public ReadOnlyObservableCollection<NodeBase> Nodes
        {
            get
            {
                return nodesCollection;
            }
        }

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

        public Document(Guid rootId, string name)
        {
            id = rootId;

            root = new RootNode(rootId, name);

            this.name = name;

            nodesCollection = new ReadOnlyObservableCollection<NodeBase>(nodes);
            nodes.Add(root);

            nodesHashSet[root.NodeId] = root;

            root.Document = this;
        }

        internal void Add(Node newNode)
        {
            if (!nodesHashSet.ContainsKey(newNode.NodeId))
            {
                if (IsChangeTracking)
                {
                    nodesToAdd.Add(newNode);
                }
                else
                {
                    nodesHashSet.Add(newNode.NodeId, newNode);
                    nodes.Add(newNode);
                }

                newNode.Document = this;
            }

            foreach (Node child in newNode.Children)
            {
                Add(child);
            }
        }

        internal void Remove(Node oldNode)
        {
            if (nodesHashSet.ContainsKey(oldNode.NodeId))
            {
                if (IsChangeTracking)
                {
                    nodesToRemove.Add(oldNode);
                }
                else
                {
                    nodesHashSet.Remove(oldNode.NodeId);
                    nodes.Remove(oldNode);
                }

                oldNode.Document = null;
            }

            foreach (Node child in oldNode.Children)
            {
                Remove(child);
            }
        }
        
        public void Apply(DocumentCommandBase command)
        {
            if (transaction == null)
            {
                throw new InvalidOperationException();
            }

            commands.Add(command);
        }

        private void ApplyOnTransaction(DocumentCommandBase command)
        {
            command.Node.Link(nodesHashSet[command.Node.Id]);

            if (command is ChangeColorCommand)
            {
                Apply((ChangeColorCommand)command);
            }
            else if (command is ChangeIconKeyCommand)
            {
                Apply((ChangeIconKeyCommand)command);
            }
            else if (command is ChangeIconSizeCommand)
            {
                Apply((ChangeIconSizeCommand)command);
            }
            else if (command is ChangeTextCommand)
            {
                Apply((ChangeTextCommand)command);
            }
            else if (command is InsertChildCommand)
            {
                Apply((InsertChildCommand)command);
            }
            else if (command is RemoveChildCommand)
            {
                Apply((RemoveChildCommand)command);
            }
        }

        private void Apply(ChangeColorCommand command)
        {
            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        private void Apply(ChangeTextCommand command)
        {
            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        private void Apply(ChangeIconKeyCommand command)
        {
            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        private void Apply(ChangeIconSizeCommand command)
        {
            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        private void Apply(InsertChildCommand command)
        {
            NodeBase node;

            if (nodesHashSet.TryGetValue(command.NewNode, out node))
            {
                command.NewNode.Link(node);
            }
            else
            {
                command.NewNode.Link(new Node(command.NewNode));
            }
            
            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        private void Apply(RemoveChildCommand command)
        {
            command.OldNode.Link(nodesHashSet[command.OldNode.Id]);

            var undo = command.Node.LinkedNode.Apply(command);

            RegisterExecutedAction(new DelegateUndoRedoAction(command, () => undo.Node.LinkedNode.Apply(undo), () => command.Node.LinkedNode.Apply(command)));
        }

        public void RegisterExecutedAction(UndoRedoAction action)
        {
            if (transaction != null)
            {
                transaction.Add(action);
            }
        }

        public void BeginTransaction(string transactionName, DateTimeOffset? timestamp = null)
        {
            if (transaction == null)
            {
                commands.Clear();

                DateTimeOffset date = timestamp.HasValue ? timestamp.Value : DateTime.Now;

                transaction = new CompositeUndoRedoAction(transactionName, date);
            }
        }

        public void CommitTransaction()
        {
            if (transaction != null)
            {
                foreach (DocumentCommandBase command in commands)
                {
                    ApplyOnTransaction(command);
                }

                foreach (Node nodeToRemove in nodesToRemove)
                {
                    if (!nodesToAdd.Contains(nodeToRemove))
                    {
                        nodesHashSet.Remove(nodeToRemove.NodeId);

                        nodes.Remove(nodeToRemove);
                    }
                }

                foreach (Node nodeToAdd in nodesToAdd)
                {
                    if (!nodesToRemove.Contains(nodeToAdd))
                    {
                        nodesHashSet[nodeToAdd.NodeId] = nodeToAdd;

                        nodes.Add(nodeToAdd);
                    }
                }

                nodesToAdd.Clear();
                nodesToRemove.Clear();

                undoRedoManager.RegisterExecutedAction(transaction);

                transaction = null;
            }
        }
    }
}
