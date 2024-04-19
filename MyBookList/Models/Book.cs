namespace MyBookList.Models;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int? CoverId { get; set; }
    
    public IEnumerable<Author> Authors { get; set; }
    public IEnumerable<Subject> Subjects { get; set; }
    public IEnumerable<BookRating> Ratings { get; set; }
    public IEnumerable<BookComment> Comments { get; set; }
}