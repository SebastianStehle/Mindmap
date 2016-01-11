// ==========================================================================
// AppBarButtonsBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

namespace GP.Utils.UI.Interactivity
{
    /// <summary>
    /// Handles the style of button ins the content area of a command bar.
    /// </summary>
    public sealed class AppBarButtonsBehavior : Behavior<CommandBar>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedElement.Opening += AssociatedElement_Opening;
            AssociatedElement.Closed += AssociatedElement_Closed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedElement.Opening -= AssociatedElement_Opening;
            AssociatedElement.Closed -= AssociatedElement_Closed;
        }

        private void AssociatedElement_Closed(object sender, object e)
        {
            foreach (AppBarButton button in AssociatedElement.Content.FindChildren<AppBarButton>())
            {
                button.IsCompact = true;
            }
        }

        private void AssociatedElement_Opening(object sender, object e)
        {
            foreach (AppBarButton button in AssociatedElement.Content.FindChildren<AppBarButton>())
            {
                button.IsCompact = false;
            }
        }
    }
}
