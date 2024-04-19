using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Services;

namespace MyBookList.Pages.Authors;

public class AuthorViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? PersonalName { get; set; }
    public int? BirthYear { get; set; }
    public int? DeathYear { get; set; }
}

public class Index : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public Index(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int PageSize { get; } = 20;
    
    public IEnumerable<AuthorViewModel> Authors { get; set; } = default!;

    public int AuthorsCount => Authors.Count();

    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        var authors = _dbContext.Authors.AsQueryable();

        if (!string.IsNullOrEmpty(SearchString))
            authors = authors.Where(x => x.Name.StartsWith(SearchString));
        
        Authors = await authors.Skip((PageIndex - 1) * PageSize).Take(PageSize)
            .Select(x => new AuthorViewModel
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