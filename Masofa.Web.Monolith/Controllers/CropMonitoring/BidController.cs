using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic;
using Masofa.BusinessLogic.CropMonitoring.Bids;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.Bids;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Text.Json;

namespace Masofa.Web.Monolith.Controllers.CropMonitoring
{
    /// <summary>
    /// Контроллер для работы с заявками
    /// </summary>
    [Route("cropMonitoring/[controller]")]
    [ApiExplorerSettings(GroupName = "CropMonitoring")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator, Foreman, FieldWorker")]
    public class BidController : BaseCrudController<Bid, MasofaCropMonitoringDbContext>
    {
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        public BidController(
            IFileStorageProvider fileStorageProvider,
            MasofaCropMonitoringDbContext dbContext,
            MasofaDictionariesDbContext masofaDictionariesDbContext,
            MasofaIdentityDbContext masofaIdentityDbContext,
            ILogger<BidController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaIdentityDbContext = masofaIdentityDbContext;
        }

        /// <summary>
        /// Сохраняет результат выполнения заявки в виде файла
        /// </summary>
        /// <param name="bidId">Идентификатор заявки</param>
        /// <param name="bidResultFile">Файл с результатом выполнения заявки</param>
        /// <returns>Результат операции сохранения</returns>
        /// <response code="200">Результат заявки успешно сохранен</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Заявка с указанным ID не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]/{bidId}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<bool>> SaveResult(Guid bidId, IFormFile bidResultFile)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(SaveResult)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var bid = DbContext.Bids.FirstOrDefault(m => m.Id.Equals(bidId));
                if (bid == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(Bid).FullName, bidId.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                using (var memStream = new MemoryStream())
                {
                    await bidResultFile.CopyToAsync(memStream);
                    await FileStorageProvider.PushFileAsync(memStream.ToArray(), bidResultFile.FileName, "bidresults");
                }

                var fileStorageItem = new FileStorageItem()
                {
                    FileContentType = FileContentType.ArchiveZIP,
                    FileStoragePath = bidResultFile.FileName,
                    FileStorageBacket = "bidresults",
                    OwnerTypeFullName = typeof(Bid).FullName,
                    OwnerId = bid.Id,
                    FileLength = bidResultFile.Length,
                };

                var fileStorageId = await Mediator.Send(new BaseCreateCommand<FileStorageItem, MasofaCommonDbContext>()
                {
                    Author = User.Identity.Name,
                    Model = fileStorageItem
                });

                bid.FileResultId = fileStorageId;
                bid.BidState = BidStateType.Finished;

                var bidResult = await Mediator.Send(new BaseUpdateCommand<Bid, MasofaCropMonitoringDbContext>()
                {
                    Author = User.Identity.Name,
                    Model = bid
                });
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, true.ToString()), requestPath);
                return true;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает список заявок по заданному запросу с расширенной информацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Список заявок с расширенной информацией</returns>
        /// <response code="200">Список заявок успешно получен</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<BidGetViewModel>>> CustomGetByQuery([FromBody] BaseGetQuery<Bid> query, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var bidViewModelList = await Mediator.Send(new GetBidByQueryRequest 
                { 
                    Query = query 
                }, 
                ct);

                return bidViewModelList;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Получает заявку по уникальному идентификатору с расширенной информацией
        /// </summary>
        /// <param name="id">Уникальный идентификатор заявки</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Заявка с расширенной информацией</returns>
        /// <response code="200">Заявка успешно найдена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="404">Заявка с указанным ID не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BidGetViewModel>> CustomGetById(Guid id, CancellationToken ct)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var bidViewModel = await Mediator.Send(new GetBidByIdRequest 
                { 
                    Id = id 
                }
                , ct);

                if (bidViewModel == null)
                {
                    return NotFound();
                }



                return bidViewModel;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Создает новую заявку с автоматическим определением поля и региона по координатам
        /// </summary>
        /// <param name="model">Данные для создания заявки</param>
        /// <returns>Уникальный идентификатор созданной заявки</returns>
        /// <response code="200">Заявка успешно создана</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Guid>> CustomCreate([FromBody] BidCreateViewModel model)
        {

            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";

            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                var point = gf.CreatePoint(new Coordinate(
                    x: model.Lng!,
                    y: model.Lat!));

                var bidTemplate = await DbContext.BidTemplates.FirstOrDefaultAsync(m => m.CropId.Equals(model.CropId));
                var field = (model.FieldId != null)
                                    ? await DbContext.Fields.FirstOrDefaultAsync(m => m.Id.Equals(model.FieldId))
                                    : await DbContext.Fields.Where(f => f.Polygon.SRID == point.SRID).FirstOrDefaultAsync(m => m.Polygon.Contains(point));


                var regionMap = await MasofaDictionariesDbContext.RegionMaps
                    .Where(m => m.Polygon.SRID == point.SRID)
                    .FirstOrDefaultAsync(m => m.Polygon.Contains(point));
                var regionId = (field == null)
                    ? ((regionMap == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.RegionMapId.Equals(regionMap.Id)))?.Id ?? Guid.Empty))
                    : ((field.RegionId == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.Id.Equals(field.RegionId)))?.Id ?? Guid.Empty));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var lastUpdateUser = await MasofaIdentityDbContext.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity.Name);
                var createUser = await MasofaIdentityDbContext.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                var poly = GeoHelper.ParsePolygon(model.Polygon);

                var bidModel = new Bid()
                {
                    DeadlineDate = new DateTime(model.DeadlineDate.Year, model.DeadlineDate.Month, model.DeadlineDate.Day, 0, 0, 0, DateTimeKind.Utc),
                    FieldPlantingDate = (model.FieldPlantingDate == null) ? null : new DateTime(model.FieldPlantingDate.Value.Year, model.FieldPlantingDate.Value.Month, model.FieldPlantingDate.Value.Day, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = model.EndDate,
                    FieldId = field?.Id ?? Guid.Empty,
                    RegionId = regionId,

                    ForemanId = model.ForemanId,
                    WorkerId = model.WorkerId,

                    BidTypeId = model.BidTypeId,
                    BidState = ((model.Publish.HasValue) && (model.Publish.Value)) ? BidStateType.Active : BidStateType.New,
                    BidTemplateId = bidTemplate?.Id ?? Guid.Empty,
                    CropId = model.CropId,
                    ParentId = model.ParentId,
                    VarietyId = model.VarietyId,

                    Lat = model.Lat,
                    Lng = model.Lng,

                    Customer = model.Customer,
                    Description = model.Description,

                    StartDate = (model.StartDate == null) ? null : new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, model.StartDate.Value.Day, 0, 0, 0, DateTimeKind.Utc),
                    Status = model.Status,
                    Comment = model.Comment,
                    CreateAt = DateTime.UtcNow,
                    LastUpdateAt = DateTime.UtcNow,
                    CreateUser = createUser.Id,
                    LastUpdateUser = lastUpdateUser.Id,
                    Polygon = poly
                };

                var createRequest = new BaseCreateCommand<Bid, MasofaCropMonitoringDbContext>()
                {
                    Model = bidModel,
                    Author = User.Identity.Name
                };

                var result = await Mediator.Send(createRequest);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Обновляет существующую заявку
        /// </summary>
        /// <param name="model">Данные для обновления заявки</param>
        /// <returns>Обновленная заявка</returns>
        /// <response code="200">Заявка успешно обновлена</response>
        /// <response code="400">Некорректные данные модели</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public new async Task<ActionResult<Bid>> CustomUpdate([FromBody] BidUpdateViewModel model)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var lastUpdateUser = await MasofaIdentityDbContext.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity.Name);

                var poly = GeoHelper.ParsePolygon(model.Polygon);

                var targetBidState = model.BidState;
                if (model.Publish == true)
                {
                    targetBidState = BidStateType.Active;
                }

                var bidUpdateMoedl = new Bid()
                {
                    Id = model.Id,
                    DeadlineDate = model.DeadlineDate,
                    Customer = model.Customer,
                    FieldId = model.FieldId,
                    RegionId = model.RegionId,
                    FieldPlantingDate = model.FieldPlantingDate,
                    Lat = model.Lat,
                    Lng = model.Lng,
                    Description = model.Description,
                    Comment = model.Comment,
                    BidState = model.BidState,
                    LastUpdateAt = DateTime.UtcNow,
                    LastUpdateUser = lastUpdateUser.Id,
                    CropId = model.CropId,
                    BidTemplateId = model.BidTemplateId,
                    BidTypeId = model.BidTypeId,
                    IsUnvalidBid = model.IsUnvalidBid,
                    ForemanId = model.ForemanId,
                    ParentId = model.ParentId,
                    Status = model.Status,
                    WorkerId = model.WorkerId,
                    VarietyId = model.VarietyId,
                    Number = model.Number,
                    EndDate = model.EndDate,
                    StartDate = model.StartDate,
                    Polygon = poly
                };

                var updateRequest = new BaseUpdateCommand<Bid, MasofaCropMonitoringDbContext>()
                {
                    Model = bidUpdateMoedl,
                    Author = User.Identity.Name
                };
                var result = await Mediator.Send(updateRequest);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    #region Helper
    public static class GeoHelper
    {
        private static readonly GeometryFactory Gf4326 =
            NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0000);

        public static Polygon? ParsePolygon(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            s = s.Trim().TrimStart('\uFEFF'); 

            if (s.StartsWith("{") || s.StartsWith("["))
            {
                var geom = new GeoJsonReader().Read<Geometry>(s);
                if (geom is GeometryCollection gc)
                {
                    geom = gc.Geometries.OfType<Polygon>().FirstOrDefault()
                           ?? throw new FormatException("GeoJSON не содержит Polygon.");
                }

                if (geom is not Polygon p)
                {
                    return null;

                }

                if (p.SRID == 0) p.SRID = 4326;
                return p;
            }

            var wktReader = new WKTReader(Gf4326);
            var g = wktReader.Read(s);
            if (g is GeometryCollection gc2)
            {
                g = gc2.Geometries.OfType<Polygon>().FirstOrDefault()
                    ?? throw new FormatException("WKT не содержит Polygon.");
                return null;
            }
            if (g is not Polygon poly)
            {
                return null;
            }

            if (poly.SRID == 0) poly.SRID = 0000;
            return poly;
        }

        public static string ToGeoJson(Polygon poly)
            => new GeoJsonWriter().Write(poly);
    }
    #endregion
}


//[HttpPost]
//[Microsoft.AspNetCore.Mvc.Route("[action]")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//public override async Task<ActionResult<Guid>> Create([FromBody] Bid model)
//{
//    var requestPath = $"{GetType().FullName}=>{nameof(Create)}";
//    try
//    {
//        await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
//        var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

//        var point = gf.CreatePoint(new Coordinate(
//            x: model.Lng!,
//            y: model.Lat!));

//        var bidTemplate = await DbContext.BidTemplates.FirstOrDefaultAsync(m => m.CropId.Equals(model.CropId));
//        var field = (model.FieldId != null)
//            ? await DbContext.Fields.FirstOrDefaultAsync(m => m.Id.Equals(model.Id))
//            : await DbContext.Fields.FirstOrDefaultAsync(m => m.Polygon.Contains(point));
//        var regionMap = await MasofaDictionariesDbContext.RegionMaps.FirstOrDefaultAsync(m => m.Polygon.Contains(point));
//        var regionId = (field == null)
//            ? ((regionMap == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.RegionMapId.Equals(regionMap.Id)))?.Id ?? Guid.Empty))
//            : ((field.RegionId == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.Id.Equals(field.RegionId)))?.Id ?? Guid.Empty));

//        var bid = new Bid()
//        {
//            DeadlineDate = model.DeadlineDate,
//            FieldPlantingDate = model.FieldPlantingDate,
//            EndDate = model.EndDate,
//            FieldId = field?.Id ?? Guid.Empty,
//            RegionId = regionId,

//            ForemanId = model.ForemanId,
//            WorkerId = model.WorkerId,

//            BidTypeId = model.BidTypeId,
//            BidState = BidStateType.New,
//            BidTemplateId = bidTemplate?.Id ?? Guid.Empty,
//            CropId = model.CropId,
//            ParentId = model.ParentId,
//            VarietyId = model.VarietyId,

//            Lat = model.Lat,
//            Lng = model.Lng,

//            Customer = model.Customer,
//            Description = model.Description,

//        };
//        return await base.Create(bid);
//    }
//    catch (Exception ex)
//    {
//        var msg = LogMessageResource.GenericError(requestPath, ex.Message);
//        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
//        Logger.LogCritical(ex, msg);
//        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//    }
//}

//[HttpPost]
//[Microsoft.AspNetCore.Mvc.Route("[action]")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//public async Task<ActionResult<List<BidGetViewModel>>> CustomGetByQuery([FromBody] BaseGetQuery<Bid> query, CancellationToken ct)
//{
//    var requestPath = $"{GetType().FullName}=>{nameof(CustomGetByQuery)}";
//    try
//    {
//        await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
//        var bidViewModelList = await Mediator.Send(new GetBidByQueryRequest
//        {
//            Query = query
//        },
//        ct);

//        return bidViewModelList;
//    }
//    catch (Exception ex)
//    {
//        var msg = LogMessageResource.GenericError(requestPath, ex.Message);
//        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
//        Logger.LogCritical(ex, msg);
//        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//    }
//}

//[HttpGet]
//[Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//public async Task<ActionResult<BidGetViewModel>> CustomGetById(Guid id, CancellationToken ct)
//{
//    var requestPath = $"{GetType().FullName}=>{nameof(CustomGetById)}";
//    try
//    {
//        await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
//        var bidViewModel = await Mediator.Send(new GetBidByIdRequest
//        {
//            Id = id
//        }
//        , ct);

//        if (bidViewModel == null)
//        {
//            var errorMsg = $"Entity with type {typeof(Bid)} with Id = {id.ToString()} not found";
//            await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
//            return NotFound(errorMsg);
//        }

//        return bidViewModel;
//    }
//    catch (Exception ex)
//    {
//        var msg = LogMessageResource.GenericError(requestPath, ex.Message);
//        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
//        Logger.LogCritical(ex, msg);
//        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//    }
//}

//[HttpPost]
//[Microsoft.AspNetCore.Mvc.Route("[action]")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//public new async Task<ActionResult<Guid>> CustomCreate([FromBody] BidCreateViewModel model)
//{
//    var requestPath = $"{GetType().FullName}=>{nameof(CustomCreate)}";
//    try
//    {
//        await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

//        if (!ModelState.IsValid)
//        {
//            await BusinessLogicLogger.LogErrorAsync($"Model is not valid in {requestPath}. Model: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}", requestPath);
//            return BadRequest(ModelState);
//        }

//        var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

//        var point = gf.CreatePoint(new Coordinate(
//            x: model.Lng!,
//            y: model.Lat!));

//        var bidTemplate = await DbContext.BidTemplates.FirstOrDefaultAsync(m => m.CropId.Equals(model.CropId));
//        var field = (model.FieldId != null)
//            ? await DbContext.Fields.FirstOrDefaultAsync(m => m.Id.Equals(model.FieldId))
//            : await DbContext.Fields.FirstOrDefaultAsync(m => m.Polygon.Contains(point));
//        var regionMap = await MasofaDictionariesDbContext.RegionMaps.FirstOrDefaultAsync(m => m.Polygon.Contains(point));
//        var regionId = (field == null)
//            ? ((regionMap == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.RegionMapId.Equals(regionMap.Id)))?.Id ?? Guid.Empty))
//            : ((field.RegionId == null) ? model.RegionId : ((await MasofaDictionariesDbContext.Regions.FirstOrDefaultAsync(m => m.Id.Equals(field.RegionId)))?.Id ?? Guid.Empty));

//        var lastUpdateUser = await MasofaIdentityDbContext.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity.Name);

//        var bidModel = new Bid()
//        {
//            DeadlineDate = model.DeadlineDate,
//            FieldPlantingDate = model.FieldPlantingDate,
//            EndDate = model.EndDate,
//            FieldId = field?.Id ?? Guid.Empty,
//            RegionId = regionId,

//            ForemanId = model.ForemanId,
//            WorkerId = model.WorkerId,

//            BidTypeId = model.BidTypeId,
//            BidState = BidStateType.New,
//            BidTemplateId = bidTemplate?.Id ?? Guid.Empty,
//            CropId = model.CropId,
//            ParentId = model.ParentId,
//            VarietyId = model.VarietyId,

//            Lat = model.Lat,
//            Lng = model.Lng,

//            Customer = model.Customer,
//            Description = model.Description,
//        };

//        var createRequest = new BaseCreateCommand<Bid, MasofaCropMonitoringDbContext>()
//        {
//            Model = bidModel,
//            Author = User.Identity.Name
//        };
//        var result = await Mediator.Send(createRequest);
//        return result;
//    }
//    catch (Exception ex)
//    {
//        var msg = LogMessageResource.GenericError(requestPath, ex.Message);
//        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
//        Logger.LogCritical(ex, msg);
//        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//    }
//}

/// <summary>
//        /// 
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Microsoft.AspNetCore.Mvc.Route("[action]")]
//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//        public new async Task<ActionResult<Bid>> CustomUpdate([FromBody] BidUpdateViewModel model)
//        {

//            var requestPath = $"{GetType().FullName}=>{nameof(CustomCreate)}";
//            await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

//            if (!ModelState.IsValid)
//            {
//                await BusinessLogicLogger.LogErrorAsync($"Model is not valid in {requestPath}. Model: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}", requestPath);
//                return BadRequest(ModelState);
//            }

//            var lastUpdateUser = await MasofaIdentityDbContext.Users.FirstOrDefaultAsync(l => l.UserName == User.Identity.Name);

//            var bidUpdateMoedl = new Bid()
//            {
//                Id = model.Id,
//                DeadlineDate = model.DeadlineDate,
//                Customer = model.Customer,
//                FieldId = model.FieldId,
//                RegionId = model.RegionId,
//                FieldPlantingDate = model.FieldPlantingDate,
//<<<<<<< HEAD
//                EndDate = model.EndDate,
//                FieldId = field?.Id ?? Guid.Empty,
//                RegionId = regionId,

//                ForemanId = model.ForemanId,
//                WorkerId = model.WorkerId,

//                BidTypeId = model.BidTypeId,
//                BidState = BidStateType.New,
//                BidTemplateId =  bidTemplate?.Id ?? Guid.Empty,
//                CropId = model.CropId,
//                ParentId = model.ParentId,
//                VarietyId = model.VarietyId,

//=======
//>>>>>>> origin/develop
//                Lat = model.Lat,
//                Lng = model.Lng,
//                Description = model.Description,
//<<<<<<< HEAD
//                Polygon = model.Polygon
//            };

//            try
//            {
//                var updateRequest = new BaseUpdateCommand<Bid, MasofaCropMonitoringDbContext>()
//                {
//                    Model = bidUpdateMoedl,
//                    Author = User.Identity.Name
//                };
//                var result = await Mediator.Send(updateRequest);
//                await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}", requestPath);
//                return result;
//            }
//            catch (Exception ex)
//            {
//                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
//                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
//                Logger.LogCritical(ex, msg);
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }