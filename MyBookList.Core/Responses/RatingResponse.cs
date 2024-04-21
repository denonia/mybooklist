using MyBookList.Core.Enums;

namespace MyBookList.Core.Responses;

public class RatingResponse
{
    public string Username { get; set; }
    public int Rating { get; set; }
    public ReadingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}