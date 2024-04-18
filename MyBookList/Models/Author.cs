namespace MyBookList.Models;

public class Author
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? PersonalName { get; set; }
    public int? BirthYear { get; set; }
    public int? DeathYear { get; set; }
    
    public IEnumerable<Book> Books { get; set; }
    public IEnumerable<AuthorAlias> Aliases { get; set; }
}