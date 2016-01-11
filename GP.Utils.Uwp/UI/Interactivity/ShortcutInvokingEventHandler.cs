// ==========================================================================
// ShortcutInvokingEventHandler.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace GP.Utils.UI.Interactivity
{
    /// <summary>
    /// Event arguments for an event that is raised before a command is invoked.
    /// </summary>
    public sealed class ShortcutInvokingEventHandler : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ShortcutInvokingEventHandler"/> is handled.
        /// </summary>
        /// <value>
        /// <c>true</c> if handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled { get; set; }
    }
}
