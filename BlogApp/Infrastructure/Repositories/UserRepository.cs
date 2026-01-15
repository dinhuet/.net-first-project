using AutoMapper;
using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public UserRepository(AppDbContext db, IMapper mapper)
    {
        _mapper = mapper;
        _db = db;
    }

    public void Register(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public User GetUserByEmail(string email)
    {
        return _db.Users.FirstOrDefault(e => e.Email == email);
    }

    public void UpdateUser(User user)
    {
        _db.Users.Update(user);
        _db.SaveChanges();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<int> GetUserIdByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .FirstAsync();
    }

    
}