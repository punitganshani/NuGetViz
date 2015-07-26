using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class D3DependencyChild
    {
        [JsonProperty("key")]
        public string key { get; set; }

        [JsonProperty("name")]
        public string name
        {
            get
            {
                return key;
            }
        }


        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("parent")]
        public string parent { get; set; }

        [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
        public List<D3DependencyChild> children { get; set; }

        [JsonProperty]
        public int Level { get; set; } 

        public D3DependencyChild()
        {

        }
    }
}