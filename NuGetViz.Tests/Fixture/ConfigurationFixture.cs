using Newtonsoft.Json;
using NuGetViz.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace NuGetViz.Tests.Fixture
{
    public class ConfigurationFixture
    {
        public ConfigurationFixture()
        {
            //var jsonConfig = File.ReadAllText(@"nugetviz.json");
            //var nugetConfig = new Settings(System.Environment.CurrentDirectory, "NuGet.config");
            SharedInfo.Create(System.Environment.CurrentDirectory);
            //JsonConvert.DeserializeObject<AppConfig>(jsonConfig), nugetConfig);
        }

        public void Dispose()
        {
        }
    }
}
