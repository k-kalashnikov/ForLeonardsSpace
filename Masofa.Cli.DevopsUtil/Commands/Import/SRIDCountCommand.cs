using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    [BaseCommand("Polygon: SRID", "Вычисление срид")]
    public class SRIDCountCommand : IBaseCommand
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        public MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        public SRIDCountCommand(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }
        public void Dispose()
        {
            
        }

        public Task Execute()
        {
            return SridCountMath();
        }

        public Task Execute(string[] args)
        {
            return SridCountMath();
        }

        public Task SridCountMath()
        {
            var wgs84 = MasofaCropMonitoringDbContext.Fields.Where(x => x.Polygon != null);
            var other = MasofaCropMonitoringDbContext.Seasons.Where(x => x.Polygon != null);
            //var mercator = MasofaCropMonitoringDbContext.Fields.Where(x => x.Polygon != null);

            Console.WriteLine($"Count of 4326 SRID: {wgs84.Count()}");
            Console.WriteLine($"Count of 0000 SRID: {other.Count()}");
            //Console.WriteLine($"Count of 3857 SRID: {mercator.Count()}");

            return Task.CompletedTask;
        }
    }
}
