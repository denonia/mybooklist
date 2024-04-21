using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Entities;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages.List;

[Authorize]
public class Index : PageModel
{
    private readonly IRatingService _ratingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public Index(IRatingService ratingService, UserManager<ApplicationUser> userManager)
    {
        _ratingService = ratingService;
        _userManager = userManager;
    }

    public IEnumerable<UserListRatingResponse> Ratings { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        Ratings = await _ratingService.GetUsersBookRatingsAsync(user.Id);

        return Page();
    }
}