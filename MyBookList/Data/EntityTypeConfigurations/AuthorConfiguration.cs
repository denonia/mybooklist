using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBookList.Models;

namespace MyBookList.Data.EntityTypeConfigurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder
            .HasMany<AuthorAlias>(x => x.Aliases)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId);

        builder
            .HasMany<Book>(x => x.Books)
            .WithMany(x => x.Authors);

        builder.HasIndex(x => x.Name);
    }
}