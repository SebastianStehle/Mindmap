// ==========================================================================
// ChangeTextCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class ChangeTextCommand : DocumentCommandBase
    {
        [XmlElement]
        public string Text { get; set; }
    }
}
