// ==========================================================================
// JsonHistoryStep.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RavenMind.Model.Storing
{
    public sealed class JsonHistoryStep
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("commands")]
        public List<JsonHistoryStepCommand> Commands { get; set; }

        public JsonHistoryStep()
        {
            Commands = new List<JsonHistoryStepCommand>();
        }
    }
}
