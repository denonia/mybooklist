using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Entities;
using MyBookList.Infrastructure.Data.Config;

namespace MyBookList.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorAlias> AuthorAliases { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<BookRating> BookRatings { get; set; }
    public DbSet<BookComment> BookComments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AuthorConfig());
        builder.ApplyConfiguration(new BookConfig());
        builder.ApplyConfiguration(new SubjectConfig());
        builder.ApplyConfiguration(new ApplicationUserConfig());
        
        base.OnModelCreating(builder);
    }
}