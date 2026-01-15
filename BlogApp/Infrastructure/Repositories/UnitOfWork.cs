using BlogApp.Application.IRepositories;
using BlogApp.Infrastructure.Persistence;

namespace BlogApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(AppDbContext db,  ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _db = db;
        _logger.LogInformation("UnitOfWork created: " + _db.GetHashCode());
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
}