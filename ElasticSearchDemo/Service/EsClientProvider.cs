using System;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using Nest.JsonNetSerializer;

namespace ElasticSearchDemo.Service
{
    public class EsClientProvider : IEsClientProvider
    {
        private readonly IConfiguration _configuration;
        private ElasticClient _client;
        public EsClientProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ElasticClient GetClient()
        {
            if (_client != null)
                return _client;

            InitClient();
            return _client;
        }

        private void InitClient()
        {
            var pool = new SingleNodeConnectionPool(new Uri(_configuration["EsUrl"]));
            var connectionSettings = new ConnectionSettings(pool, JsonNetSerializer.Default);
            _client = new ElasticClient(connectionSettings.DefaultIndex("book"));
        }
    }
}