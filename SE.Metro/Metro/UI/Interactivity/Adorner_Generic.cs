// ==========================================================================
// Adorner_Generic.cs
// SE Requirements Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;

namespace SE.Metro.UI.Interactivity
{    
    /// <summary>
    /// Anstract base class for adorners with a generic reference to the 
    /// adornered control.
    /// </summary>
    /// <typeparam name="T">The type of the control. Must derive from <see cref="DependencyObject"/>.</typeparam>
    /// <remarks>Always derive from this class instead of deriving from <see cref="Adorner"/>, because 
    /// it provides a type safe access to the adorned control.</remarks>
    public abstract class Adorner<T> : Adorner where T : FrameworkElement
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
        /// Initializes a new instance of the <see cref="Adorner&lt;T&gt;"/> class.
        /// </summary>
        protected Adorner()
            : base(typeof(T))
        {
        }

        #endregion
    }
}
