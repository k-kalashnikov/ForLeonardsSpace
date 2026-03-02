using Masofa.BusinessLogic.Index;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Cli.DevopsUtil.Commands.Indices
{
    [BaseCommand("Calculate anomalies for previous dates", "Calculate anomalies for previous dates")]
    public class CalculateAnomaliesCommand : IBaseCommand
    {
        private IMediator Mediator { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }

        public CalculateAnomaliesCommand(MasofaIndicesDbContext masofaIndicesDbContext, IMediator mediator)
        {
            IndicesDbContext = masofaIndicesDbContext;
            Mediator = mediator;
        }


        public void Dispose()
        {
            Console.WriteLine("\nCalculateAnomaliesCommand END");
        }

        public async Task Execute()
        {
            Console.WriteLine("CalculateAnomaliesCommand START\n");

            var anomaliesDates = await IndicesDbContext.AnomalyPoints
                .AsNoTracking()
                .Select(p => p.CreateAt)
                .ToHashSetAsync();

            var indicesDates = await IndicesDbContext.NdviPoints
                .AsNoTracking()
                .Where(p => !anomaliesDates.Contains(p.CreateAt))
                .OrderBy(p => p.CreateAt)
                .Select(p => p.CreateAt)
                .ToHashSetAsync();

            foreach (var indexDateTime in indicesDates)
            {
                await Mediator.Send(new CalculateAnomalyOnDateCommand()
                {
                    Date = DateOnly.FromDateTime(indexDateTime)
                });
            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
