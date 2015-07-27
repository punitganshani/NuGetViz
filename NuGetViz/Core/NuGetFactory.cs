using NuGet.Protocol.Core.Types;
using System;
using System.Configuration;
using System.Linq;
using NuGet.Protocol.VisualStudio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGetViz.Core
{
    public class NuGetFactory
    {
        private static Dictionary<string, SourceRepository> Repositories;

        private SourceRepository _repo;

        private string _key;
        private string _sourceUrl;
    
        public SourceRepository Repo
        {
            get
            {
                return _repo;
            }
        }

        public string SourceUrl
        {
            get
            {
                return _sourceUrl;
            }
        }

        public NuGetFactory(string key) : this(key, GetSourceUrl(key))
        {

        }

        static NuGetFactory()
        {
            Repositories = new Dictionary<string, SourceRepository>();
        }

        public NuGetFactory(string key, string sourceUrl)
        {
            _key = key;
            _sourceUrl = sourceUrl;

            if (Repositories.ContainsKey(sourceUrl))
            {
                lock (this)
                {
                    if (Repositories.ContainsKey(sourceUrl))
                    {
                        _repo = Repositories[sourceUrl];
                    }
                }
            }
            else
            {
                _repo = Repository.Factory.GetVisualStudio(sourceUrl);
                lock (this)
                {
                    Repositories[sourceUrl] = _repo;
                }
            }
        }

        private static string GetSourceUrl(string key)
        {
            var repoConfig = SharedInfo.Instance.GetConfig().Repositories.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (repoConfig == null)
                throw new ConfigurationErrorsException("No NuGet repository found with key:" + key);

            return repoConfig.AbsoluteUri;
        }


        public Task<DependencyInfoResource> GetDependency()
        {
            return _repo.GetResourceAsync<DependencyInfoResource>();
        }

        public Task<MetadataResource> GetMetadata()
        {
            return _repo.GetResourceAsync<MetadataResource>();
        }

        public Task<UIMetadataResource> GetUIMetadata()
        {
            return _repo.GetResourceAsync<UIMetadataResource>();
        }

        public Task<UISearchResource> GetSearch()
        {
            return _repo.GetResourceAsync<UISearchResource>();
        }

        public Task<DownloadResource> GetDownload()
        {
            return _repo.GetResourceAsync<DownloadResource>();
        }
    }
}