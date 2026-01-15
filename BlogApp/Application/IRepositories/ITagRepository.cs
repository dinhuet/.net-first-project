using BlogApp.Domain.Models;

namespace BlogApp.Application.IRepositories;

public interface ITagRepository
{
    List<Tag> GetExistingTask(List<string> normalizedTagNames);
    void AddMultipleTag(List<Tag> tag);
}