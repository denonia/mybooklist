using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Entities;
using MyBookList.Core.Enums;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages.Books;

public class Details : PageModel
{
    private readonly IBookService _bookDetailsService;
    private readonly IRatingService _ratingService;
    private readonly ICommentService _commentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public Details(IBookService bookDetailsService, IRatingService ratingService, ICommentService commentService,
        UserManager<ApplicationUser> userManager)
    {
        _bookDetailsService = bookDetailsService;
        _ratingService = ratingService;
        _commentService = commentService;
        _userManager = userManager;
    }

    public BookDetailsResponse Book { get; set; }
    public CommentModel Comment { get; set; }
    public RatingModel Rating { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var book = await _bookDetailsService.GetBookDetailsAsync(id);

        if (book is null)
            return NotFound();

        Book = book;

        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);
            var rating = await _ratingService.GetUsersBookRatingAsync(Book.Id, userId);
            if (rating != null)
                Rating = new RatingModel
                {
                    Rating = rating.Rating,
                    Status = rating.Status
                };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCommentAsync(string id, CommentModel comment)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        if (userId != null) await _commentService.PostCommentAsync(id, userId, comment.Body);

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
        if (await _commentService.CheckCommentOwnerAsync(commentId, userId))
            await _commentService.DeleteCommentAsync(commentId);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRatingAsync(string id, RatingModel rating)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        await _ratingService.UpdateRatingAsync(id, userId, rating.Rating, rating.Status);

        return RedirectToPage(new { id });
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
}