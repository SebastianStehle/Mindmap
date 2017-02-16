// ==========================================================================
// JsonHistoryStepsCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Hercules.Model.Storing.Json
{
    public sealed class JsonHistoryStepCommand
    {
        [JsonProperty]
        public string CommandType { get; set; }

        [JsonProperty]
        public PropertiesBag Properties { get; set; }

        public JsonHistoryStepCommand()
        {
            Properties = new PropertiesBag();
        }
    }
}
