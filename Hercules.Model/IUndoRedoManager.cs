// ==========================================================================
// IUndoRedoManager.cs
// Hercules Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Hercules.Model
{
    public interface IUndoRedoManager
    {
        event EventHandler StateChanged;

        IEnumerable<IUndoRedoAction> History { get; }

        int Index { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        void RevertTo(int index);

        void Reset();

        void Undo();

        void UndoAll();

        void Redo();

        void RedoAll();

        void RegisterExecutedAction(IUndoRedoAction action);
    }
}
