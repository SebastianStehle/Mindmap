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
    public class Document : DocumentObject
    {
        private readonly UndoRedoStack<DocumentState> undoRedoStack;
        private readonly Vector2 size = new Vector2(20000, 20000);
        private DocumentStateProjections projections;

        public DocumentState Current
        {
            get { return undoRedoStack.Current; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Document(Guid id)
        {
            undoRedoStack  = new UndoRedoStack<DocumentState>(new DocumentState(Guid.NewGuid()));
            undoRedoStack.StateChanged += UndoRedoStack_StateChanged;

            projections = new DocumentStateProjections(undoRedoStack.Current);
        }

        public RootNode Root()
        {
            return projections.Root();
        }

        public NodeBase Parent(NodeBase node)
        {
            return projections.Parent(node.Id);
        }

        public IReadOnlyDictionary<Guid, NodeBase> Nodes()
        {
            return projections.Nodes();
        }

        public IReadOnlyList<Node> Children(Node node)
        {
            return projections.Children(node);
        }

        public IReadOnlyList<Node> LeftMainNodes()
        {
            return projections.LeftMainNodes();
        }

        public IReadOnlyList<Node> RightMainNodes()
        {
            return projections.RightMainNodes();
        }

        public void Dispatch(dynamic action)
        {
            DocumentState newState = undoRedoStack.Current.Dispatch(action, this);

            if (undoRedoStack.Current == newState)
            {
                return;
            }

            undoRedoStack.Update(newState, !(action is SelectNode));
        }

        private void UndoRedoStack_StateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Current));

            projections = new DocumentStateProjections(undoRedoStack.Current);
        }
    }
}
