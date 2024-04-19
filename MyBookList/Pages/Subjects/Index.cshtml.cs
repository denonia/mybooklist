using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Pages.Books;

namespace MyBookList.Pages.Subjects;

public class SubjectViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; }
}

public class Index : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public Index(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int PageSize { get; } = 200;

    public IEnumerable<SubjectViewModel> Subjects { get; set; } = default!;

    public int SubjectsCount => Subjects.Count();

    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        var subjects = _dbContext.Subjects.AsQueryable();

        if (!string.IsNullOrEmpty(SearchString))
            subjects = subjects.Where(x => x.Title.StartsWith(SearchString));

        Subjects = await subjects.Skip((PageIndex - 1) * PageSize).Take(PageSize)
            .Select(x => new SubjectViewModel
            {
                Id = x.Id,
                Title = x.Title,
            })
            .ToListAsync();
    }
}