using MyBookList.Core.Responses;

namespace MyBookList.Core.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<LatestCommentResponse>> GetLatestCommentsAsync(int amount);
    Task PostCommentAsync(string bookId, string userId, string body);
    Task<bool> CheckCommentOwnerAsync(Guid commentId, string userId);
    Task DeleteCommentAsync(Guid commentId);
}