using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    [XmlRoot("Step")]
    public sealed class HistoryStep
    {
        [XmlAttribute]
        public DateTimeOffset Date { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Command")]
        public List<DocumentCommandBase> Commands { get; set; }

        public HistoryStep()
        {
            Commands = new List<DocumentCommandBase>();
        }
    }
}
