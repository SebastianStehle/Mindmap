// ==========================================================================
// CompositeUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace RavenMind.Model
{
    public sealed class CompositeUndoRedoAction : UndoRedoAction
    {
        private readonly List<UndoRedoAction> actions = new List<UndoRedoAction>();
        private readonly DateTimeOffset date;
        private readonly string name;

        public DateTimeOffset Date
        {
            get
            {
                return date;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public IReadOnlyList<UndoRedoAction> Actions
        {
            get
            {
                return actions;
            }
        }

        public CompositeUndoRedoAction(string name, DateTimeOffset date)
            : base(null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is null or empty.", "name");
            }

            this.date = date;
            this.name = name;
        }

        public void Add(UndoRedoAction action)
        {
            actions.Add(action);
        }

        public override void Undo()
        {
            IEnumerable<UndoRedoAction> allActions = actions;

            foreach (UndoRedoAction action in allActions.Reverse())
            {
                if (action != null)
                {
                    action.Undo();
                }
            }
        }

        public override void Redo()
        {
            foreach (UndoRedoAction action in actions)
            {
                if (action != null)
                {
                    action.Redo();
                }
            }
        }
    }
}
