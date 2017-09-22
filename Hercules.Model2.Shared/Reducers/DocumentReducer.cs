// ==========================================================================
// DocumentReducer.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Hercules.Model2.Actions;

namespace Hercules.Model2.Reducers
{
    class DocumentReducer : IReducer<Document>
    {
        public Document Reduce(Document state, IAction action)
        {
            switch (action)
            {
                case SelectNode selectNode:
                    return state.WithSelectedNode(selectNode.NodeId);
            }

            return state;
        }
    }
}
