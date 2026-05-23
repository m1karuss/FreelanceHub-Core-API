using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FreelanceHub.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Use a dummy connection string for migrations
            optionsBuilder.UseMySql(
                "Server=localhost;Database=freelancehub_dev;User=root;Password=password;",
                new MySqlServerVersion(new Version(8, 0, 21))
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
