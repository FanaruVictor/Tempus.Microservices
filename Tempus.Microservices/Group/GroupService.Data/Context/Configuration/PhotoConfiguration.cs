using GroupService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupService.Data.Context.Configuration;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.Property(x => x.PublicId).IsRequired();
        builder.Property(x => x.Url).IsRequired();
        builder
            .HasOne(u => u.Group)
            .WithOne(p => p.GroupPhoto)
            .HasForeignKey<Photo>(p => p.GroupId);
    }
}