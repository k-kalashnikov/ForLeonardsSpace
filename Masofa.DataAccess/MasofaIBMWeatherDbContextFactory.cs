using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Factory для создания контекста данных для работы с моделями спутников Sentinel
    /// </summary>
    public class MasofaIBMWeatherDbContextFactory : IDesignTimeDbContextFactory<MasofaIBMWeatherDbContext>
    {
        public MasofaIBMWeatherDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaIBMWeatherDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaIBMWeatherDbContext(optionsBuilder.Options);
        }
    }
} 