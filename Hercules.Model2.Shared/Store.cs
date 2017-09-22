// ==========================================================================
// Store.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hercules.Model2
{
    public sealed class Store<T>
    {
        private readonly IReducer<T> reducer;
        private readonly BehaviorSubject<T> state;
        private readonly Subject<IAction> actions = new Subject<IAction>();

        public IObservable<T> State
        {
            get { return state; }
        }

        public IObservable<IAction> Actions
        {
            get { return actions; }
        }

        public Store(IReducer<T> reducer, T initialState)
        {
            this.reducer = reducer;

            state = new BehaviorSubject<T>(initialState);
        }

        public void Apply(IAction action)
        {
            state.OnNext(reducer.Reduce(state.Value, action));

            actions.OnNext(action);
        }

        public IObservable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            return state.Select(selector);
        }
    }
}
