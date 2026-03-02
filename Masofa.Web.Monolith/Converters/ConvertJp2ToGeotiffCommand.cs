using Masofa.Web.Monolith.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using OSGeo.GDAL;
using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.Converters
{
    public class ConvertJp2ToGeotiffCommand : IRequest<string>
    {
        [Required]
        public required string InputPath { get; set; }

        [Required]
        public required string OutputPath { get; set; } = string.Empty;
    }

    public class ConvertJp2ToGeotiffCommandHandler : IRequestHandler<ConvertJp2ToGeotiffCommand, string>
    {
        private ILogger Logger { get; set; }
        private GdalInitializer GdalInitializer { get; }

        public ConvertJp2ToGeotiffCommandHandler(ILogger<ConvertJp2ToGeotiffCommandHandler> logger, GdalInitializer gdalInitializer)
        {
            Logger = logger;
            GdalInitializer = gdalInitializer;
        }

        public async Task<string> Handle(ConvertJp2ToGeotiffCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                using Dataset srcDs = GdalInitializer.Open(request.InputPath, Access.GA_ReadOnly)
                    ?? throw new Exception("Cannot open JP2 file: " + request.InputPath);

                var drv = GdalInitializer.GetDriverByName("GTiff")
                    ?? throw new Exception("GTiff driver is unavailable");

                using Dataset dstDs = drv.CreateCopy(request.OutputPath, srcDs, 0, null, null, null);

                return dstDs == null
                    ? throw new Exception("Cannot create GeoTIFF")
                    : request.OutputPath;
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