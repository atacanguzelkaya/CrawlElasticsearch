using CrawlElasticsearch.Models;
using HtmlAgilityPack;
using System.Net;

namespace CrawlElasticsearch.Services.WebCrawlerServices
{
    public class WebCrawlerService : IWebCrawlerService
    {
        private readonly HttpClient _httpClient;

        public WebCrawlerService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Articles>> CrawlPageAsync(string url, string searchTerm)
        {
            var htmlDocument = await GetHtmlDocumentAsync(url);
            return ExtractData(htmlDocument, searchTerm, url);
        }

        private async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
        {
            var htmlContent = await _httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);
            return htmlDocument;
        }

        private List<Articles> ExtractData(HtmlDocument document, string searchTerm, string baseUrl)
        {
            var webPages = new List<Articles>();
            var existingUrls = new HashSet<string>();

            var bodyNode = document.DocumentNode.SelectSingleNode("//body");
            if (bodyNode != null)
            {
                // Remove authors
                var authorSwiperNode = bodyNode.SelectSingleNode(".//section[@class='mb-4 author-swiper']");
                if (authorSwiperNode != null)
                {
                    authorSwiperNode.Remove();
                }
                //Remove Footer
                var footerNode = bodyNode.SelectSingleNode(".//footer");
                if (footerNode != null)
                {
                    footerNode.Remove();
                }

                var links = bodyNode.SelectNodes(".//a");
                if (links != null)
                {
                    foreach (var linkNode in links)
                    {
                        var link = linkNode.GetAttributeValue("href", string.Empty);
                        var fullLink = string.IsNullOrEmpty(link) ? string.Empty : new Uri(new Uri(baseUrl), link).ToString();

                        var imgNode = linkNode.SelectSingleNode(".//img");
                        if (imgNode != null)
                        {
                            var imageUrl = imgNode.GetAttributeValue("src", string.Empty);
                            var altText = imgNode.GetAttributeValue("alt", string.Empty);
                            altText = WebUtility.HtmlDecode(altText);

                            if (!string.IsNullOrEmpty(altText) && !string.IsNullOrEmpty(fullLink) && !string.IsNullOrEmpty(imageUrl) && !existingUrls.Contains(fullLink))
                            {
                                webPages.Add(new Articles
                                {
                                    Title = altText,
                                    Url = fullLink,
                                    ImageUrl = imageUrl,
                                });
                                existingUrls.Add(fullLink);
                            }
                        }
                    }
                }
            }

            return webPages;
        }
    }
}
