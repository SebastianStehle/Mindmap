// ==========================================================================
// UndoableReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model2.Actions;

namespace Hercules.Model2.Reducers
{
    public sealed class UndoableReducer<T> : IReducer<UndoableState<T>> where T : class
    {
        private readonly IReducer<T> stateReducer;
        private readonly Func<IAction, bool> ignore;

        public UndoableReducer(IReducer<T> stateReducer, Func<IAction, bool> ignore)
        {
            this.stateReducer = stateReducer;

            this.ignore = ignore;
        }

        public UndoableState<T> Reduce(UndoableState<T> state, IAction action)
        {
            switch (action)
            {
                case StateUndo undo:
                    return state.Undo();
                case StateRedo redo:
                    return state.Redo();
            }

            var newPresent = stateReducer.Reduce(state.Present, action);

            if (newPresent == state.Present)
            {
                return state;
            }
            else if (ignore(action))
            {
                return state.ReplacePresent(newPresent);
            }
            else
            {
                return state.Executed(newPresent, action);
            }
        }
    }
}
