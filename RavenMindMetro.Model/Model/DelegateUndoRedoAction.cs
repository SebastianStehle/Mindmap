// ==========================================================================
// DelegateUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace RavenMind.Model
{
    /// <summary>
    /// An undo redo action which calls two delegates for an operation 
    /// to make it undone or to redo it.
    /// </summary>
    /// <typeparam name="TUndoResult">The result of the undo action.</typeparam>
    /// <typeparam name="TRedoResult">The result of the redo action.</typeparam>
    public sealed class DelegateUndoRedoAction<TUndoResult, TRedoResult> : IUndoRedoAction
    {
        #region Fields

        private readonly Func<TUndoResult, TRedoResult> redoAction;
        private readonly Func<TRedoResult, TUndoResult> undoAction;
        private TRedoResult redoResult;
        private TUndoResult undoResult;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateUndoRedoAction&lt;TUndoResult, TRedoResult&gt;"/> class
        /// with the result of the operationand the delegates to undo or redo the operation again.
        /// </summary>
        /// <param name="operationResult">The operation result.</param>
        /// <param name="undoAction">The undo action. Cannot be null.</param>
        /// <param name="redoAction">The redo action. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="undoAction"/> is null.
        ///     - or -
        ///     <paramref name="redoAction"/> is null.
        /// </exception>
        public DelegateUndoRedoAction(TRedoResult operationResult, Func<TRedoResult, TUndoResult> undoAction, Func<TUndoResult, TRedoResult> redoAction)
        {
            if (undoAction == null)
            {
                throw new ArgumentNullException("undoAction");
            }

            if (redoAction == null)
            {
                throw new ArgumentNullException("redoAction");
            }

            this.undoAction = undoAction;
            this.undoResult = default(TUndoResult);
            this.redoAction = redoAction;
            this.redoResult = operationResult;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines an undo method, which is called to undo all changes
        /// that has been made by this action.
        /// </summary>
        public void Undo()
        {
            undoResult = undoAction(redoResult);
        }

        /// <summary>
        /// Executes the action again.
        /// </summary>
        public void Redo()
        {
            redoResult = redoAction(undoResult);
        }

        #endregion
    }
}
