// ==========================================================================
// Document.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hercules.Model
{
    public sealed class Document : DocumentObject, IDocumentCommands
    {
        private readonly Dictionary<Guid, NodeBase> nodesHashSet = new Dictionary<Guid, NodeBase>();
        private readonly HashSet<NodeBase> nodes = new HashSet<NodeBase>();
        private readonly RootNode root;
        private readonly IUndoRedoManager undoRedoManager = new UndoRedoManager();
        private readonly Vector2 size = new Vector2(5000, 5000);
        private CompositeUndoRedoAction transaction;
        private NodeBase selectedNode;
        
        public event EventHandler<NodeEventArgs> NodeAdded;

        public event EventHandler<NodeEventArgs> NodeRemoved;

        public event EventHandler<NodeEventArgs> NodeSelected;

        public event EventHandler StateChanged
        {
            add
            {
                undoRedoManager.StateChanged += value;
            }
            remove
            {
                undoRedoManager.StateChanged -= value;
            }
        }

        public IUndoRedoManager UndoRedoManager
        {
            get { return undoRedoManager; }
        }

        public IEnumerable<NodeBase> Nodes
        {
            get { return nodes; }
        }

        public NodeBase SelectedNode
        {
            get { return selectedNode; }
        }

        public RootNode Root
        {
            get { return root; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public double Width
        {
            get { return size.X; }
        }

        public double Height
        {
            get { return size.Y; }
        }

        public bool IsChangeTracking
        {
            get { return transaction != null; }
        }

        public Document(Guid id)
        {
            root = new RootNode(id);

            nodes.Add(root);
            nodesHashSet[root.Id] = root;

            root.LinkTo(this);
        }

        internal void Add(Node newNode)
        {
            if (nodes.Add(newNode))
            {
                newNode.LinkTo(this);

                OnNodeAdded(newNode);
            }

            foreach (Node child in newNode.Children)
            {
                Add(child);
            }
        }

        internal void Remove(Node oldNode)
        {
            if (nodes.Remove(oldNode))
            {
                nodesHashSet.Remove(oldNode.Id);

                oldNode.LinkTo((Document)null);

                if (oldNode.IsSelected)
                {
                    Select(null);
                }

                OnNodeRemoved(oldNode);
            }

            foreach (Node child in oldNode.Children)
            {
                Remove(child);
            }
        }

        internal NodeBase GetOrCreateNode<T>(Guid id, Func<Guid, T> factory) where T : NodeBase
        {
            NodeBase result;

            if (!nodesHashSet.TryGetValue(id, out result))
            {
                result = factory(id);

                nodesHashSet.Add(id, result);
            }

            return result;
        }
        
        public void Apply(CommandBase command)
        {
            if (IsChangeTracking)
            {
                transaction.Add(command);
            }
            else
            {
                command.Execute();
            }
        }

        public void BeginTransaction(string transactionName, DateTimeOffset? timestamp = null)
        {
            if (transaction == null)
            {
                DateTimeOffset date = timestamp ?? DateTimeOffset.Now;

                transaction = new CompositeUndoRedoAction(transactionName, date);
            }
        }

        public void CommitTransaction()
        {
            if (transaction != null)
            {
                foreach (CommandBase command in transaction.Commands)
                {
                    command.Execute();
                }

                undoRedoManager.RegisterExecutedAction(transaction);

                transaction = null;
            }
        }

        public void MakeTransaction(string transactionName, Action<IDocumentCommands> action)
        {
            if (!IsChangeTracking)
            {
                BeginTransaction(transactionName);

                action(this);

                CommitTransaction();
            }
        }

        private void OnNodeAdded(NodeBase node)
        {
            EventHandler<NodeEventArgs> eventHandler = NodeAdded;

            if (eventHandler != null)
            {
                eventHandler(this, new NodeEventArgs(node));
            }
        }

        private void OnNodeRemoved(NodeBase node)
        {
            EventHandler<NodeEventArgs> eventHandler = NodeRemoved;

            if (eventHandler != null)
            {
                eventHandler(this, new NodeEventArgs(node));
            }
        }

        private void OnNodeSelected(NodeBase node)
        {
            EventHandler<NodeEventArgs> eventHandler = NodeSelected;

            if (eventHandler != null)
            {
                eventHandler(this, new NodeEventArgs(node));
            }
        }

        public void Select(NodeBase node)
        {
            if (selectedNode != node)
            {
                if (selectedNode != null)
                {
                    selectedNode.ChangeIsSelected(false);
                }

                selectedNode = node;

                if (selectedNode != null)
                {
                    selectedNode.ChangeIsSelected(true);
                }

                OnPropertyChanged("SelectedNode");

                if (node != null)
                {
                    OnNodeSelected(node);
                }
            }
        }
    }
}
