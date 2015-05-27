// ==========================================================================
// JsonHistory.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mindmap.Model.Storing
{
    [DataContract]
    public sealed class JsonHistory
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<JsonHistoryStep> Steps { get; set; }

        public JsonHistory()
        {
            Steps = new List<JsonHistoryStep>();
        }
    }
}
