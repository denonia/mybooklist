using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBookList.Core.Entities;

namespace MyBookList.Infrastructure.Data.Config;

public class BookConfig : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder
            .HasMany<Subject>(x => x.Subjects)
            .WithMany(x => x.Books);

        builder
            .HasMany<BookRating>(x => x.Ratings)
            .WithOne(x => x.Book)
            .HasForeignKey(x => x.BookId);
        
        builder
            .HasMany<BookComment>(x => x.Comments)
            .WithOne(x => x.Book)
            .HasForeignKey(x => x.BookId);

        builder.HasIndex(x => x.Title);
    }
}