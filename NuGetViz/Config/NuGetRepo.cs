using Newtonsoft.Json;

namespace NuGetViz.Core
{
    public class NuGetRepo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("uri")]
        public string AbsoluteUri { get; set; }
    }
}