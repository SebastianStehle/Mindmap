// ==========================================================================
// UndoRedoExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Linq;

namespace Hercules.Model
{
    public static class UndoRedoExtensions
    {
        public static bool IsLastCommand<TCommand>(this IUndoRedoManager manager) where TCommand : class, IUndoRedoAction
        {
            return IsLastCommand<TCommand>(manager, x => true);
        }

        public static bool IsLastCommand<TCommand>(this IUndoRedoManager manager, Predicate<TCommand> predicate) where TCommand : class, IUndoRedoAction
        {
            TCommand command = manager.History.FirstOrDefault() as TCommand;

            if (command == null)
            {
                CompositeUndoRedoAction composite = manager.History.FirstOrDefault() as CompositeUndoRedoAction;

                if (composite != null && composite.Commands.Count == 1)
                {
                    command = composite.Commands[0] as TCommand;
                }
            }

            return command != null && predicate(command);
        }
    }
}
