using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistrationService.Core.Entities;

namespace RegistrationService.Data.Context.Configuration
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.LastUpdatedAt).IsRequired();
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
        }
    }
}
