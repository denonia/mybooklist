using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;

namespace MyBookList.Pages.Subjects;

public class Index : PageModel
{
    private readonly ISubjectService _subjectService;

    public Index(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public int PageSize { get; } = 200;

    public IEnumerable<SubjectResponse> Subjects { get; set; } = default!;

    public int SubjectsCount => Subjects.Count();

    [BindProperty(SupportsGet = true)] public string? SearchString { get; set; }
    public int PageIndex { get; set; } = 1;

    public async Task OnGetAsync(int pageIndex = 1)
    {
        if (pageIndex >= 1)
            PageIndex = pageIndex;

        Subjects = await _subjectService.SearchSubjectsAsync(PageIndex, PageSize, SearchString);
    }
}