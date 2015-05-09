using System;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class RemoveChildCommand : DocumentCommandBase
    {
        [XmlElement]
        public NodeId OldNode { get; set; }
    }
}
