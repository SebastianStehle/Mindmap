// ==========================================================================
// JsonHistoryItem.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Newtonsoft.Json;

namespace RavenMind.Model.Storing
{
    public sealed class JsonHistoryStepCommand
    {
        [JsonProperty("commandType")]
        public string CommandType { get; set; }

        [JsonProperty("properties")]
        public CommandProperties Properties { get; set; }

        public JsonHistoryStepCommand()
        {
            Properties = new CommandProperties();
        }
    }
}
