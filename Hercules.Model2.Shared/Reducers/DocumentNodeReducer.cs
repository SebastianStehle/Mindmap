// ==========================================================================
// DocumentNodeReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model2.Actions;

namespace Hercules.Model2.Reducers
{
    public sealed class DocumentNodeReducer : IReducer<Document>
    {
        public Document Reduce(Document state, IAction action)
        {
            switch (action)
            {
                case ChangeText changeText:
                    return state.UpdateNode(changeText.NodeId, n => n.WithText(changeText.Text));
                case ChangeShape changeShape:
                    return state.UpdateNode(changeShape.NodeId, n => n.WithShape(changeShape.Shape));
                case ToggleCollapse toggleCollapse:
                    return state.UpdateNode(toggleCollapse.NodeId, n => n.WithCollapsed(!n.IsCollapsed));
            }

            return state;
        }
    }
}
