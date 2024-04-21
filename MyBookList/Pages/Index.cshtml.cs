using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages;

public class IndexModel : PageModel
{
    private readonly IBookService _bookService;
    private readonly IRatingService _ratingService;
    private readonly ICommentService _commentService;

    public IndexModel(IBookService bookService, IRatingService ratingService, ICommentService commentService)
    {
        _bookService = bookService;
        _ratingService = ratingService;
        _commentService = commentService;
    }

    public IEnumerable<BookResponse> RandomBooks { get; set; }
    public IEnumerable<LatestRatingResponse> LatestRatings { get; set; }
    public IEnumerable<LatestCommentResponse> LatestComments { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        RandomBooks = await _bookService.GetRandomBooksAsync(4);
        LatestRatings = await _ratingService.GetLatestRatingsAsync(5);
        LatestComments = await _commentService.GetLatestCommentsAsync(5);

        return Page();
    }
}