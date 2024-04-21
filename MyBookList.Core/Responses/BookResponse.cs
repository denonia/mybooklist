namespace MyBookList.Core.Responses;

public class BookResponse
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public string ThumbnailUrl { get; set; }
    public IEnumerable<string> AuthorNames { get; init; }

    public string AuthorsString => string.Join(",", AuthorNames);
}