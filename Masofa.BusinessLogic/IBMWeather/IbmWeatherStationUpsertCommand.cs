using Masofa.Common.Models;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.IBMWeather
{
    public class IbmWeatherStationUpsertCommand : IRequest<Guid>
    {
        /// <summary>
        /// Координаты точки
        /// </summary>
        public required NetTopologySuite.Geometries.Point Point { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        //[Required]
        //public double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        //[Required]
        //public double Longitude { get; set; }

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

    public class IbmWeatherStationUpsertCommandHandler : IRequestHandler<IbmWeatherStationUpsertCommand, Guid>
    {
        private ILogger Logger { get; set; }
        private MasofaIBMWeatherDbContext IBMWeatherDbContext { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }
        public IbmWeatherStationUpsertCommandHandler(MasofaIBMWeatherDbContext iBMWeatherDbContext, MasofaIdentityDbContext identityDbContext, ILogger<IbmWeatherStationUpsertCommandHandler> logger)
        {
            IBMWeatherDbContext = iBMWeatherDbContext;
            IdentityDbContext = identityDbContext;
            Logger = logger;
        }

        public async Task<Guid> Handle(IbmWeatherStationUpsertCommand request, CancellationToken cancellationToken)
        {


            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var station = await IBMWeatherDbContext.IBMMeteoStations
                    .Where(s => s.Point.X == request.Point.X && s.Point.Y == request.Point.Y)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstOrDefaultAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                if (station == null)
                {
                    //var p = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude) { SRID = 4326 };

                    station = new IBMMeteoStation()
                    {
                        Point = request.Point,
                        CreateAt = DateTime.UtcNow,
                        CreateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty,
                        RegionId = request.RegionId,
                        //FieldId = request.FieldId,
                        Status = request.Status,
                        LastUpdateAt = DateTime.UtcNow,
                        LastUpdateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty
                    };
                    var result = await IBMWeatherDbContext.IBMMeteoStations.AddAsync(station, cancellationToken);
                    await IBMWeatherDbContext.SaveChangesAsync(cancellationToken);
                    return result.Entity.Id;
                }

                if (station.RegionId != request.RegionId || station.Status != request.Status)
                {

                    station.RegionId = request.RegionId;
                    station.Status = request.Status;
                    station.LastUpdateAt = DateTime.UtcNow;
                    station.LastUpdateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty;

                    IBMWeatherDbContext.IBMMeteoStations.Attach(station);

                    IBMWeatherDbContext.Entry(station).Property(s => s.RegionId).IsModified = true;
                    IBMWeatherDbContext.Entry(station).Property(s => s.Status).IsModified = true;
                    IBMWeatherDbContext.Entry(station).Property(s => s.LastUpdateAt).IsModified = true;
                    IBMWeatherDbContext.Entry(station).Property(s => s.LastUpdateUser).IsModified = true;

                    await IBMWeatherDbContext.SaveChangesAsync(cancellationToken);
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
