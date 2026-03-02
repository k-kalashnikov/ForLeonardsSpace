using Masofa.Common.Resources;

using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Satellite;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;
using MediatR;

namespace Masofa.BusinessLogic.Common.Satellite
{
    public class GetIndicexBySatelliteProductRequest : IRequest<IndicexBySatelliteProductViewModel>
    {
        public Guid SatelliteProductId { get; set; }
    }

    public class GetIndicexBySatelliteProductRequestHandler : IRequestHandler<GetIndicexBySatelliteProductRequest, IndicexBySatelliteProductViewModel>
    {

        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public GetIndicexBySatelliteProductRequestHandler(MasofaIndicesDbContext masofaIndicesDbContext, IBusinessLogicLogger businessLogicLogger)
        {
            MasofaIndicesDbContext = masofaIndicesDbContext;
            BusinessLogicLogger = businessLogicLogger;
        }



        public async Task<IndicexBySatelliteProductViewModel> Handle(GetIndicexBySatelliteProductRequest request, CancellationToken cancellationToken)
        {
            var path = $"{nameof(Masofa.BusinessLogic.Common.Satellite)}=>{nameof(GetIndicexBySatelliteProductRequestHandler)}=>{nameof(Handle)}";
            try
            {
                var result = new IndicexBySatelliteProductViewModel();
                result.SatelliteProductId = request.SatelliteProductId;
                result.ArviId = MasofaIndicesDbContext.ArviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.EviId = MasofaIndicesDbContext.EviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.GndviId = MasofaIndicesDbContext.GndviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.MndwiId = MasofaIndicesDbContext.MndwiPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.NdmiId = MasofaIndicesDbContext.NdmiPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.NdviId = MasofaIndicesDbContext.NdviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.OrviId = MasofaIndicesDbContext.OrviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                result.OsaviId = MasofaIndicesDbContext.OsaviPolygons.FirstOrDefault(m => m.SatelliteProductId == request.SatelliteProductId)?.Id;
                return result;
            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError(path, ex.Message), path);
                throw ex;
            }
        }
    }


    public class IndicexBySatelliteProductViewModel
    {
        public Guid SatelliteProductId { get; set; }
        public Guid? ArviId { get; set; }
        public Guid? EviId { get; set; }
        public Guid? GndviId { get; set; }
        public Guid? MndwiId { get; set; }
        public Guid? NdmiId { get; set; }
        public Guid? NdviId { get; set; }
        public Guid? OrviId { get; set; }
        public Guid? OsaviId { get; set; }
    }
}
