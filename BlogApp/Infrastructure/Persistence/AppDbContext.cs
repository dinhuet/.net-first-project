using BlogApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<BlogTag> BlogTags => Set<BlogTag>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Otp> Otps { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BlogCategory PK
        modelBuilder.Entity<BlogCategory>()
            .HasKey(bc => new { bc.BlogId, bc.CategoryId });

        modelBuilder.Entity<BlogCategory>()
            .HasOne(bc => bc.Blog)
            .WithMany(b => b.BlogCategories)
            .HasForeignKey(bc => bc.BlogId);

        modelBuilder.Entity<BlogCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BlogCategories)
            .HasForeignKey(bc => bc.CategoryId);

        // BlogTag PK
        modelBuilder.Entity<BlogTag>()
            .HasKey(bt => new { bt.BlogId, bt.TagId });

        modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.Blog)
            .WithMany(b => b.BlogTags)
            .HasForeignKey(bt => bt.BlogId);

        modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.Tag)
            .WithMany(t => t.BlogTags)
            .HasForeignKey(bt => bt.TagId);

        // Like PK
        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.BlogId, l.UserId });

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Blog)
            .WithMany(b => b.Likes)
            .HasForeignKey(l => l.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment self-ref
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // User-Blog
        modelBuilder.Entity<Blog>()
            .HasOne(b => b.Author)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment-Blog & Comment-User
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Blog)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .HasOne(o => o.User)
            .WithMany(o => o.RefreshTokens)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Otp
        modelBuilder.Entity<Otp>()
            .HasOne(o => o.User)
            .WithMany(o => o.Otps)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}