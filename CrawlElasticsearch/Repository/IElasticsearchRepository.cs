using CrawlElasticsearch.Models;
using Nest;

namespace CrawlElasticsearch.Repository
{
    public interface IElasticsearchRepository
    {
        Task IndexPagesAsync(List<Articles> pages);
        Task<ISearchResponse<Articles>> SearchPagesAsync(string query, int pageNumber, int pageSize);
        Task<ISearchResponse<Articles>> GetAllPagesAsync(int pageNumber, int pageSize);
    }
}