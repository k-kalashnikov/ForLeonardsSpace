using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Masofa.BusinessLogic.Cli.Migration
{
    public class BidTemplateMigrationCommand : IRequest<List<string>>
    {
        public List<IFormFile> Files { get; set; }
    }

    public class BidTemplateMigrationCommandHandler : IRequestHandler<BidTemplateMigrationCommand, List<string>>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private IMediator Mediator { get; set; }
        private ILogger Logger { get; set; }

        public BidTemplateMigrationCommandHandler(MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, 
            IMediator mediator, 
            MasofaDictionariesDbContext masofaDictionariesDbContext, 
            ILogger<BidTemplateMigrationCommandHandler> logger)
        {
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            Mediator = mediator;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            Logger = logger;
        }

        public async Task<List<string>> Handle(BidTemplateMigrationCommand request, CancellationToken cancellationToken)
        {
            var messages = new List<string>();
            foreach (var file in request.Files)
            {
                try
                {
                    using (var fileStream = new StreamReader(file.OpenReadStream()))
                    {
                        var templateText = await fileStream.ReadToEndAsync();
                        var templateObj = JsonConvert.DeserializeObject<Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2.BidTemplateSchemaVersion2>(templateText);
                        var crop = MasofaDictionariesDbContext.Crops.FirstOrDefault(m => m.Id.Equals(templateObj.CropId));

                        if (crop == null)
                        {
                            messages.Add($"Crop with Id = {templateObj.CropId} not found");
                            continue;
                        }

                        if (MasofaCropMonitoringDbContext.BidTemplates.Any(bt =>
                            bt.ContentVersion.Equals(templateObj.ContentVersion)
                            && bt.SchemaVersion.Equals(templateObj.SchemaVersion)
                            && bt.CropId.Equals(templateObj.CropId)))
                        {
                            messages.Add($"Bid Templates with CropId = {templateObj.CropId} and ContentVersion = {templateObj.ContentVersion} and SchemaVersion = {templateObj.SchemaVersion} already exist");
                            continue;
                        }

                        var bidTemplate = new BidTemplate()
                        {
                            ContentVersion = templateObj.ContentVersion,
                            SchemaVersion = templateObj.SchemaVersion,
                            CropId = templateObj.CropId,
                            Data = templateObj
                        };

                        var btId = await Mediator.Send(new BaseCreateCommand<BidTemplate, MasofaCropMonitoringDbContext>()
                        {
                            Author = "Admin",
                            Model = bidTemplate
                        });
                        messages.Add($"Bid Templates with CropId = {templateObj.CropId} and ContentVersion = {templateObj.ContentVersion} and SchemaVersion = {templateObj.SchemaVersion} created with id {btId}");
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"Something wrong in {nameof(Masofa.BusinessLogic.Cli.Migration)} => {nameof(BidTemplateMigrationCommandHandler)} => {nameof(Handle)}";
                    messages.Add(msg);
                    Logger.LogCritical(ex, msg);
                }
            }


            return messages;
        }
    }
}
