// ==========================================================================
// JsonHistoryStepsCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System.Runtime.Serialization;

namespace Hercules.Model.Storing.Json
{
    [DataContract]
    public sealed class JsonHistoryStepCommand
    {
        [DataMember]
        public string CommandType { get; set; }

        [DataMember]
        public PropertiesBag Properties { get; set; }

        public JsonHistoryStepCommand()
        {
            Properties = new PropertiesBag();
        }
    }
}
