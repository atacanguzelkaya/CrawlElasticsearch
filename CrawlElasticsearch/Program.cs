using CrawlElasticsearch.Repository;
using CrawlElasticsearch.Services.ElasticsearchServices;
using CrawlElasticsearch.Services.WebCrawlerServices;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Elasticsearch Configration
var esConfig = builder.Configuration.GetSection("ElasticSearch");

builder.Services.AddSingleton<IElasticClient>(serviceProvider =>
    new ElasticClient(new ConnectionSettings(new Uri(esConfig["Uri"])).DefaultIndex(esConfig["Index"]))
);

//Repository provider
builder.Services.AddSingleton<IElasticsearchRepository>(serviceProvider =>
{
    var elasticClient = serviceProvider.GetRequiredService<IElasticClient>();
    return new ElasticsearchRepository(elasticClient, esConfig["Index"]);
});

builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();
builder.Services.AddSingleton<IWebCrawlerService, WebCrawlerService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseStatusCodePagesWithRedirects("/");
app.MapRazorPages();

app.Run();