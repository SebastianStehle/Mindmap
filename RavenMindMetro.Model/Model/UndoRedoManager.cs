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
    /// <summary>
    /// Base implemenation of the <see cref="IUndoRedoManager"/> interface.
    /// </summary>
    public sealed class UndoRedoManager : IUndoRedoManager
    {
        #region Fields

        private readonly Stack<IUndoRedoAction> undoStack = new Stack<IUndoRedoAction>();
        private readonly Stack<IUndoRedoAction> redoStack = new Stack<IUndoRedoAction>();

        #endregion

        #region Events

        /// <summary>
        /// This event is invoked when the state of the undo redo manager has changed.
        /// This occurs, whenever an action is registered or after an action
        /// is redone or undone.
        /// </summary>
        public event EventHandler StateChanged;
        /// <summary>
        /// Raises the <see cref="E:StateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnStateChanged(EventArgs e)
        {
            EventHandler stateChanged = StateChanged;

            if (stateChanged != null)
            {
                stateChanged(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether there is a command to undo.
        /// </summary>
        /// <value><c>true</c> if there is a command to undo; otherwise, <c>false</c>.</value>
        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether there is a command to redo.
        /// </summary>
        /// <value><c>true</c> if there is a command to redo; otherwise, <c>false</c>.</value>
        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the manager by deleting all recorded actions.
        /// </summary>
        public void Reset()
        {
            undoStack.Clear();
            redoStack.Clear();

            OnStateChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Reverts the last executed change and goes back to the previous state when there are
        /// any registered changes.
        /// </summary>
        public void Undo()
        {
            if (CanUndo)
            {
                IUndoRedoAction lastUndoAction = undoStack.Pop();
                lastUndoAction.Undo();
                redoStack.Push(lastUndoAction);

                OnStateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reverts all changes that has been registered at this undo redo manager.
        /// </summary>
        public void UndoAll()
        {
            while (CanUndo)
            {
                Undo();
            }

            OnStateChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Executes the last state again that has been reverted.
        /// </summary>
        public void Redo()
        {
            if (CanRedo)
            {
                IUndoRedoAction lastRedoAction = redoStack.Pop();
                lastRedoAction.Redo();
                undoStack.Push(lastRedoAction);
            }

            OnStateChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Executes all reverted states again.
        /// </summary>
        public void RedoAll()
        {
            while (CanRedo)
            {
                Redo();
            }

            OnStateChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Registers the executed action. Call this method after the action has been
        /// executed.
        /// </summary>
        /// <param name="action">The action that has been executed. Cannot be null and
        /// must a an item of the history.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="action"/> is not
        /// a part of the history.</exception>
        public void RegisterExecutedAction(IUndoRedoAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            undoStack.Push(action);
            redoStack.Clear();

            OnStateChanged(EventArgs.Empty);
        }

        #endregion
    }
}
