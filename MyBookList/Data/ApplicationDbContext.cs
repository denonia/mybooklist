using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBookList.Data.EntityTypeConfigurations;
using MyBookList.Models;

namespace MyBookList.Data;

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
        builder.ApplyConfiguration(new AuthorConfiguration());
        builder.ApplyConfiguration(new BookConfiguration());
        builder.ApplyConfiguration(new SubjectConfiguration());
        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        
        base.OnModelCreating(builder);
    }
}