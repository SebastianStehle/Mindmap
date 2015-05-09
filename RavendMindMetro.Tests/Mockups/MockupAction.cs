// ==========================================================================
// MockupAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using RavenMind.Model;

namespace RavenMind.Mockups
{
    internal class MockupAction : IUndoRedoAction
    {
        #region IUndoRedoAction Members

        public string Description { get; set; }

        public bool IsUndoInvoked { get; private set; }
        public bool IsRedoInvoked { get; private set; }

        public void Undo()
        {
            IsUndoInvoked = true;
        }

        public void Redo()
        {
            IsRedoInvoked = true;
        }

        #endregion
    }
}
