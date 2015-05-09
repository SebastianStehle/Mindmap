using Newtonsoft.Json;
using System;

namespace RavenMind.Model.Storing
{
    public sealed class JsonHistory
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public Guid Id { get; set; }
    }
}
