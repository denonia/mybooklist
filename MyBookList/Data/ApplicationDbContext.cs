using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBookList.Models;

namespace MyBookList.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorAlias> AuthorAliases { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Author>()
            .HasMany<AuthorAlias>(x => x.Aliases)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId);

        builder.Entity<Author>()
            .HasMany<Book>(x => x.Books)
            .WithMany(x => x.Authors);

        builder.Entity<Book>()
            .HasMany<Subject>(x => x.Subjects)
            .WithMany(x => x.Books);
        
        builder.Entity<Book>()
            .HasIndex(x => x.Title);
        
        builder.Entity<Author>()
            .HasIndex(x => x.Name);
        
        builder.Entity<Subject>()
            .HasIndex(x => x.Title);

        base.OnModelCreating(builder);
    }
}