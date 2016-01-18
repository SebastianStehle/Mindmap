// ==========================================================================
// CommandShortcutBehavior.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Windows.Input;
using Windows.UI.Xaml;

namespace GP.Utils.UI.Interactivity
{
    /// <summary>
    /// A behavior to invoke a command when the shortcut key is pressed.
    /// </summary>
    public class CommandShortcutBehavior : ShortcutBehaviorBase
    {
        /// <summary>
        /// Defines the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CommandShortcutBehavior), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the command to invoke.
        /// </summary>
        /// <value>The command to invoke.</value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(CommandShortcutBehavior), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets parameter for the command to invoke.
        /// </summary>
        /// <value>The parameter for the command to invoke.</value>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Called when the shortcut must be invoked.
        /// </summary>
        protected override void InvokeShortcut()
        {
            object parameter = CommandParameter;

            if (Command == null)
            {
                return;
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }
    }
}
