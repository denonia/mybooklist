using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Enums;
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

public class RatingViewModel
{
    public string Username { get; set; }
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public int Rating { get; set; }
    public ReadingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CommentViewModel
{
    public string Username { get; set; }
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
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
    public IEnumerable<RatingViewModel> LatestRatings { get; set; }
    public IEnumerable<CommentViewModel> LatestComments { get; set; }

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

        LatestRatings = await _dbContext.BookRatings
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new RatingViewModel
            {
                Username = r.User.UserName!,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                CreatedAt = r.CreatedAt,
                Rating = r.Rating,
                Status = r.Status
            }).ToListAsync();

        LatestComments = await _dbContext.BookComments
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(c => new CommentViewModel
            {
                Username = c.User.UserName!,
                BookId = c.BookId,
                BookTitle = c.Book.Title,
                CreatedAt = c.CreatedAt,
                Body = c.Body
            }).ToListAsync();

        foreach (var book in RandomBooks)
            book.ThumbnailUrl = await _thumbnailService.GetBookThumbnailUrlAsync(book.CoverId);

        return Page();
    }
}