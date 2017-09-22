// ==========================================================================
// UndoableState.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hercules.Model2
{
    public sealed class UndoableState<T>
    {
        private readonly int pastCapacity;
        private ImmutableList<(T State, IAction Action)> undoStack = ImmutableList<(T State, IAction Action)>.Empty;
        private ImmutableList<(T State, IAction Action)> redoStack = ImmutableList<(T State, IAction Action)>.Empty;
        private IAction presentState;
        private (T State, IAction Action) present;

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public T Present
        {
            get { return present.State; }
        }

        public IEnumerable<IAction> Actions
        {
            get { return undoStack.Select(x => x.Action); }
        }

        public UndoableState(int pastCapacity)
        {
            this.pastCapacity = pastCapacity;
        }

        public UndoableState<T> Undo()
        {
            if (!CanUndo)
            {
                return this;
            }

            return Clone(s =>
            {
                s.undoStack = undoStack.RemoveAt(0);
                s.redoStack = redoStack.Insert(0, present);

                s.present = undoStack.Last();
            });
        }

        public UndoableState<T> Redo()
        {
            if (!CanRedo)
            {
                return this;
            }

            return Clone(s =>
            {
                s.undoStack = undoStack.Insert(0, present);
                s.redoStack = redoStack.RemoveAt(0);

                s.present = redoStack.First();
            });
        }

        public UndoableState<T> Executed(T state, IAction action)
        {
            return Clone(s =>
            {
                s.undoStack = undoStack.Add(s.present);
                s.redoStack = ImmutableList<(T State, IAction Action)>.Empty;

                s.present = (state, action);

                if (s.undoStack.Count > pastCapacity)
                {
                    s.undoStack = s.undoStack.RemoveAt(0);
                }
            });
        }

        public UndoableState<T> ReplacePresent(T state)
        {
            return Clone(s => s.present = (state, s.present.Action));
        }

        private UndoableState<T> Clone(Action<UndoableState<T>> change)
        {
            var result = (UndoableState<T>)MemberwiseClone();

            change(result);

            return result;
        }
    }
}
