using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Services;

namespace MyBookList.Pages.Books;

public class BookDetailsViewModel
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public int? CoverId { get; init; }
    public string ThumbnailUrl { get; set; }
    public IEnumerable<string> AuthorNames { get; init; }
    public IEnumerable<string> Subjects { get; init; }

    public string AuthorsString => string.Join(",", AuthorNames);
}

public class Details : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ThumbnailService _thumbnailService;

    public Details(ApplicationDbContext dbContext, ThumbnailService thumbnailService)
    {
        _dbContext = dbContext;
        _thumbnailService = thumbnailService;
    }

    public BookDetailsViewModel Book { get; set; }

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        var book = await _dbContext.Books
            .Select(x => new BookDetailsViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CoverId = x.CoverId,
                AuthorNames = x.Authors.Select(a => a.Name),
                Subjects = x.Subjects.Select(s => s.Title)
            })
            .SingleOrDefaultAsync(x => x.Id == id);

        if (book is null)
            return NotFound();

        book.ThumbnailUrl = await _thumbnailService.GetBookThumbnailUrlAsync(book.CoverId);

        Book = book;
        

        return Page();
    }
}