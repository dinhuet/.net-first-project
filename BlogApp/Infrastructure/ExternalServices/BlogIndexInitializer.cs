using BlogApp.Application.Index;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;

namespace BlogApp.Infrastructure.ExternalServices;

public class BlogIndexInitializer
{
    private readonly ElasticsearchClient _client;

    public BlogIndexInitializer(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task InitAsync()
    {
        var exists = await _client.Indices.ExistsAsync("blogs");
        if (exists.Exists) return;

        await _client.Indices.CreateAsync("blogs", c => c
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("autocomplete", ca => ca
                            .Tokenizer("autocomplete_tokenizer")
                            .Filter("lowercase")
                        )
                    )
                    .Tokenizers(t => t
                        .EdgeNGram("autocomplete_tokenizer", e => e
                            .MinGram(2)
                            .MaxGram(20)
                            .TokenChars(TokenChar.Letter, TokenChar.Digit)
                        )
                    )
                )
            )
            .Mappings(m => m
                .Properties<BlogIndex>(p => p
                    .IntegerNumber(x => x.Id)

                    .Text(x => x.Title, t => t
                        .Fields(f => f
                            .Keyword("keyword")
                            .Text("autocomplete", tt => tt
                                .Analyzer("autocomplete")
                                .SearchAnalyzer("standard")
                            )
                        )
                    )

                    .Text(x => x.Content)
                    .Date(x => x.PublishedAt)
                )
            )
        );
    }
}
