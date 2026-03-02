using Masofa.Common.Models;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Era
{
    public class EraWeatherStationUpsertCommand : IRequest<Guid>
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

        /// <summary>
        /// Регион
        /// </summary>
        [Required]
        public Guid RegionId { get; set; }

        /// <summary>
        /// Поле
        /// </summary>
        [Required]
        public Guid FieldId { get; set; }

        /// <summary>
		/// Статус сущности
		/// </summary>
        public StatusType Status { get; set; } = StatusType.Active;

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        [Required]
        public required string Author { get; set; }
    }

    public class EraWeatherStationUpsertCommandHandler : IRequestHandler<EraWeatherStationUpsertCommand, Guid>
    {
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        private MasofaEraDbContext EraDbContext { get; set; }

        public EraWeatherStationUpsertCommandHandler(ILogger<EraWeatherStationUpsertCommandHandler> logger,
            IMediator mediator, MasofaIdentityDbContext identityDbContext, MasofaEraDbContext eraDbContext)
        {
            Logger = logger;
            Mediator = mediator;
            IdentityDbContext = identityDbContext;
            EraDbContext = eraDbContext;
        }

        public async Task<Guid> Handle(EraWeatherStationUpsertCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var station = await Mediator.Send(new EraWeatherStationGetByCoordinatesCommand()
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                }, cancellationToken);

                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstOrDefaultAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                if (station == null)
                {
                    var p = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude)
                    {
                        SRID = 4326
                    };
                    station = new EraWeatherStation()
                    {
                        Point = p,
                        CreateAt = DateTime.UtcNow,
                        CreateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty,
                        RegionId = request.RegionId,
                        FieldId = request.FieldId,
                        Status = request.Status,
                        LastUpdateAt = DateTime.UtcNow,
                        LastUpdateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty
                    };
                    var result = await EraDbContext.EraWeatherStations.AddAsync(station, cancellationToken);
                    await EraDbContext.SaveChangesAsync(cancellationToken);
                    return result.Entity.Id;
                }

                if (station.RegionId != request.RegionId || station.Status != request.Status)
                {

                    station.RegionId = request.RegionId;
                    station.Status = request.Status;
                    station.LastUpdateAt = DateTime.UtcNow;
                    station.LastUpdateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty;

                    EraDbContext.EraWeatherStations.Attach(station);

                    EraDbContext.Entry(station).Property(s => s.RegionId).IsModified = true;
                    EraDbContext.Entry(station).Property(s => s.Status).IsModified = true;
                    EraDbContext.Entry(station).Property(s => s.LastUpdateAt).IsModified = true;
                    EraDbContext.Entry(station).Property(s => s.LastUpdateUser).IsModified = true;

                    await EraDbContext.SaveChangesAsync(cancellationToken);
                }

                return station.Id;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}