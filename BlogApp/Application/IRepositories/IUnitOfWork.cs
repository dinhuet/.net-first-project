namespace BlogApp.Application.IRepositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}