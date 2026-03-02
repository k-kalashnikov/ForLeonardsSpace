using Masofa.Common.Models.Satellite;
using MediatR;
using Microsoft.Extensions.Logging;
using Masofa.BusinessLogic.FieldSatellite.Requests;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    public class GetFieldProductsRequest : IRequest<GetFieldProductsResponse>
    {
        public Guid FieldId { get; set; }
        public ProductSourceType? SatelliteType { get; set; }
    }

    public class GetFieldProductsResponse
    {
        public List<FieldProductInfo> Products { get; set; } = new();
    }

    public class FieldProductInfo
    {
        public string ProductId { get; set; } = default!;
        public ProductSourceType SatelliteType { get; set; }
    }

    public class GetFieldProductsRequestHandler : IRequestHandler<GetFieldProductsRequest, GetFieldProductsResponse>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetFieldProductsRequestHandler> _logger;

        public GetFieldProductsRequestHandler(
            IMediator mediator,
            ILogger<GetFieldProductsRequestHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<GetFieldProductsResponse> Handle(GetFieldProductsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting products for field {FieldId} from mapping", request.FieldId);

            var mappings = await _mediator.Send(new GetProductsForFieldRequest(
                request.FieldId,
                request.SatelliteType,
                cancellationToken), cancellationToken);

            return new GetFieldProductsResponse
            {
                Products = mappings.Select(m => new FieldProductInfo
                {
                    ProductId = m.ProductId,
                    SatelliteType = m.SatelliteType
                }).ToList()
            };
        }
    }
}
