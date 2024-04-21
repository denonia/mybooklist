namespace MyBookList.Core.Responses;

public class AuthorResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? PersonalName { get; set; }
    public int? BirthYear { get; set; }
    public int? DeathYear { get; set; }
}