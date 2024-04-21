namespace MyBookList.Core.Responses;

public class LatestCommentResponse
{
    public string Username { get; set; }
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}