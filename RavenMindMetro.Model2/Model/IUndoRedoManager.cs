// ==========================================================================
// IUndoRedoManager.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace RavenMind.Model
{
    public interface IUndoRedoManager
    {
        event EventHandler StateChanged;

        IEnumerable<UndoRedoAction> History { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        void Reset();

        void Undo();

        void UndoAll();

        void Redo();

        void RedoAll();

        void RegisterExecutedAction(UndoRedoAction action);
    }
}
