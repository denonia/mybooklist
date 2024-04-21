namespace MyBookList.Core.Responses;

public class CommentResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}