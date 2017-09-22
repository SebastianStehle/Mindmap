// ==========================================================================
// CompositeReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Hercules.Model2
{
    public sealed class CompositeReducer<T> : IReducer<T>
    {
        private readonly IEnumerable<IReducer<T>> reducers;

        public CompositeReducer(params IReducer<T>[] reducers)
        {
            this.reducers = reducers;
        }

        public T Reduce(T state, IAction action)
        {
            foreach (var reducer in reducers)
            {
                state = reducer.Reduce(state, action);
            }

            return state;
        }
    }
}
