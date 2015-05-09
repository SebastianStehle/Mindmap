// ==========================================================================
// UndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public abstract class UndoRedoAction
    {
        private readonly DocumentCommandBase command;

        public DocumentCommandBase Command
        {
            get
            {
                return command;
            }
        }

        protected UndoRedoAction(DocumentCommandBase command)
        {
            this.command = command;
        }

        public abstract void Undo();

        public abstract void Redo();
    }
}
