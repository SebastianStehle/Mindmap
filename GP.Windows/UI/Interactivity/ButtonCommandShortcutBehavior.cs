// ==========================================================================
// ButtonCommandShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Windows.UI.Xaml.Controls;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke the command of the button when the key is pressed.
    /// </summary>
    public class ButtonCommandShortcutBehavior : ShortcutBehaviorBase<Button>
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            object parameter = AssociatedElement.CommandParameter;

            if (AssociatedElement.Command != null)
            {
                if (AssociatedElement.Command.CanExecute(parameter))
                {
                    AssociatedElement.Command.Execute(parameter);
                }
            }
        }
    }
}
