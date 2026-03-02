using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaSentinelHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaSentinelHistoryDbContext>
    {
        public MasofaSentinelHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaSentinelHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaSentinelHistoryDbContext(optionsBuilder.Options);
        }
    }
}
