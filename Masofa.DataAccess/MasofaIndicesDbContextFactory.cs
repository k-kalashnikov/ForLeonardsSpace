using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.DataAccess
{
    internal class MasofaIndicesDbContextFactory : IDesignTimeDbContextFactory<MasofaIndicesDbContext>
    {
        public MasofaIndicesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaIndicesDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaIndicesDbContext(optionsBuilder.Options);
        }
    }
}
