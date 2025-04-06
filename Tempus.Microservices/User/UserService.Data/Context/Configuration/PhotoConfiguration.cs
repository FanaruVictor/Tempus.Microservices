using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Core.Entities;

namespace UserService.Data.Context.Configuration;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.Property(x => x.PublicId).IsRequired();
        builder.Property(x => x.Url).IsRequired();
        builder
            .HasOne(u => u.User)
            .WithOne(p => p.Photo)
            .HasForeignKey<Photo>(p => p.UserId);
    }
}

