using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Enums;
using MyBookList.Models;
using MyBookList.Services;

namespace MyBookList.Pages.Books;

public class CommentViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RatingViewModel
{
    public string Username { get; set; }
    public int Rating { get; set; }
    public ReadingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookDetailsViewModel
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public int? CoverId { get; init; }
    public string ThumbnailUrl { get; set; }
    public IEnumerable<string> AuthorNames { get; init; }
    public IEnumerable<string> Subjects { get; init; }
    public IEnumerable<CommentViewModel> Comments { get; init; }
    public IEnumerable<RatingViewModel> LatestRatings { get; init; }
}

public class CommentModel
{
    [BindProperty]
    [Required(ErrorMessage = "Comment can't be empty.")]
    [MaxLength(1000)]
    public string Body { get; set; }
}

public class RatingModel
{
    [BindProperty]
    [Required]
    [Range(0, 10)]
    public int Rating { get; set; }

    [BindProperty] [Required] public ReadingStatus Status { get; set; }
}

public class Details : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ThumbnailService _thumbnailService;

    public Details(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
        ThumbnailService thumbnailService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _thumbnailService = thumbnailService;
    }

    public BookDetailsViewModel Book { get; set; }
    public CommentModel CommentModel { get; set; }
    public RatingModel RatingModel { get; set; }

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
                Subjects = x.Subjects.Select(s => s.Title),
                Comments = x.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Username = c.User.UserName!,
                        Body = c.Body,
                        CreatedAt = c.CreatedAt
                    }),
                LatestRatings = x.Ratings
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .Select(r => new RatingViewModel
                    {
                        Username = r.User.UserName!,
                        Rating = r.Rating,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt
                    })
            })
            .SingleOrDefaultAsync(x => x.Id == id);

        if (book is null)
            return NotFound();

        book.ThumbnailUrl = await _thumbnailService.GetBookThumbnailUrlAsync(book.CoverId);

        Book = book;

        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);
            var rating = await _dbContext.BookRatings
                .Where(r => r.BookId == Book.Id && r.UserId == userId)
                .FirstOrDefaultAsync();

            if (rating != null)
            {
                RatingModel = new RatingModel
                {
                    Rating = rating.Rating,
                    Status = rating.Status
                };
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCommentAsync(string id, CommentModel commentModel)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User);

        if (userId != null)
        {
            var comment = new BookComment
            {
                UserId = userId,
                BookId = id,
                Body = commentModel.Body
            };
            _dbContext.BookComments.Add(comment);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteCommentAsync(string id, Guid commentId)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User);

        var comment = await _dbContext.BookComments.FindAsync(commentId);
        if (comment != null && comment.UserId == userId)
        {
            _dbContext.BookComments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRatingAsync(string id, RatingModel ratingModel)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        var rating = await _dbContext.BookRatings
            .Where(r => r.BookId == id && r.UserId == userId)
            .FirstOrDefaultAsync();

        if (rating != null)
        {
            rating.Rating = ratingModel.Rating;
            rating.Status = ratingModel.Status;
            rating.CreatedAt = DateTime.Now;
        }
        else
        {
            var newRating = new BookRating
            {
                UserId = userId,
                BookId = id,
                Rating = ratingModel.Rating,
                Status = ratingModel.Status
            };

            _dbContext.BookRatings.Add(newRating);
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToPage(new { id });
    }
}