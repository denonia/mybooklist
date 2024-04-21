namespace MyBookList.Core.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    
    public IEnumerable<Book> Books { get; set; }
}