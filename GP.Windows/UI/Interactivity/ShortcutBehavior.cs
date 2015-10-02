// ==========================================================================
// ShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke the event when the key is pressed.
    /// </summary>
    public class ShortcutBehavior : ShortcutBehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
        }
    }
}
