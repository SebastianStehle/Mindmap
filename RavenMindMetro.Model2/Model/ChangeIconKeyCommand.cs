// ==========================================================================
// ChangeIconKeyCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class ChangeIconKeyCommand : DocumentCommandBase
    {
        [XmlElement]
        public string IconKey { get; set; }
    }
}
