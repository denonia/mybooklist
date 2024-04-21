namespace MyBookList.Core.Entities;

public class BookComment
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string BookId { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ApplicationUser User { get; set; }
    public Book Book { get; set; }
}