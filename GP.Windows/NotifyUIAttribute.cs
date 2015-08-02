// ==========================================================================
// NotifyUIAttribute.cs
// Bus Portal (busliniensuche.de)
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
