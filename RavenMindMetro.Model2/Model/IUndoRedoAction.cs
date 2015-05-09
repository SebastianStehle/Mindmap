// ==========================================================================
// IUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace RavenMind.Model
{
    public interface IUndoRedoAction
    {
        void Undo();

        void Redo();
    }
}
