// ==========================================================================
// IUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    /// <summary>
    /// Defines an interface for an executed action, that can be undone or 
    /// executed again. This interface should be implemented by an command, that
    /// also implements the <see cref="T:System.Windows.Input.ICommand"/> interface 
    /// or by a custom structure.
    /// </summary>
    public interface IUndoRedoAction
    {
        /// <summary>
        /// Defines an undo method, which is called to undo all changes
        /// that has been made by this action.
        /// </summary>
        void Undo();

        /// <summary>
        /// Executes the action again.
        /// </summary>
        void Redo();
    }
}
