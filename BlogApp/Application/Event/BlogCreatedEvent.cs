using BlogApp.Domain.Enums;

namespace BlogApp.Application.Event;

public class BlogCreatedEvent
{
    public int BlogId { get; set; }
    public int AuthorId { get; set; }
    public BlogStatus Status { get; set; }
}
