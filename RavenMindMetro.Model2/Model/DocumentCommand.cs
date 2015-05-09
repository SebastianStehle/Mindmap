// ==========================================================================
// IDocumentCommand.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Xml.Serialization;

namespace RavenMind.Model
{
    [XmlInclude(typeof(ChangeColorCommand))]
    [XmlInclude(typeof(ChangeIconKeyCommand))]
    [XmlInclude(typeof(ChangeIconSizeCommand))]
    [XmlInclude(typeof(ChangeTextCommand))]
    [XmlInclude(typeof(InsertChildCommand))]
    [XmlInclude(typeof(RemoveChildCommand))]
    public abstract class DocumentCommandBase
    {
        public NodeId Node { get; set; }
    }
}
