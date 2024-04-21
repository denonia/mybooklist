using MyBookList.Core.Responses;

namespace MyBookList.Core.Interfaces;

public interface ISubjectService
{
    public Task<IEnumerable<SubjectResponse>> SearchSubjectsAsync(int pageIndex, int pageSize,
        string? searchString);
}