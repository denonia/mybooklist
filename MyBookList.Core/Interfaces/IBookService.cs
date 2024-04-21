using MyBookList.Core.Responses;

namespace MyBookList.Core.Interfaces;

public interface IBookService
{
    public Task<IEnumerable<BookResponse>> GetRandomBooksAsync(int amount);
        
    public Task<IEnumerable<BookResponse>> SearchBooksAsync(int pageIndex, int pageSize, bool hasDescription,
        bool hasCover,
        string? searchString,
        string? author, string? subject);

    public Task<BookDetailsResponse?> GetBookDetailsAsync(string id);
}