// ==========================================================================
// ChangeText.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model2.Actions
{
    public sealed class ChangeText : IAction
    {
        public Guid NodeId { get; set; }

        public string Text { get; set; }
    }
}
