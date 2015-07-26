using Newtonsoft.Json;
using NuGetViz.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetViz.Tests.Fixture
{
    public class ConfigurationFixture
    {
        public ConfigurationFixture()
        {
            var jsonConfig = File.ReadAllText(@"nugetviz.json");
            SharedInfo.Instance.Config = JsonConvert.DeserializeObject<AppConfig>(jsonConfig);
        }

        public void Dispose()
        {
        }
    }
}
