using System;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    public sealed class InsertChildCommand : DocumentCommandBase
    {
        [XmlElement]
        public NodeId NewNode { get; set; }

        [XmlElement]
        public int? Index { get; set; }

        [XmlElement]
        public NodeSide Side { get; set; }
    }
}
