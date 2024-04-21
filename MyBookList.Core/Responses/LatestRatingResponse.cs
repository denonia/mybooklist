using MyBookList.Core.Enums;

namespace MyBookList.Core.Responses;

public class LatestRatingResponse
{
    public string Username { get; set; }
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public int Rating { get; set; }
    public ReadingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}