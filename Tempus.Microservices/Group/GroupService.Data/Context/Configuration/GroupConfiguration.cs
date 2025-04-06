using GroupService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupService.Data.Context.Configuration;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.OwnerId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(u => u.GroupPhoto)
            .WithOne(p => p.Group)
            .HasForeignKey<Photo>(p => p.GroupId);
    }

    private Guid? GetGuid(string s)
    {
        if(Guid.TryParse(s, out var guid))
        {
            return guid;
        }

        return null;
    }
}