using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaLandsatHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaLandsatHistoryDbContext>
    {
        public MasofaLandsatHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaLandsatHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaLandsatHistoryDbContext(optionsBuilder.Options);
        }
    }
}
