using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Entities;
using MyBookList.Core.Enums;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;
using MyBookList.Infrastructure.Data;

namespace MyBookList.Infrastructure.Services;

public class RatingService : IRatingService
{
    private readonly ApplicationDbContext _dbContext;

    public RatingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BookRating?> GetUsersBookRatingAsync(string bookId, string userId)
    {
        return _dbContext.BookRatings
            .Where(r => r.BookId == bookId && r.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserListRatingResponse>> GetUsersBookRatingsAsync(string userId)
    {
        return await _dbContext.BookRatings
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new UserListRatingResponse
            {
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                Status = r.Status,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LatestRatingResponse>> GetLatestRatingsAsync(int amount)
    {
       return await _dbContext.BookRatings
            .OrderByDescending(r => r.CreatedAt)
            .Take(amount)
            .Select(r => new LatestRatingResponse
            {
                Username = r.User.UserName!,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                CreatedAt = r.CreatedAt,
                Rating = r.Rating,
                Status = r.Status
            }).ToListAsync();
    }

    public async Task UpdateRatingAsync(string bookId, string userId, int rating, ReadingStatus status)
    {
        var bookRating = await _dbContext.BookRatings
            .Where(r => r.BookId == bookId && r.UserId == userId)
            .FirstOrDefaultAsync();

        var newRating = new BookRating
        {
            UserId = userId,
            BookId = bookId,
            Rating = rating,
            Status = status
        };

        if (bookRating == null)
            _dbContext.BookRatings.Add(newRating);
        else
        {
            newRating.Id = bookRating.Id;
            _dbContext.Entry(bookRating).CurrentValues.SetValues(newRating);
        }

        await _dbContext.SaveChangesAsync();
    }
}