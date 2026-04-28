using Microsoft.EntityFrameworkCore;

namespace VehiclePartsPro.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets will be added later by your friend
    }
}