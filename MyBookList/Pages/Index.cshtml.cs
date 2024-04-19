using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Models;
using MyBookList.Services;

namespace MyBookList.Pages;

public class BookViewModel
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public int? CoverId { get; init; }
    public string ThumbnailUrl { get; set; }
}

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ThumbnailService _thumbnailService;

    public IndexModel(ApplicationDbContext dbContext, ThumbnailService thumbnailService)
    {
        _dbContext = dbContext;
        _thumbnailService = thumbnailService;
    }

    public IEnumerable<BookViewModel> RandomBooks { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // TODO: determine max amount
        var skipAmount = Random.Shared.Next(0, 1000);
        
        RandomBooks = await _dbContext.Books
            .Where(x => x.CoverId != null && x.Description != null)
            .Skip(skipAmount)
            .Take(4)
            .Select(x => new BookViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CoverId = x.CoverId
            }).ToListAsync();

        foreach (var book in RandomBooks)
            book.ThumbnailUrl = await _thumbnailService.GetBookThumbnailUrlAsync(book.CoverId);

        return Page();
    }
}