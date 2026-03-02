using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Era
{
    public class EraWeatherStationGetByCoordinatesCommand : IRequest<EraWeatherStation?>
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public double Longitude { get; set; }
    }

    public class EraWeatherStationGetByCoordinatesCommandHandler : IRequestHandler<EraWeatherStationGetByCoordinatesCommand, EraWeatherStation?>
    {
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraWeatherStationGetByCoordinatesCommandHandler(MasofaEraDbContext eraDbContext)
        {
            EraDbContext = eraDbContext;
        }

        public async Task<EraWeatherStation?> Handle(EraWeatherStationGetByCoordinatesCommand request, CancellationToken cancellationToken)
        {
            var station = await EraDbContext.EraWeatherStations
                .Where(s => s.Point.X == request.Longitude && s.Point.Y == request.Latitude)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return station;
        }
    }
}
