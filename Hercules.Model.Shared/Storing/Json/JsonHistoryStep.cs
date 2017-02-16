// ==========================================================================
// JsonHistoryStep.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonHistoryStep
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public DateTimeOffset Date { get; set; }

        [JsonProperty]
        public List<JsonHistoryStepCommand> Commands { get; set; }

        public JsonHistoryStep()
        {
            Commands = new List<JsonHistoryStepCommand>();
        }
    }
}
