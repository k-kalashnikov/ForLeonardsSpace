using MediatR;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json.Linq;

namespace Masofa.BusinessLogic.Common
{
    public class GetCountryMultiPolygonCommand : IRequest<NetTopologySuite.Geometries.MultiPolygon?>
    {
    }

    public class GetCountryMultiPolygonCommandHandler : IRequestHandler<GetCountryMultiPolygonCommand, NetTopologySuite.Geometries.MultiPolygon?>
    {
        private readonly string _filePath;

        public GetCountryMultiPolygonCommandHandler(IConfiguration configuration)
        {
            _filePath = configuration.GetValue<string>("CountryBoundaries:GeoJsonFileName") ?? string.Empty;
        }

        public Task<NetTopologySuite.Geometries.MultiPolygon?> Handle(GetCountryMultiPolygonCommand request, CancellationToken cancellationToken)
        {
            NetTopologySuite.Geometries.MultiPolygon? result = null;

            var haveFile = File.Exists(_filePath);
            if (!haveFile) return Task.FromResult(result);

            var reader = new GeoJsonReader();
            JToken token = JToken.Parse(File.ReadAllText(_filePath));
            string rootType = token["type"]?.ToString() ?? "";

            Geometry geom = rootType switch
            {
                "FeatureCollection" => reader.Read<FeatureCollection>(token.ToString())[0].Geometry,
                "Polygon" => reader.Read<Polygon>(token.ToString()),
                "MultiPolygon" => reader.Read<MultiPolygon>(token.ToString()),
                _ => throw new NotSupportedException(rootType)
            };

            if (geom is MultiPolygon mp)
            {
                return Task.FromResult(mp);
            }
            else if (geom is Polygon p)
            {
                MultiPolygon mpoly = p.Factory.CreateMultiPolygon([p]);
                return Task.FromResult(mpoly);
            }

            return Task.FromResult(result);
        }
    }
}
