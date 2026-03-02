using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.DataAccess
{
    public class MasofaAnaliticReportDbContextFactory : IDesignTimeDbContextFactory<MasofaAnaliticReportDbContext>
    {
        public MasofaAnaliticReportDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaAnaliticReportDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaAnaliticReportDbContext(optionsBuilder.Options);
        }
    }
}
