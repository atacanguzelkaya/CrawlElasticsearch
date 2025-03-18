namespace CrawlElasticsearch.Models
{
    public class Articles
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Url { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}