using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;
using MyBookList.Infrastructure.Data;

namespace MyBookList.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _dbContext;

    public SubjectService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<SubjectResponse>> SearchSubjectsAsync(int pageIndex, int pageSize, string? searchString)
    {
        var subjects = _dbContext.Subjects.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
            subjects = subjects.Where(x => x.Title.StartsWith(searchString));

        return await subjects.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            .Select(x => new SubjectResponse
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToListAsync();
    }
}