// ==========================================================================
// ApplicationStore.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Reactive.Linq;
using Hercules.Model2.Actions;
using Hercules.Model2.Reducers;

namespace Hercules.Model2
{
    public sealed class ApplicationStore
    {
        private readonly Store<ApplicationState> store;

        public IObservable<Document> SelectedDocument { get; }

        public IObservable<Node> SelectedNode { get; }

        public IObservable<bool> CanUndo { get; }

        public IObservable<bool> CanRedo { get; }

        public ApplicationStore()
        {
            store =
                new Store<ApplicationState>(
                    new ApplicationStateReducer(
                        new UndoableReducer<Document>(
                            new CompositeReducer<Document>(
                                new DocumentNodeReducer()
                            ),
                            x => false)),
                    new ApplicationState());

            SelectedDocument =
                store.Select(x => x.Document.Present)
                    .DistinctUntilChanged()
                    .Publish()
                    .Replay(1)
                    .RefCount();

            SelectedNode =
                store.Select(x => x.Document.Present.GetSelectedNode())
                    .DistinctUntilChanged()
                    .Publish()
                    .Replay(1)
                    .RefCount();

            CanUndo = 
                store.Select(x => x.Document.CanUndo)
                    .DistinctUntilChanged()
                    .Publish()
                    .Replay(1)
                    .RefCount();

            CanRedo =
                store.Select(x => x.Document.CanRedo)
                    .DistinctUntilChanged()
                    .Publish()
                    .Replay(1)
                    .RefCount();
        }

        public void Undo()
        {
            store.Apply(new StateUndo());
        }

        public void Redo()
        {
            store.Apply(new StateRedo());
        }
    }
}
