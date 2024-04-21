namespace MyBookList.Core.Responses;

public class BookDetailsResponse
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public string ThumbnailUrl { get; set; }
    public IEnumerable<string> AuthorNames { get; init; }
    public IEnumerable<string> Subjects { get; init; }
    public IEnumerable<CommentResponse> Comments { get; init; }
    public IEnumerable<RatingResponse> LatestRatings { get; init; }
}