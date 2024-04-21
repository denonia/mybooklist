namespace MyBookList.Core.Entities;

public class AuthorAlias
{
    public Guid Id { get; set; }
    public string AuthorId { get; set; }
    public string Name { get; set; }
    
    public Author Author { get; set; }
}