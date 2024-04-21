namespace MyBookList.Services;

public interface IThumbnailService
{
    public string GetBookThumbnailUrl(int? bookId);
}