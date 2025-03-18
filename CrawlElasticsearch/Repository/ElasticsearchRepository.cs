using CrawlElasticsearch.Models;
using Nest;

namespace CrawlElasticsearch.Repository
{
    public class ElasticsearchRepository : IElasticsearchRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _index;

        public ElasticsearchRepository(IElasticClient elasticClient, string index)
        {
            _elasticClient = elasticClient;
            _index = index;
        }

        //Data indexing process
        public async Task IndexPagesAsync(List<Articles> pages)
        {
            foreach (var page in pages)
            {
                var existingPage = await SearchPageByTitleAndUrlAsync(page.Title, page.Url); // Do not save if similar data exists
                if (existingPage == null)
                {
                    var response = await _elasticClient.IndexDocumentAsync(page);
                    if (!response.IsValid)
                    {
                        Console.WriteLine($"Error indexing page {page.Url}");
                    }
                }
            }
        }

        //Search for a word in the data
        public async Task<ISearchResponse<Articles>> SearchPagesAsync(string query, int pageNumber, int pageSize)
        {
            var response = await _elasticClient.SearchAsync<Articles>(s => s
                .Query(q => q
                    .Wildcard(m => m
                        .Field(f => f.Title)
                        .Value($"*{query.ToLower()}*")
                    )
                )
                .From((pageNumber - 1) * pageSize)
                .Size(pageSize)
            );

            return response;
        }

        //Fetch all data
        public async Task<ISearchResponse<Articles>> GetAllPagesAsync(int pageNumber, int pageSize)
        {
            var response = await _elasticClient.SearchAsync<Articles>(s => s
                .From((pageNumber - 1) * pageSize)
                .Size(pageSize)
            );

            return response;
        }

        //Finding similar data in the data
        private async Task<Articles> SearchPageByTitleAndUrlAsync(string title, string url)
        {
            var response = await _elasticClient.SearchAsync<Articles>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Match(mp => mp.Field(f => f.Title).Query(title)),
                            m => m.Match(mp => mp.Field(f => f.Url).Query(url))
                        )
                    )
                )
            );

            return response.Documents.FirstOrDefault();
        }
    }
}
