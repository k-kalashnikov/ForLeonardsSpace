using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaIdentityHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaIdentityHistoryDbContext>
    {
        public MasofaIdentityHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaIdentityHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaIdentityHistoryDbContext(optionsBuilder.Options);
        }
    }
}
