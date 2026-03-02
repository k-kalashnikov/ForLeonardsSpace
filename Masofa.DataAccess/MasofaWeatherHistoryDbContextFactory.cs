using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaWeatherHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaWeatherHistoryDbContext>
    {
        public MasofaWeatherHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaWeatherHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaWeatherHistoryDbContext(optionsBuilder.Options);
        }
    }
}
