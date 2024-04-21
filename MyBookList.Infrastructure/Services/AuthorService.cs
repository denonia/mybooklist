using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;
using MyBookList.Infrastructure.Data;

namespace MyBookList.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly ApplicationDbContext _dbContext;

    public AuthorService(ApplicationDbContext _dbContext)
    {
        this._dbContext = _dbContext;
    }

    public async Task<IEnumerable<AuthorResponse>> SearchAuthorsAsync(int pageIndex, int pageSize,
        string? searchString)
    {
        var authors = _dbContext.Authors.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
            authors = authors.Where(x => x.Name.StartsWith(searchString));
        else
            authors = authors.Where(x => (x.BirthYear != null) & (x.DeathYear != null));

        return await authors.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            .Select(x => new AuthorResponse
            {
                Id = x.Id,
                Name = x.Name,
                PersonalName = x.PersonalName,
                BirthYear = x.BirthYear,
                DeathYear = x.DeathYear
            })
            .ToListAsync();
    }
}