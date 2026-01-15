using BlogApp.Application.IRepositories;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly AppDbContext _db;

    public OtpRepository(AppDbContext db)
    {
        _db = db;
    }    
    
    public void AddOtp(Otp otp)
    {
        _db.Otps.Add(otp);
        _db.SaveChanges();
    }

    public Otp GetValidOtp(string email)
    {
        return _db.Otps.Include(o => o.User)
            .FirstOrDefault(o => o.User.Email == email && !o.IsUsed && o.ExpiredAt > DateTime.UtcNow);
    }

    public void UpdateOtp(Otp otp)
    {
        _db.Otps.Update(otp);
        _db.SaveChanges();
    }
}