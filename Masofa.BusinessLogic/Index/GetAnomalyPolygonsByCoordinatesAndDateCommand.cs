using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Masofa.BusinessLogic.Index
{
    public class GetAnomalyPolygonsByCoordinatesAndDateCommand : IRequest<List<AnomalyPolygon>>
    {
        [Required]
        public required decimal West { get; set; }

        [Required]
        public required decimal East { get; set; }

        [Required]
        public required decimal South { get; set; }

        [Required]
        public required decimal North { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public List<AnomalyType> AnomalyTypes { get; set; }
    }

    public class GetAnomalyPolygonsByCoordinatesAndDateCommandHandler : IRequestHandler<GetAnomalyPolygonsByCoordinatesAndDateCommand, List<AnomalyPolygon>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<GetPolygonsByFieldAndDateCommandHandler> Logger { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }
        public GetAnomalyPolygonsByCoordinatesAndDateCommandHandler(
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GetPolygonsByFieldAndDateCommandHandler> logger,
            MasofaIndicesDbContext indicesDbContext)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            IndicesDbContext = indicesDbContext;
        }

        public async Task<List<AnomalyPolygon>> Handle(GetAnomalyPolygonsByCoordinatesAndDateCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                var west = (double)request.West;
                var east = (double)request.East;
                var south = (double)request.South;
                var north = (double)request.North;

                var coordinates = new[]
                {
                    new NetTopologySuite.Geometries.Coordinate(west, south),
                    new NetTopologySuite.Geometries.Coordinate(east, south),
                    new NetTopologySuite.Geometries.Coordinate(east, north),
                    new NetTopologySuite.Geometries.Coordinate(west, north),
                    new NetTopologySuite.Geometries.Coordinate(west, south)
                };

                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);
                var boundingBox = geometryFactory.CreatePolygon(coordinates);

                var query = IndicesDbContext.AnomalyPolygons.AsNoTracking();
                query = query.Where(ap => ap.Status == StatusType.Active);
                query = query.Where(ap => ap.Polygon.Intersects(boundingBox) && DateOnly.FromDateTime(ap.OriginalDate) == request.Date);
                if (request.AnomalyTypes.Count != 0)
                {
                    query = query.Where(ap => request.AnomalyTypes.Contains(ap.AnomalyType));
                }

                var result = await query.ToListAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }
    }
}
