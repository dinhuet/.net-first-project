using BlogApp.Application.Index;

namespace BlogApp.Infrastructure.ExternalServices.Impl;

public interface IBlogSuggestService
{
    Task<List<string>> SuggestAsync(string keyword);
    Task<List<BlogIndex>> SearchAsync(string keyword);
}
