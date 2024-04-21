using Microsoft.AspNetCore.Identity;

namespace MyBookList.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public IEnumerable<BookRating> BookRatings { get; set; }
    public IEnumerable<BookComment> BookComments { get; set; }
}