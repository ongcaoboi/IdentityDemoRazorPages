using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Data;
public class IdentityDemoDbContext : IdentityDbContext {

    public IdentityDemoDbContext (DbContextOptions<IdentityDemoDbContext> options) : base (options) { }

    protected override void OnModelCreating (ModelBuilder builder) {

        base.OnModelCreating (builder); 
        
        foreach (var entityType in builder.Model.GetEntityTypes()) {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet")) {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
    }
}