using CrawlElasticsearch.Models;
using CrawlElasticsearch.Repository;
using Nest;

namespace CrawlElasticsearch.Services.ElasticsearchServices
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticsearchRepository _elasticsearchRepository;

        public ElasticsearchService(IElasticsearchRepository elasticsearchRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
        }

        public async Task IndexPagesAsync(List<Articles> pages)
        {
            await _elasticsearchRepository.IndexPagesAsync(pages);
        }

        public async Task<ISearchResponse<Articles>> GetSearchPagesAsync(string query, int pageNumber, int pageSize)
        {
            return await _elasticsearchRepository.SearchPagesAsync(query, pageNumber, pageSize);
        }

        public async Task<ISearchResponse<Articles>> GetAllSearchPagesAsync(int pageNumber, int pageSize)
        {
            return await _elasticsearchRepository.GetAllPagesAsync(pageNumber, pageSize);
        }
    }
}