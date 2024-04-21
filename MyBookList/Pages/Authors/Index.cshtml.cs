using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages.Authors;

public class Index : PageModel
{
    private readonly IAuthorService _authorService;

    public Index(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public int PageSize { get; } = 20;

    public IEnumerable<AuthorResponse> Authors { get; set; } = default!;

    public int AuthorsCount => Authors.Count();

    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        Authors = await _authorService.SearchAuthorsAsync(PageIndex, PageSize, SearchString);
    }
}