using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;

namespace Masofa.Cli.DevopsUtil.Commands.Export
{
    [BaseCommand("Bid Template Export", "BidTemplateExport")]
    public class BidTemplateExport : IBaseCommand
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        public BidTemplateExport(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {
            var bidTeplates = MasofaCropMonitoringDbContext.BidTemplates.ToList();
            var crops = MasofaDictionariesDbContext.Crops.ToList();
            foreach (var bidTemplate in bidTeplates)
            {
                var dataJson = bidTemplate.DataJson;
                var crop = MasofaDictionariesDbContext.Crops.First(m => m.Id == bidTemplate.CropId);

                File.WriteAllText($"D:\\Debug\\{crop.Names["en-US"]}_{bidTemplate.SchemaVersion}_{bidTemplate.ContentVersion}.json", dataJson);
            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
