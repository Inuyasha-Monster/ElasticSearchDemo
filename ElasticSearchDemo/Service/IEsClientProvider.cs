using Nest;

namespace ElasticSearchDemo.Service
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
    }
}