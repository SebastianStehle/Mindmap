// ==========================================================================
// LegacyNameAttribute.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LegacyNameAttribute : Attribute
    {
        private readonly string oldName;

        public string OldName
        {
            get { return oldName; }
        }

        public LegacyNameAttribute(string oldName)
        {
            this.oldName = oldName;
        }
    }
}