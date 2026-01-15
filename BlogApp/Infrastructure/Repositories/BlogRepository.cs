using System.Runtime.InteropServices.JavaScript;
using BlogApp.Application.Index;
using BlogApp.Application.IRepositories;
using BlogApp.Application.Mapper;
using BlogApp.Application.MiddleWare;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.Persistence;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly AppDbContext _db;
    private readonly ElasticsearchClient _esClient;

    public BlogRepository(AppDbContext dbContext, ElasticsearchClient esClient)
    {
        _esClient = esClient;
        _db = dbContext;
    }
    
    public async Task IndexBlogAsync(BlogIndex blog)
    {
        await _esClient.IndexAsync(blog, i => i.Index("blogs").Id(blog.Id));
    }
    
    /*public async Task<List<BlogIndex>> SearchBlogsAsync(string query)
    {
        var response = await _esClient.SearchAsync<BlogIndex>(s => s
            .Query(q => q.MultiMatch(m => m.Fields(f => f.Field(b => b.Title).Field(b => b.Content)).Query(query)))
        );
        return response.Hits.Select(h => h.Source!).ToList();
    }*/
    
    // index blogs
    public async Task AddAsync(Blog blog)
    {
        await _db.Blogs.AddAsync(blog);
        await _db.SaveChangesAsync();
        
        await _esClient.IndexAsync(blog.ToIndex(), i => i
            .Index("blogs")
            .Id(blog.Id)
        );
    }
    
    // Search blog theo title
    public async Task<List<BlogIndex>> SearchBlogsByTitleAsync(string titleQuery)
    {
        var response = await _esClient.SearchAsync<BlogIndex>(s => s
            .Index("blogs")
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Title)   // chỉ search theo title
                    .Query(titleQuery)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            // Xử lý lỗi
            throw new Exception("Elasticsearch search failed: ");
        }

        // Trả list BlogIndex
        return response.Hits.Select(h => h.Source!).ToList();
    }



    public IQueryable<Blog> GetPageBlog()
    {
        return _db.Blogs
            .Include(b => b.Author)
            .Include(b => b.BlogCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.BlogTags)
                .ThenInclude(bt => bt.Tag)
            .Include(b => b.Comments)
            .AsQueryable(); 
    }

    public async Task PublishBlog(int id)
    {
        var blog =  _db.Blogs.FirstOrDefault(b => b.Id == id);

        if (blog == null) throw new AppException(ErrorCode.BlogIsNotExist);
        
        blog.PublishedAt = DateTime.UtcNow;
        blog.Status = BlogStatus.Published;
         _db.Blogs.Update(blog);
        await _db.SaveChangesAsync();
    }

}