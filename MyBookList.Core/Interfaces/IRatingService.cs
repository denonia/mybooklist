using MyBookList.Core.Entities;
using MyBookList.Core.Enums;
using MyBookList.Core.Responses;

namespace MyBookList.Core.Interfaces;

public interface IRatingService
{
    Task<BookRating?> GetUsersBookRatingAsync(string bookId, string userId);
    Task<IEnumerable<UserListRatingResponse>> GetUsersBookRatingsAsync(string userId);
    Task<IEnumerable<LatestRatingResponse>> GetLatestRatingsAsync(int amount);
    Task UpdateRatingAsync(string bookId, string userId, int rating, ReadingStatus status);
}