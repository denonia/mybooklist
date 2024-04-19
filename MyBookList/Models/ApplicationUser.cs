using Microsoft.AspNetCore.Identity;

namespace MyBookList.Models;

public class ApplicationUser : IdentityUser
{
    public IEnumerable<BookRating> BookRatings { get; set; }
    public IEnumerable<BookComment> BookComments { get; set; }
}