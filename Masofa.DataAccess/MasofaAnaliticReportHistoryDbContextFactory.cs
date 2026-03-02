using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    public class MasofaAnaliticReportHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaAnaliticReportHistoryDbContext>
    {
        public MasofaAnaliticReportHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaAnaliticReportHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaAnaliticReportHistoryDbContext(optionsBuilder.Options);
        }
    }
}
