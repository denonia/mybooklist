using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data;
using MyBookList.Enums;
using MyBookList.Models;

namespace MyBookList.Pages.List;

public class RatingViewModel
{
    public string BookId { get; set; }
    public string BookTitle { get; set; }
    public ReadingStatus Status { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

[Authorize]
public class Index : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public Index(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public IEnumerable<RatingViewModel> Ratings { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        Ratings = await _dbContext.BookRatings
            .Where(r => r.UserId == user.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RatingViewModel
            {
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                Status = r.Status,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Page();
    }
}