using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Masofa.DataAccess
{
    public class MasofaIdentityDbContextFactory : IDesignTimeDbContextFactory<MasofaIdentityDbContext>
    {
        public MasofaIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaIdentityDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaIdentityDbContext(optionsBuilder.Options);
        }
    }
}
