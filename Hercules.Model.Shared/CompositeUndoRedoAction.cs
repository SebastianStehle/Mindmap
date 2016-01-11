// ==========================================================================
// CompositeUndoRedoAction.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class CompositeUndoRedoAction : IUndoRedoAction
    {
        private readonly List<IUndoRedoAction> actions = new List<IUndoRedoAction>();
        private readonly DateTimeOffset date;
        private readonly string name;

        public DateTimeOffset Date
        {
            get { return date; }
        }

        public string Name
        {
            get { return name; }
        }

        public IReadOnlyList<IUndoRedoAction> Actions
        {
            get { return actions; }
        }

        public CompositeUndoRedoAction(string name, DateTimeOffset date)
        {
            Guard.NotNullOrEmpty(name, nameof(name));

            this.date = date;
            this.name = name;
        }

        public void Add(IUndoRedoAction command)
        {
            Guard.NotNull(command, nameof(command));

            actions.Add(command);
        }

        public void Undo()
        {
            foreach (IUndoRedoAction action in actions.OfType<IUndoRedoAction>().Reverse())
            {
                action.Undo();
            }
        }

        public void Redo()
        {
            foreach (IUndoRedoAction action in actions)
            {
                action.Redo();
            }
        }
    }
}
