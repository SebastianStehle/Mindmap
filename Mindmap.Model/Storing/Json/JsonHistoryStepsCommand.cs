// ==========================================================================
// JsonHistoryItem.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Runtime.Serialization;

namespace Mindmap.Model.Storing
{
    [DataContract]
    public sealed class JsonHistoryStepCommand
    {
        [DataMember]
        public string CommandType { get; set; }

        [DataMember]
        public CommandProperties Properties { get; set; }

        public JsonHistoryStepCommand()
        {
            Properties = new CommandProperties();
        }
    }
}
