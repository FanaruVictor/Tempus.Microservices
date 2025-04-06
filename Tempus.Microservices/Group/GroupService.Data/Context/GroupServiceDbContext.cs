using System.Reflection;
using GroupService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Data.Context;

public class GroupServiceDbContext(DbContextOptions<GroupServiceDbContext> options) : DbContext(options)
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}