// ==========================================================================
// JsonHistoryStep.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hercules.Model.Storing.Json
{
    [DataContract]
    public sealed class JsonHistoryStep
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTimeOffset Date { get; set; }

        [DataMember]
        public List<JsonHistoryStepCommand> Commands { get; set; }

        public JsonHistoryStep()
        {
            Commands = new List<JsonHistoryStepCommand>();
        }
    }
}
