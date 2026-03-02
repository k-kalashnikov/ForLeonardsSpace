using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Cli.DevopsUtil.Commands.CropMonitoring
{
    [BaseCommand("Fill Bids Period", "Fill Bids Period")]
    public class FillBidsPeriodCommand : IBaseCommand
    {
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }

        private static readonly Random Random = new();

        public FillBidsPeriodCommand(MasofaCropMonitoringDbContext cropMonitoringDbContext)
        {
            CropMonitoringDbContext = cropMonitoringDbContext;
        }

        public FillBidsPeriodCommand()
        {
        }

        public void Dispose()
        {
            Console.WriteLine("\nFillBidsPeriodCommand END");
        }

        public async Task Execute()
        {
            Console.WriteLine("FillBidsPeriodCommand START\n");
            try
            {
                var bids = await CropMonitoringDbContext.Bids.Where(b => b.StartDate == null || b.EndDate == null).ToListAsync();

                await File.WriteAllTextAsync("BidsBeforePeriodFill.json", Newtonsoft.Json.JsonConvert.SerializeObject(bids));

                foreach (var b in bids)
                {
                    if (b.StartDate == null && b.EndDate == null)
                    {
                        b.StartDate = b.CreateAt.AddDays(GetRandomValue());
                        b.EndDate = b.StartDate.Value.AddDays(1);
                    }
                    else if (b.StartDate == null && b.EndDate != null)
                    {
                        b.StartDate = b.EndDate.Value.AddDays(-1);
                    }
                    else if (b.StartDate != null && b.EndDate == null)
                    {
                        b.EndDate = b.StartDate.Value.AddDays(1);
                    }
                    else
                    {
                        Console.WriteLine("Super ELSE");
                    }
                }

                await File.WriteAllTextAsync("BidsAfterPeriodFill.json", Newtonsoft.Json.JsonConvert.SerializeObject(bids));

                await CropMonitoringDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        private int GetRandomValue()
        {
            int[] values = { 1, 2, 3 };
            return values[Random.Next(values.Length)];
        }
    }
}
