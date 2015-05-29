// ==========================================================================
// JsonHistoryStep.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MindmapApp.Model.Storing
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
