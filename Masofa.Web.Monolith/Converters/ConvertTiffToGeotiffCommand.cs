using Masofa.Web.Monolith.Services;
using MediatR;
using OSGeo.GDAL;
using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.Converters
{
    public class ConvertTiffToGeotiffCommand : IRequest<string>
    {
        [Required]
        public required string InputPath { get; set; }

        [Required]
        public required string OutputPath { get; set; } = string.Empty;
    }

    public class ConvertTiffToGeotiffCommandHandler : IRequestHandler<ConvertTiffToGeotiffCommand, string>
    {
        private GdalInitializer GdalInitializer { get; }
        private ILogger Logger { get; set; }

        public ConvertTiffToGeotiffCommandHandler(ILogger<ConvertTiffToGeotiffCommandHandler> logger, GdalInitializer gdalInitializer)
        {
            Logger = logger;
            GdalInitializer = gdalInitializer;
        }

        public async Task<string> Handle(ConvertTiffToGeotiffCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                using var src = GdalInitializer.Open(request.InputPath, Access.GA_ReadOnly);
                using var dst = GdalInitializer.GetDriverByName("GTiff").CreateCopy(request.OutputPath, src, 0, new[] { "COMPRESS=LZW", "TILED=YES" }, null, null);
                return request.OutputPath;
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
