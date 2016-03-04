// ==========================================================================
// Document.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GP.Utils;
using Hercules.Model.Layouting;
using Hercules.Model.Layouting.HorizontalStraight;
using PropertyChanged;

// ReSharper disable InvertIf

namespace Hercules.Model
{
    [ImplementPropertyChanged]
    public sealed class Document : DocumentObject, IDocumentCommands
    {
        private readonly NodeCache nodeCache = new NodeCache();
        private readonly HashSet<NodeBase> nodes = new HashSet<NodeBase>();
        private readonly RootNode root;
        private readonly IUndoRedoManager undoRedoManager = new UndoRedoManager();
        private readonly ILayout layout = HorizontalStraightLayout.Instance;
        private readonly Vector2 size = new Vector2(20000, 12000);
        private CompositeUndoRedoAction transaction;
        private NodeBase selectedNode;

        public event EventHandler<NodeEventArgs> NodeAdded;

        public event EventHandler<NodeEventArgs> NodeRemoved;

        public event EventHandler<NodeEventArgs> NodeSelected;

        public event EventHandler<StateChangedEventArgs> StateChanged
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

        [NotifyUI]
        public bool IsCheckableDefault { get; private set; }

        public IUndoRedoManager UndoRedoManager
        {
            get { return undoRedoManager; }
        }

        public ILayout Layout
        {
            get { return layout; }
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
            nodeCache.Add(root);

            root.LinkToDocument(this);
        }

        public static Document CreateNew(string name)
        {
            Document document = new Document(Guid.NewGuid());

            if (!string.IsNullOrWhiteSpace(name))
            {
                document.Root.ChangeTextTransactional(name);
            }

            return document;
        }

        internal void ChangeIsCheckableDefault(bool newIsCheckableDefault)
        {
            IsCheckableDefault = newIsCheckableDefault;
        }

        internal void Add(Node newNode)
        {
            if (nodes.Add(newNode))
            {
                newNode.LinkToDocument(this);

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
                oldNode.UnlinkFromDocument();

                OnNodeRemoved(oldNode);
            }

            foreach (Node child in oldNode.Children)
            {
                Remove(child);
            }

            if (oldNode.IsSelected)
            {
                Select(null);
            }
        }

        internal NodeBase GetOrCreateNode<T>(Guid id, Func<Guid, T> factory) where T : NodeBase
        {
            return nodeCache.GetOrCreateNode(id, factory);
        }

        public void Apply(IUndoRedoCommand command)
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
                foreach (IUndoRedoCommand command in transaction.Actions.OfType<IUndoRedoCommand>())
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

                OnPropertyChanged(nameof(SelectedNode));

                if (node != null)
                {
                    OnNodeSelected(node);
                }
            }
        }
    }
}
