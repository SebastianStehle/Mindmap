// ==========================================================================
// CommandInvokingEventHandler.cs
// Metro Library SE
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace GreenParrot.Windows.UI.Interactivity
{
    /// <summary>
    /// Event arguments for an event that is raised before a command is invoked.
    /// </summary>
    public sealed class CommandInvokingEventHandler : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandInvokingEventHandler"/> is handled.
        /// </summary>
        /// <value>
        /// <c>true</c> if handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled { get; set; }
    }
}
