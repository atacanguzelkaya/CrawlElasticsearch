using CrawlElasticsearch.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using CrawlElasticsearch.Services.ElasticsearchServices;
using CrawlElasticsearch.Services.WebCrawlerServices;

public class IndexModel : PageModel
{
    private readonly IWebCrawlerService _webCrawler;
    private readonly IElasticsearchService _elasticsearchService;

    public IndexModel(IWebCrawlerService webCrawlerService, IElasticsearchService elasticsearchService)
    {
        _webCrawler = webCrawlerService;
        _elasticsearchService = elasticsearchService;
    }

    public List<Articles> SearchResults { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 12;

    // Crawling the page and indexing it in Elasticsearch
    public async Task<IActionResult> OnPostAsync(string url, string searchTerm)
    {
        if (string.IsNullOrEmpty(url))
        {
            ModelState.AddModelError(string.Empty, "URL cannot be empty.");
            return Page();
        }

        try
        {
            var pages = await _webCrawler.CrawlPageAsync(url, searchTerm);
            await _elasticsearchService.IndexPagesAsync(pages);
            await Task.Delay(1000);
            return RedirectToPage("./Index", new { query = searchTerm, pageNumber = 1 });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return Page();
        }
    }

    // Searching Process with pagination
    public async Task OnGetAsync(string query, int pageNumber = 1)
    {
        try
        {
            if (!string.IsNullOrEmpty(query))
            {
                var result = await _elasticsearchService.GetSearchPagesAsync(query, pageNumber, PageSize);
                SearchResults = result.Documents.ToList();
                CurrentPage = pageNumber;
                TotalPages = (int)Math.Ceiling((double)result.HitsMetadata.Total.Value / PageSize);
            }
            else
            {
                var result = await _elasticsearchService.GetAllSearchPagesAsync(pageNumber, PageSize);
                SearchResults = result.Documents.ToList();
                CurrentPage = pageNumber;
                TotalPages = (int)Math.Ceiling((double)result.HitsMetadata.Total.Value / PageSize);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error occurred while retrieving data: {ex.Message}");
        }
    }
}