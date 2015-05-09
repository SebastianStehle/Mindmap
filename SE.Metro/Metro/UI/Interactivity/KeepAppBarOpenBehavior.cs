// ==========================================================================
// KeepAppBarOpenBehavior.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

namespace SE.Metro.UI.Interactivity
{
    /// <summary>
    /// Behavior to keep the app bar open.
    /// </summary>
    public sealed class KeepAppBarOpenBehavior : Behavior<AppBar>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedObject.IsSticky = true;
            AssociatedObject.IsOpen = true;
            AssociatedObject.Closed += AssociatedObject_Closed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.Closed -= AssociatedObject_Closed;
        }

        private void AssociatedObject_Closed(object sender, object e)
        {
            AssociatedObject.IsOpen = true;
        }
    }
}
