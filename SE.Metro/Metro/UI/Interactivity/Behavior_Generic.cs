// ==========================================================================
// Behavior_Generic.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable object.
    /// </summary>
    public abstract class Behavior<T> : Behavior where T : FrameworkElement
    {
        #region Properties

        /// <summary>
        /// Gets a reference to the adorned object this is used by the 
        /// current instance.
        /// </summary>
        /// <value>The adorned object.</value>
        public new T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObject;
            }
        }

        #endregion  

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Behavior&lt;T&gt;"/> class.
        /// </summary>
        protected Behavior()
            : base(typeof(T))
        {
        }

        #endregion
    }
}
