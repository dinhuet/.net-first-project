using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;

namespace BlogApp.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;
    public TagRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public List<Tag> GetExistingTask(List<string> normalizedTagNames)
    {
        return _db.Tags
            .Where(t => normalizedTagNames.Contains(t.Name.ToLower()))
            .ToList<Tag>();
    }

    public void AddMultipleTag(List<Tag> tag)
    {
        _db.Tags.AddRange(tag);
        _db.SaveChanges();
    }

}