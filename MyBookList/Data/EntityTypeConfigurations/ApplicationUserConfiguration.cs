using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBookList.Models;

namespace MyBookList.Data.EntityTypeConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .HasMany<BookRating>(x => x.BookRatings)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder
            .HasMany<BookComment>(x => x.BookComments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}