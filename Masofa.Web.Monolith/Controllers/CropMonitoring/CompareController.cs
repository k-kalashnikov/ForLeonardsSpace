using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class CompareController : BaseController
    {
        private DepricatedUmapiServerTwoDbContext DepricatedUmapiServerTwoDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private DepricatedUfieldsServerOneDbContext DepricatedUfieldsServerOneDbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }


        public CompareController(
            ILogger<CompareController> logger,
            IConfiguration configuration,
            IMediator mediator,
            DepricatedUmapiServerTwoDbContext depricatedUmapiServerTwoDbContext,
            DepricatedUfieldsServerOneDbContext depricatedUfieldsServerOneDbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext,
            IHttpContextAccessor httpContextAccessor,
            IBusinessLogicLogger businessLogicLogger) : base(logger, configuration, mediator)
        {
            DepricatedUmapiServerTwoDbContext = depricatedUmapiServerTwoDbContext;
            DepricatedUfieldsServerOneDbContext = depricatedUfieldsServerOneDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            HttpContextAccessor = httpContextAccessor;
            BusinessLogicLogger = businessLogicLogger;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<CompareResultViewModel>>> GetCompareFieldData()
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(GetCompareFieldData)}";
        //    var result = new List<CompareResultViewModel>();
        //    try
        //    {
        //        await BusinessLogicLogger.LogInformationAsync($"Start request in {requestPath}", requestPath);
        //        var bids = await DepricatedUmapiServerTwoDbContext.Bids.Select(m => new Bid()
        //        {
        //            Id = m.Id,
        //            CropId = (Guid)m.CropId,
        //            FieldPlantingDate = m.FieldPlantingDate.HasValue ? ResolveDateTime(m.FieldPlantingDate.Value) : null,
        //            RegionId = m.RegionId
        //        }).ToListAsync();
        //        var crops = await MasofaDictionariesDbContext.Crops.Select(m => new Crop()
        //        {
        //            Id = m.Id,
        //            NameEn = m.NameEn,
        //            NameRu = m.NameRu,
        //            CropPeriods = new List<CropPeriod>()
        //        }).ToListAsync();
        //        var regions = await MasofaDictionariesDbContext.Regions.Select(m => new Region()
        //        {
        //            Id = m.Id,
        //            NameEn = m.NameEn,
        //            NameRu = m.NameRu,
        //        }).ToListAsync();
        //        foreach (var bid in bids)
        //        {
        //            var item = new CompareResultViewModel()
        //            {
        //                BidNumber = (int)bid.Number,
        //                SeedDate = bid.FieldPlantingDate,
        //                Crop = crops.FirstOrDefault(c => c.Id.Equals(bid.CropId)),
        //                Region = regions.FirstOrDefault(r => r.Id.Equals(bid.RegionId))
        //            };
        //            if (bid.Lat != null && bid.Lng != null)
        //            {
        //                var point = new Point(bid.Lng, bid.Lat) { SRID = 4326 };
        //                item.FieldPolygonJson = DepricatedUfieldsServerOneDbContext.Fields.FirstOrDefault(f => (f.Polygon != null) && (f.Polygon.Contains(point)))?
        //                    .Polygon?.ToText() ?? string.Empty;
        //            }
        //            result.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = $"Something wrong in {requestPath}. {ex.Message}";
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //    return result;
        //}

        private static DateTime ResolveDateTime(LocalDate localDateTime)
        {
            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
        }
    }

    public class CompareResultViewModel
    {
        public Region? Region { get; set; }
        public Crop? Crop { get; set; }
        public DateTime? SeedDate { get; set; }
        public int BidNumber { get; set; }
        public Geometry? FieldPolygon { get; set; }
        public string FieldPolygonJson { get; set; } = string.Empty;
    }
}
