using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaTileHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaTileHistoryDbContext>
    {
        public MasofaTileHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaTileHistoryDbContext>();
            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });
            return new MasofaTileHistoryDbContext(optionsBuilder.Options);
        }
    }
}
