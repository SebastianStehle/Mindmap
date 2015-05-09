// ==========================================================================
// ChangeColorCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class ChangeColorCommand : DocumentCommandBase
    {
        [XmlElement]
        public int Color { get; set; }
    }
}
