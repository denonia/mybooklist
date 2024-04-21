using MyBookList.Core.Responses;

namespace MyBookList.Core.Interfaces;

public interface IAuthorService
{
    public Task<IEnumerable<AuthorResponse>> SearchAuthorsAsync(int pageIndex, int pageSize,
        string? searchString);
}