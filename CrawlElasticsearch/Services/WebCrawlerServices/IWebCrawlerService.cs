using CrawlElasticsearch.Models;

namespace CrawlElasticsearch.Services.WebCrawlerServices
{
    public interface IWebCrawlerService
    {
        Task<List<Articles>> CrawlPageAsync(string url, string searchTerm);
    }
}
