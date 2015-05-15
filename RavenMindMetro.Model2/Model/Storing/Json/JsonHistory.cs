﻿// ==========================================================================
// JsonHistory.cs
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
    public sealed class JsonHistory
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("steps")]
        public List<JsonHistoryStep> Steps { get; set; }

        public JsonHistory()
        {
            Steps = new List<JsonHistoryStep>();
        }
    }
}
