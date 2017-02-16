// ==========================================================================
// UndoRedoExtensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GP.Utils;

// ReSharper disable InvertIf

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
            var command = LastCommand(manager, predicate);

            return command != null;
        }

        public static TCommand LastCommand<TCommand>(this IUndoRedoManager manager, Predicate<TCommand> predicate) where TCommand : class, IUndoRedoAction
        {
            var command = manager.History.FirstOrDefault() as TCommand;

            if (command == null)
            {
                var composite = manager.History.FirstOrDefault() as CompositeUndoRedoAction;

                if (composite != null && composite.Actions.Count == 1)
                {
                    command = composite.Actions[0] as TCommand;
                }
            }

            return command != null && predicate(command) ? command : null;
        }

        public static IEnumerable<IUndoRedoCommand> Commands(this IUndoRedoManager manager)
        {
            foreach (var action in manager.History)
            {
                var command = action as IUndoRedoCommand;

                if (command == null)
                {
                    var composite = action as CompositeUndoRedoAction;

                    if (composite != null)
                    {
                        foreach (var nested in composite.Actions.OfType<IUndoRedoCommand>())
                        {
                            yield return nested;
                        }
                    }
                }
                else
                {
                    yield return command;
                }
            }
        }
    }
}
