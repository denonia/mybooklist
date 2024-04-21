using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages.Books;

public class Index : PageModel
{
    private readonly IBookService _bookService;

    public Index(IBookService bookService)
    {
        _bookService = bookService;
    }

    public int PageSize { get; } = 20;

    public IEnumerable<BookResponse> Books { get; set; } = default!;

    public int BooksCount => Books.Count();

    [BindProperty(SupportsGet = true)] public bool HasDescription { get; set; } = true;
    [BindProperty(SupportsGet = true)] public bool HasCover { get; set; } = true;
    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    [BindProperty(SupportsGet = true)] public string? Author { get; set; }
    [BindProperty(SupportsGet = true)] public string? Subject { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        Books = await _bookService.SearchBooksAsync(PageIndex, PageSize, HasDescription, HasCover, SearchString,
            Author, Subject);
    }
}