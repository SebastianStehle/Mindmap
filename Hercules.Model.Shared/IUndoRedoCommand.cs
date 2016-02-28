// ==========================================================================
// IUndoRedoCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model
{
    public interface IUndoRedoCommand : IUndoRedoAction
    {
        void Execute();

        void Save(PropertiesBag properties);
    }
}
