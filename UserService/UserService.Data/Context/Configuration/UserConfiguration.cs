using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Core.Entities;

namespace UserService.Data.Context.Configuration
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36).IsRequired();
            builder.Property(x => x.Username).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.IsDarkTheme).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.PhoneNumber).IsRequired(false);
            builder.Property(x => x.GroupIds).IsRequired(false).HasConversion(
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
                .HasOne(u => u.Photo)
                .WithOne(p => p.User)
                .HasForeignKey<Photo>(p => p.UserId);
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
