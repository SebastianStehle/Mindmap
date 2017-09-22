// ==========================================================================
// ApplicationStateReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
namespace Hercules.Model2.Reducers
{
    public sealed class ApplicationStateReducer : IReducer<ApplicationState>
    {
        private IReducer<UndoableState<Document>> documentReducer;

        public ApplicationStateReducer(IReducer<UndoableState<Document>> documentReducer)
        {
            this.documentReducer = documentReducer;
        }

        public ApplicationState Reduce(ApplicationState state, IAction action)
        {
            var newDocument = documentReducer.Reduce(state.Document, action);

            return state.WithDocument(newDocument);
        }
    }
}
