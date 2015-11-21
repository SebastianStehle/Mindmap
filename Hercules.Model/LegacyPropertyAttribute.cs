// ==========================================================================
// LegacyPropertyAttribute.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LegacyPropertyAttribute : Attribute
    {
        private readonly string oldName;
        private readonly string newName;

        public string OldName
        {
            get { return oldName; }
        }

        public string NewName
        {
            get { return newName; }
        }

        public LegacyPropertyAttribute(string oldName, string newName)
        {
            this.oldName = oldName;
            this.newName = newName;
        }
    }
}