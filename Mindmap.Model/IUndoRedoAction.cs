// ==========================================================================
// IUndoRedoAction.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Mindmap.Model
{
    public interface IUndoRedoAction
    {
        void Undo();

        void Redo();
    }
}
