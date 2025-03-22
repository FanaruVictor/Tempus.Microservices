using GroupService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupService.Data.Context.Configuration
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.OwnerId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UserIds).HasConversion(
                x => string.Join(',', x),
                x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(GetGuid)
                    .Where(g => g.HasValue)
                    .Select(g => g.Value)
                    .ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (hash, guid) => HashCode.Combine(hash, guid.GetHashCode())),
                c => c.ToList()
            ));


            builder
              .HasOne(u => u.GroupPhoto)
              .WithOne(p => p.Group)
              .HasForeignKey<Photo>(p => p.GroupId);
        }

        private Guid? GetGuid(string s)
        {
            if (Guid.TryParse(s, out var guid))
            {
                return guid;
            }

            return null;
        }
    }
}
