// ==========================================================================
// RootNode.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.ObjectModel;
using System.Linq;

namespace RavenMind.Model
{
    /// <summary>
    /// A composition of actions, that are invoked at the same time.
    /// </summary>
    public sealed class CompositeUndoRedoAction : IUndoRedoAction
    {
        #region Fields

        private readonly Collection<IUndoRedoAction> actions = new Collection<IUndoRedoAction>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of actions that will be undone at the same time. 
        /// </summary>
        /// <value>The list of actions that will be undone at the same time. Will never be null.</value>
        public Collection<IUndoRedoAction> Actions
        {
            get
            {
                return actions;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines an undo method, which is called to undo all changes
        /// that has been made by this action.
        /// </summary>
        public void Undo()
        {
            foreach (IUndoRedoAction action in Actions.Reverse())
            {
                if (action != null)
                {
                    action.Undo();
                }
            }
        }

        /// <summary>
        /// Executes the action again.
        /// </summary>
        public void Redo()
        {
            foreach (IUndoRedoAction action in Actions)
            {
                if (action != null)
                {
                    action.Redo();
                }
            }
        }

        #endregion
    }
}
