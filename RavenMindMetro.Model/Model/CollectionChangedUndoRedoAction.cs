// ==========================================================================
// CollectionChangedUndoRedoAction.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections;

namespace RavenMind.Model
{
    /// <summary>
    /// Stores all information about a changed collection, which is a added or removed 
    /// item of a collection.
    /// </summary>
    public sealed class CollectionChangedUndoRedoAction : IUndoRedoAction
    {
        #region Properties

        private IList targetCollection;
        /// <summary>
        /// Gets the target collection which has been changed.
        /// </summary>
        /// <value>The target collection which has been changed.</value>
        public IList TargetCollection
        {
            get
            {
                return targetCollection;
            }
        }

        private object target;
        /// <summary>
        /// Gets the target shape, which has been added to a collection
        /// or removed from it.
        /// </summary>
        /// <value>The target shape, which has been added to a collection
        /// or removed from it.</value>
        public object Target
        {
            get
            {
                return target;
            }
        }

        /// <summary>
        /// Gets or sets the operation of the arrangment.
        /// </summary>
        /// <value>The arrangment operation.</value>
        public CollectionChangedOperation Operation { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedUndoRedoAction"/> class with the changed collection 
        /// and the target object.
        /// </summary>
        /// <param name="targetCollection">The target collection which has been changed. Cannot be null.</param>
        /// <param name="target">The target object, which has been added to a collection
        /// or removed from it. Cannot be null.</param>
        /// <param name="operation">The arrangment operation.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="targetCollection"/> is null.
        ///     -or - 
        ///     <paramref name="target"/> is null.
        /// </exception>
        public CollectionChangedUndoRedoAction(IList targetCollection, object target, CollectionChangedOperation operation)
        {
            if (targetCollection == null)
            {
                throw new ArgumentNullException("targetCollection");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.targetCollection = targetCollection;
            this.target = target;

            Operation = operation;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines an undo method, which is called to undo all changes
        /// that has been made by this action.
        /// </summary>
        public void Undo()
        {
            if (Operation == CollectionChangedOperation.Added)
            {
                targetCollection.Remove(target);
            }
            else
            {
                targetCollection.Add(target);
            }
        }

        /// <summary>
        /// Executes the action again.
        /// </summary>
        public void Redo()
        {
            if (Operation == CollectionChangedOperation.Removed)
            {
                targetCollection.Remove(target);
            }
            else
            {
                targetCollection.Add(target);
            }
        }

        #endregion
    }
}
