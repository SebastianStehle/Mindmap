// ==========================================================================
// UndoRedoManager.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace RavenMind.Model
{
    public sealed class UndoRedoManager : IUndoRedoManager
    {
        private readonly Stack<UndoRedoAction> undoStack = new Stack<UndoRedoAction>();
        private readonly Stack<UndoRedoAction> redoStack = new Stack<UndoRedoAction>();

        public event EventHandler StateChanged;

        private void OnStateChanged(EventArgs e)
        {
            EventHandler stateChanged = StateChanged;

            if (stateChanged != null)
            {
                stateChanged(this, e);
            }
        }

        public IEnumerable<UndoRedoAction> History
        {
            get
            {
                return undoStack;
            }
        }

        public bool CanUndo
        {
            get
            {
                return undoStack.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return redoStack.Count > 0;
            }
        }

        public void Reset()
        {
            undoStack.Clear();
            redoStack.Clear();

            OnStateChanged(EventArgs.Empty);
        }

        public void Undo()
        {
            if (CanUndo)
            {
                UndoInternal();

                OnStateChanged(EventArgs.Empty);
            }
        }

        public void UndoAll()
        {
            if (CanUndo)
            {
                while (CanUndo)
                {
                    UndoInternal();
                }

                OnStateChanged(EventArgs.Empty);
            }
        }

        private void UndoInternal()
        {
            UndoRedoAction lastUndoAction = undoStack.Pop();

            lastUndoAction.Undo();

            redoStack.Push(lastUndoAction);
        }

        public void Redo()
        {
            if (CanRedo)
            {
                if (CanRedo)
                {
                    RedoInternal();
                }

                OnStateChanged(EventArgs.Empty);
            }
        }

        public void RedoAll()
        {
            while (CanRedo)
            {
                RedoInternal();
            }

            OnStateChanged(EventArgs.Empty);
        }

        private void RedoInternal()
        {
            UndoRedoAction lastRedoAction = redoStack.Pop();

            lastRedoAction.Redo();

            undoStack.Push(lastRedoAction);
        }

        public void RegisterExecutedAction(UndoRedoAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            undoStack.Push(action);
            redoStack.Clear();

            OnStateChanged(EventArgs.Empty);
        }
    }
}
