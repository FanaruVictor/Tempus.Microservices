using Microsoft.EntityFrameworkCore;
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
            builder.Property(x => x.IsDarkTheme).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ExternalId).IsRequired(false);
            builder.Property(x => x.PhoneNumber).IsRequired(false);

            builder
                .HasOne(u => u.UserPhoto)
                .WithOne(p => p.User)
                .HasForeignKey<UserPhoto>(p => p.UserId);
        }
    }
}
