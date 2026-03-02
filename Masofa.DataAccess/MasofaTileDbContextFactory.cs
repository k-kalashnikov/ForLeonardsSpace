using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaTileDbContextFactory : IDesignTimeDbContextFactory<MasofaTileDbContext>
    {
        public MasofaTileDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaTileDbContext>();
            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });
            return new MasofaTileDbContext(optionsBuilder.Options);
        }
    }
}
