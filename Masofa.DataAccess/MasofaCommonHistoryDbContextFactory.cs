using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaCommonHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaCommonHistoryDbContext>
    {
        public MasofaCommonHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaCommonHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaCommonHistoryDbContext(optionsBuilder.Options);
        }
    }
}
