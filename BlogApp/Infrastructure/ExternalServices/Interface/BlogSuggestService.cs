using BlogApp.Application.Index;
using BlogApp.Infrastructure.ExternalServices.Impl;
using Elastic.Clients.Elasticsearch;

namespace BlogApp.Infrastructure.ExternalServices.Interface;

public class BlogSuggestService : IBlogSuggestService
{
    private readonly ElasticsearchClient _client;

    public BlogSuggestService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<List<string>> SuggestAsync(string keyword)
    {
        var res = await _client.SearchAsync<BlogIndex>(s => s
            .Indices("blogs")
            .Size(5)
            .SourceIncludes(f => f.Title)
            .Query(q => q
                .Match(m => m
                    .Field("title.autocomplete")
                    .Query(keyword)
                )
            )
        );

        return res.Documents
            .Select(d => d.Title)
            .Distinct()
            .ToList();
    }
    
    public async Task<List<BlogIndex>> SearchAsync(string keyword)
    {
        var res = await _client.SearchAsync<BlogIndex>(s => s
            .Indices("blogs")
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(keyword)
                    .Fields(f => f.Title, f => f.Content)
                    .Fuzziness(2)     // bật fuzzy
                    .PrefixLength(2)               // 2 ký tự đầu phải đúng
                    .MaxExpansions(50)              // giowis hanj số term sinh ra
                )
            )
            .Sort(s => s
                .Score(sc => sc.Order(SortOrder.Desc))
                .Field(p => p.PublishedAt, so => so.Order(SortOrder.Desc))
            )
        );

        // filter
        /*.Filter(f => f
                .Term(t => t.Field("categoryId").Value(3))      // exact match
                .Range(r => r.Field("totalView").Gte(1000))     // >= 1000
                .DateRange(dr => dr.Field("publishedAt").Gte("2024-01-01")) // ngày
                .Bool(b => b.Must(...))                         // nested filter
            )*/

        return res.Documents.ToList();
    }

}