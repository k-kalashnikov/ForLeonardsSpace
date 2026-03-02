using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Factory для создания контекста данных для работы с моделями спутников Landsat
    /// </summary>
    public class MasofaLandsatDbContextFactory : IDesignTimeDbContextFactory<MasofaLandsatDbContext>
    {
        public MasofaLandsatDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaLandsatDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaLandsatDbContext(optionsBuilder.Options);
        }
    }
} 