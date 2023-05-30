using Microsoft.EntityFrameworkCore;
using password_usermanagement.Models;

namespace password_usermanagement.Data;

public class DatabaseContext : DbContext
{
    public virtual DbSet<Role> Roles { get; set; }
 
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
        
    }
    public override int SaveChanges()
    {
        var now = DateTime.UtcNow;

        // Set "created at" and "updated at" fields for new and modified entities
        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            if (entry.Entity.GetType().GetProperty("CreatedAt") != null)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                else
                {
                    entry.Property("CreatedAt").IsModified = false;
                }
            }

            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }

        return base.SaveChanges();
    }
}
