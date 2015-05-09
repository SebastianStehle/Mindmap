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
    public sealed class DelegateUndoRedoAction : UndoRedoAction
    {
        private readonly Action redoAction;
        private readonly Action undoAction;

        public DelegateUndoRedoAction(DocumentCommandBase command, Action undoAction, Action redoAction)
            : base(command)
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
            this.redoAction = redoAction;
        }

        public override void Undo()
        {
            undoAction();
        }

        public override void Redo()
        {
            redoAction();
        }
    }
}
