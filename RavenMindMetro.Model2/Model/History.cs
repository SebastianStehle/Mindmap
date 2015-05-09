using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RavenMind.Model
{
    [XmlRoot]
    public sealed class History
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement]
        public List<HistoryStep> Steps { get; set; }

        public History()
        {
            Steps = new List<HistoryStep>();
        }
    }
}
