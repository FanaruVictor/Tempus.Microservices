using CategoryService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CategoryService.Data.Context.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .HasDiscriminator<string>("category_type")
                .HasValue<Category>("category")
                .HasValue<GroupCategory>("category_group");
        }
    }
}
