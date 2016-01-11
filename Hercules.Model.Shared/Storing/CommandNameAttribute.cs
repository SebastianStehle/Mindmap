// ==========================================================================
// CommandNameAttribute.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;

namespace Hercules.Model.Storing
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandNameAttribute : Attribute
    {
        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        public CommandNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
