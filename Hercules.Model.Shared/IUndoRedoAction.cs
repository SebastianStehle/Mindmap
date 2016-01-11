// ==========================================================================
// IUndoRedoAction.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model
{
    public interface IUndoRedoAction
    {
        void Undo();

        void Redo();
    }
}
