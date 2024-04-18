using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;

namespace MyBookList.Pages.Books;

public class BookViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int? CoverId { get; set; }
    public IEnumerable<string> AuthorNames { get; set; }

    public string Authors => string.Join(",", AuthorNames);
}

public class Index : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public Index(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<BookViewModel> Books { get; set; } = default!;

    public int BooksCount => Books.Count();

    [BindProperty(SupportsGet = true)] public bool HasDescription { get; set; } = true;
    [BindProperty(SupportsGet = true)] public bool HasCover { get; set; } = true;
    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }

    public async Task OnGetAsync()
    {
        var books = _dbContext.Books.AsQueryable();

        if (!string.IsNullOrEmpty(SearchString))
            books = books.Where(x => x.Title.StartsWith(SearchString));

        if (HasDescription)
            books = books.Where(x => x.Description != null);

        if (HasCover)
            books = books.Where(x => x.CoverId != null);

        Books = await books.Take(20)
            .Select(x => new BookViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CoverId = x.CoverId,
                AuthorNames = x.Authors.Select(a => a.Name),
            })
            .ToListAsync();
    }
}