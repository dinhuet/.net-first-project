using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;

namespace BlogApp.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public void AddCategory(Category category)
    {
        _db.Categories.Add(category);
        _db.SaveChanges();
    }

    public List<Category> GetCategories()
    {
        return _db.Categories.ToList();
    }

    public Category GetCategoryById(int categoryId)
    {
        return _db.Categories.FirstOrDefault(c => c.Id == categoryId);
    }
}