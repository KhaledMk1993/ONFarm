using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ONFarm.Infrastructure.Data;

namespace ONFarm.Infrastructure;

public class ONFarmDbContextFactory : IDesignTimeDbContextFactory<ONFarmDbContext>
{
    public ONFarmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ONFarmDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=onfarm_db;Username=postgres;Password=postgres");

        return new ONFarmDbContext(optionsBuilder.Options);
    }
}
