using Newtonsoft.Json;
using NuGet.Configuration;
using NuGetViz.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NuGetViz
{
    public class SharedInfo
    {
        private static SharedInfo _instance;
        private AppConfig _config;
        private ISettings _settings;

        public static SharedInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new ArgumentNullException("SharedInfo object needs to be created using Create method");
                }
                return _instance;
            }
        }

        private SharedInfo(AppConfig config, ISettings settings)
        {
            _config = config;
            _settings = settings;

            Update();
        }

        private void Update()
        {
            List<SettingValue> repoValues = new List<SettingValue>();
            foreach (var item in _config.Repositories)
            {
                repoValues.Add(new SettingValue(item.Key, item.AbsoluteUri, false));
            }

            List<SettingValue> configValues = new List<SettingValue>();
            configValues.Add(new SettingValue("globalPackagesFolder", _config.PackageDownloadFolderName, false));

            _settings.UpdateSections("activePackageSource", repoValues);
            _settings.UpdateSections("packageSources", repoValues);
            _settings.UpdateSections("config", configValues);
        }

        public static SharedInfo Create(string currentFolder)
        {
            var jsonConfig = File.ReadAllText(Path.Combine(currentFolder, "nugetviz.json"));
            var nugetConfig = new Settings(currentFolder, "NuGet.config");

            var config = JsonConvert.DeserializeObject<AppConfig>(jsonConfig);

            if (string.IsNullOrEmpty(config.PackageDownloadFolderName))
            {
                config.PackageDownloadFolderName = ".nuget";
            }

            if (!Path.IsPathRooted(config.PackageDownloadFolderName))
            {
                config.PackageDownloadFolderName = Path.Combine(currentFolder, config.PackageDownloadFolderName);
            }

            return Create(config, nugetConfig);
        }

        private static SharedInfo Create(AppConfig config, ISettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            if (config == null)
                throw new ArgumentNullException("config");

            return _instance = (_instance ?? new SharedInfo(config, settings));
        }

        public ISettings GetNuGetSettings()
        {
            return _settings;
        }

        public AppConfig GetConfig()
        {
            return _config;
        }
    }
}