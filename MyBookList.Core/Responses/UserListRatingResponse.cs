using MyBookList.Core.Enums;

namespace MyBookList.Core.Responses;

public class UserListRatingResponse
{
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public ReadingStatus Status { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}