// ==========================================================================
// Behavior.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable object.
    /// </summary>
    /// <remarks>This is an infrastructure class. Behavior authors should derive from Behavior&lt;T&gt; instead of from this class.</remarks>
    public abstract class Behavior : AttachedObject, IAttachedObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Behavior"/> class with the 
        /// type of the associated object.
        /// </summary>
        /// <param name="associatedType">Type of the associated object. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="associatedType"/> is null.</exception>
        protected Behavior(Type associatedType)
            : base(associatedType)
        {
        }

        #endregion
    }
}
