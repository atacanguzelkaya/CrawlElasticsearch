using CrawlElasticsearch.Models;
using Nest;

namespace CrawlElasticsearch.Services.ElasticsearchServices
{
    public interface IElasticsearchService
    {
        Task IndexPagesAsync(List<Articles> pages);
        Task<ISearchResponse<Articles>> GetSearchPagesAsync(string query, int pageNumber, int pageSize);
        Task<ISearchResponse<Articles>> GetAllSearchPagesAsync(int pageNumber, int pageSize);
    }
}
