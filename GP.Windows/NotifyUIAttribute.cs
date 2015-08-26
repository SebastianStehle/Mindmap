// ==========================================================================
// NotifyUIAttribute.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace GP.Windows
{
    /// <summary>
    /// Marks a property to notify the UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NotifyUIAttribute : Attribute
    {
    }
}
