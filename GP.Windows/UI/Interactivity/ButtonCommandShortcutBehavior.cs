// ==========================================================================
// ButtonCommandShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace GP.Windows.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke the command of the button when the shortcut key is pressed.
    /// </summary>
    public class ButtonCommandShortcutBehavior : ShortcutBehaviorBase
    {
        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            Button associatedButton = AssociatedElement as Button;

            if (associatedButton != null)
            {
                ICommand command = associatedButton.Command;

                if (command != null)
                {
                    object parameter = associatedButton.CommandParameter;

                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
            }
        }
    }
}
