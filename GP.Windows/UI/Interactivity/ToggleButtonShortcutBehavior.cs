// ==========================================================================
// ToggleButtonShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls.Primitives;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke the command of the button when the shortcut key is pressed.
    /// </summary>
    public class ToggleButtonShortcutBehavior : ShortcutBehaviorBase
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            ToggleButton associatedToggleButton = AssociatedElement as ToggleButton;

            if (associatedToggleButton != null)
            {
                associatedToggleButton.IsChecked = !associatedToggleButton.IsChecked;
            }
        }
    }
}
