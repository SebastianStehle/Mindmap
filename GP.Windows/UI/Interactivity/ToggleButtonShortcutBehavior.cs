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
    /// A behavior to invoke the command of the button when the key is pressed.
    /// </summary>
    public class ToggleButtonShortcutBehavior : ShortcutBehaviorBase<ToggleButton>
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            AssociatedElement.IsChecked = !AssociatedElement.IsChecked;
        }
    }
}
