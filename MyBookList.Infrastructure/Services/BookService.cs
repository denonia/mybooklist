using Humanizer;
using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;
using MyBookList.Infrastructure.Data;
using MyBookList.Services;

namespace MyBookList.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IThumbnailService _thumbnailService;

    public BookService(ApplicationDbContext dbContext, IThumbnailService thumbnailService)
    {
        _dbContext = dbContext;
        _thumbnailService = thumbnailService;
    }

    public async Task<IEnumerable<BookResponse>> GetRandomBooksAsync(int amount)
    {
        // TODO: determine max amount
        var skipAmount = Random.Shared.Next(0, 1000);

        return await _dbContext.Books
            .Where(x => x.CoverId != null && x.Description != null)
            .Skip(skipAmount)
            .Take(amount)
            .Select(x => new BookResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ThumbnailUrl = _thumbnailService.GetBookThumbnailUrl(x.CoverId),
                AuthorNames = x.Authors.Select(a => a.Name)
            }).ToListAsync();
    }

    public async Task<IEnumerable<BookResponse>> SearchBooksAsync(int pageIndex, int pageSize, bool hasDescription,
        bool hasCover,
        string? searchString,
        string? author, string? subject)
    {
        var books = _dbContext.Books.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
            books = books.Where(x => x.Title.StartsWith(searchString));

        if (!string.IsNullOrEmpty(author))
        {
            var authors = _dbContext.Authors.Where(x => x.Name == author);
            books = books.Where(x => x.Authors.Intersect(authors).Any());
        }

        if (!string.IsNullOrEmpty(subject))
        {
            var subjectEntry = _dbContext.Subjects.FirstOrDefault(x => x.Title == subject);
            books = books.Where(x => x.Subjects.Contains(subjectEntry));
        }

        if (hasDescription)
            books = books.Where(x => x.Description != null);

        if (hasCover)
            books = books.Where(x => x.CoverId != null);

        return await books.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            .Select(x => new BookResponse
            {
                Id = x.Id,
                Title = x.Title.Truncate(100),
                Description = x.Description.Truncate(200),
                ThumbnailUrl = _thumbnailService.GetBookThumbnailUrl(x.CoverId),
                AuthorNames = x.Authors.Select(a => a.Name)
            })
            .ToListAsync();
    }

    public Task<BookDetailsResponse?> GetBookDetailsAsync(string id)
    {
        return _dbContext.Books
            .Select(x => new BookDetailsResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ThumbnailUrl = _thumbnailService.GetBookThumbnailUrl(x.CoverId),
                AuthorNames = x.Authors.Select(a => a.Name),
                Subjects = x.Subjects.Select(s => s.Title),
                Comments = x.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentResponse
                    {
                        Id = c.Id,
                        Username = c.User.UserName!,
                        Body = c.Body,
                        CreatedAt = c.CreatedAt
                    }),
                LatestRatings = x.Ratings
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .Select(r => new RatingResponse
                    {
                        Username = r.User.UserName!,
                        Rating = r.Rating,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt
                    })
            })
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}