using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Entities;
using MyBookList.Core.Interfaces;
using MyBookList.Core.Responses;
using MyBookList.Infrastructure.Data;

namespace MyBookList.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _dbContext;

    public CommentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<LatestCommentResponse>> GetLatestCommentsAsync(int amount)
    {
        return await _dbContext.BookComments
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(c => new LatestCommentResponse
            {
                Username = c.User.UserName!,
                BookId = c.BookId,
                BookTitle = c.Book.Title,
                CreatedAt = c.CreatedAt,
                Body = c.Body
            }).ToListAsync();
    }

    public async Task PostCommentAsync(string bookId, string userId, string body)
    {
        var comment = new BookComment
        {
            UserId = userId,
            BookId = bookId,
            Body = body
        };
        _dbContext.BookComments.Add(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> CheckCommentOwnerAsync(Guid commentId, string userId)
    {
        var comment = await _dbContext.BookComments.FindAsync(commentId);
        return comment != null && comment.UserId == userId;
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        var comment = await _dbContext.BookComments.FindAsync(commentId);
        _dbContext.BookComments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}