// ==========================================================================
// ChangeIconSizeCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class ChangeIconSizeCommand : DocumentCommandBase
    {
        [XmlElement]
        public IconSize IconSize { get; set; }
    }
}
