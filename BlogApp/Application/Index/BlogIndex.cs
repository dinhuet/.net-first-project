namespace BlogApp.Application.Index;

public class BlogIndex
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? PublishedAt { get; set; }
}