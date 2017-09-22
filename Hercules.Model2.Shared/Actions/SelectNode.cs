// ==========================================================================
// SelectNode.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model2.Actions
{
    public class SelectNode : IAction
    {
        public Guid? NodeId { get; set; }
    }
}
