// ==========================================================================
// IUndoRedoManager.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// Manages commands and supports undo redo functionality. This class 
    /// should be used in the view model and should be only instantiated one time.
    /// </summary>
    public interface IUndoRedoManager
    {
        #region Events

        /// <summary>
        /// This event is invoked when the state of the undo redo manager has changed. 
        /// This occurs, whenever an action is registered or after an action 
        /// is redone or undone.
        /// </summary>
        event EventHandler StateChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether there is a command to undo.
        /// </summary>
        /// <value><c>true</c> if there is a command to undo; otherwise, <c>false</c>.</value>
        bool CanUndo { get; }

        /// <summary>
        /// Gets a value indicating whether there is a command to redo.
        /// </summary>
        /// <value><c>true</c> if there is a command to redo; otherwise, <c>false</c>.</value>
        bool CanRedo { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the manager by deleting all recorded actions.
        /// </summary>
        void Reset();

        /// <summary>
        /// Reverts the last executed change and goes back to the previous state when there are 
        /// any registered changes.
        /// </summary>
        void Undo();

        /// <summary>
        /// Reverts all changes that has been registered at this undo redo manager.
        /// </summary>
        void UndoAll();

        /// <summary>
        /// Executes the last state again that has been reverted.
        /// </summary>
        void Redo();

        /// <summary>
        /// Executes all reverted states again.
        /// </summary>
        void RedoAll();

        /// <summary>
        /// Registers the executed action. Call this method after the action has been 
        /// executed.
        /// </summary>
        /// <param name="action">The action that has been executed. Cannot be null and
        /// must a an item of the history.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="action"/> is not 
        /// a part of the history.</exception>
        void RegisterExecutedAction(IUndoRedoAction action);

        #endregion
    }
}
