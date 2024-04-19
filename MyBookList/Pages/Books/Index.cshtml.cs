using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Services;

namespace MyBookList.Pages.Books;

public class BookViewModel
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public int? CoverId { get; init; }
    public string ThumbnailUrl { get; set; }
    public IEnumerable<string> AuthorNames { get; init; }

    public string AuthorsString => string.Join(",", AuthorNames);
}

public class Index : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ThumbnailService _thumbnailService;

    public Index(ApplicationDbContext dbContext, ThumbnailService thumbnailService)
    {
        _dbContext = dbContext;
        _thumbnailService = thumbnailService;
    }

    public int PageSize { get; } = 20;
    
    public IEnumerable<BookViewModel> Books { get; set; } = default!;

    public int BooksCount => Books.Count();

    [BindProperty(SupportsGet = true)] public bool HasDescription { get; set; } = true;
    [BindProperty(SupportsGet = true)] public bool HasCover { get; set; } = true;
    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    [BindProperty(SupportsGet = true)] public string? Author { get; set; }
    [BindProperty(SupportsGet = true)] public string? Subject { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        var books = _dbContext.Books.AsQueryable();

        if (!string.IsNullOrEmpty(SearchString))
            books = books.Where(x => x.Title.StartsWith(SearchString));
        
        if (!string.IsNullOrEmpty(Author))
        {
            var authors = _dbContext.Authors.Where(x => x.Name == Author);
            books = books.Where(x => x.Authors.Intersect(authors).Any());
        }
        
        if (!string.IsNullOrEmpty(Subject))
        {
            var subject = _dbContext.Subjects.FirstOrDefault(x => x.Title == Subject);
            books = books.Where(x => x.Subjects.Contains(subject));
        }

        if (HasDescription)
            books = books.Where(x => x.Description != null);

        if (HasCover)
            books = books.Where(x => x.CoverId != null);

        Books = await books.Skip((PageIndex - 1) * PageSize).Take(PageSize)
            .Select(x => new BookViewModel
            {
                Id = x.Id,
                Title = x.Title.Truncate(100),
                Description = x.Description.Truncate(200),
                CoverId = x.CoverId,
                AuthorNames = x.Authors.Select(a => a.Name),
            })
            .ToListAsync();

        foreach (var book in Books)
            book.ThumbnailUrl = await _thumbnailService.GetBookThumbnailUrlAsync(book.CoverId);
    }
}