using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Factory для создания контекста данных для работы с моделями спутников Sentinel
    /// </summary>
    public class MasofaSentinelDbContextFactory : IDesignTimeDbContextFactory<MasofaSentinelDbContext>
    {
        public MasofaSentinelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaSentinelDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaSentinelDbContext(optionsBuilder.Options);
        }
    }
} 