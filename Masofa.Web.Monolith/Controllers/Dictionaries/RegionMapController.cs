using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Resources;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления картами регионов
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class RegionMapController : BaseDictionaryController<RegionMap, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegionMapController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public RegionMapController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<RegionMapController> logger,
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
        }

        /// <summary>
        /// Получает список сущностей по заданному запросу с фильтрацией, сортировкой и пагинацией
        /// </summary>
        /// <param name="query">Объект запроса с параметрами фильтрации, сортировки и пагинации</param>
        /// <returns>Список найденных сущностей</returns>
        /// <response code="200">Успешно получен список сущностей</response>
        /// <response code="400">Некорректные параметры запроса</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<RegionMap>>> CustomGetByQuery([FromBody] BaseGetQuery<RegionMap> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                var currentRegionMaps = (await base.GetByQuery(query)).Value;
                if (currentRegionMaps == null)
                {
                    return NotFound();
                }

                Dictionary<Guid, Guid> regionByMap = await DbContext.Regions
                    .Where(r => r.RegionMapId != null && currentRegionMaps.Select(rm => rm.Id).ToList().Contains(r.RegionMapId.Value))
                    .ToDictionaryAsync(r => r.RegionMapId ?? Guid.Empty, r => r.Id);

                var childRegionsByParent = await DbContext.Regions
                    .Where(r => r.ParentId != null && regionByMap.Values.ToList().Contains(r.ParentId.Value) && r.RegionMapId != null)
                    .GroupBy(r => r.ParentId ?? Guid.Empty)
                    .ToDictionaryAsync(x => x.Key, x => x.ToList());

                List<Guid> childRegionMapIds = [];
                foreach (var (parentId, childRegions) in childRegionsByParent)
                {
                    foreach (var childRegion in childRegions)
                    {
                        if (childRegion.RegionMapId != null)
                        {
                            childRegionMapIds.Add(childRegion.RegionMapId.Value);
                        }
                    }
                }

                var childRegionMapsById = await DbContext.RegionMaps.Where(rm => rm.Polygon != null && childRegionMapIds.Contains(rm.Id))
                    .ToDictionaryAsync(rm => rm.Id, rm => rm);

                foreach (var regionMap in currentRegionMaps)
                {
                    if (regionByMap.TryGetValue(regionMap.Id, out var parentRegionId))
                    {
                        if (childRegionsByParent.TryGetValue(parentRegionId, out var childRegions))
                        {
                            List<NetTopologySuite.Geometries.Polygon> geometries = [];
                            foreach (var childRegion in childRegions)
                            {
                                if (childRegion.Id == childRegion.ParentId) continue;

                                if (childRegion.RegionMapId != null)
                                {
                                    if (childRegionMapsById.TryGetValue(childRegion.RegionMapId.Value, out var childRegionMap))
                                    {
                                        if (childRegionMap.Polygon != null)
                                        {
                                            if (childRegionMap.Polygon is NetTopologySuite.Geometries.Polygon polygon)
                                            {
                                                geometries.Add(polygon);
                                            }
                                            else if (childRegionMap.Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                                            {
                                                geometries.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                                            }
                                        }
                                    }
                                }
                            }

                            if (geometries.Count > 0)
                            {
                                regionMap.Polygon = CreateConcaveEnvelope(geometries);
                            }
                        }
                    }
                }

                return currentRegionMaps;
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
        /// Получает сущность по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор сущности</param>
        /// <returns>Найденная сущность</returns>
        /// <response code="200">Сущность успешно найдена</response>
        /// <response code="404">Сущность с указанным ID не найдена</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]/{id}")]
        public async Task<ActionResult<RegionMap>> CustomGetById(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetById)}";
            try
            {
                var currentRegionMap = (await base.GetById(id)).Value;
                if (currentRegionMap == null)
                {
                    return NotFound();
                }

                var currentRegion = await DbContext.Regions.FirstOrDefaultAsync(r => r.RegionMapId == id);
                if (currentRegion == null)
                {
                    return currentRegionMap;
                }

                var childRegions = await DbContext.Regions
                    .Where(r => r.ParentId == currentRegion.Id && r.Id != currentRegion.Id && r.RegionMapId != null)
                    .ToListAsync();

                if (childRegions.Count == 0)
                {
                    return currentRegionMap;
                }

                var childRegionMaps = await DbContext.RegionMaps
                    .Where(rm => childRegions.Select(r => r.RegionMapId).Contains(rm.Id) && rm.Polygon != null)
                    .ToListAsync();

                List<NetTopologySuite.Geometries.Polygon> geometries = [];
                foreach (var childRegionMap in childRegionMaps)
                {
                    if (childRegionMap.Polygon != null)
                    {
                        if (childRegionMap.Polygon is NetTopologySuite.Geometries.Polygon polygon)
                        {
                            geometries.Add(polygon);
                        }
                        else if (childRegionMap.Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                        {
                            geometries.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                        }
                    }
                }

                if (geometries.Count > 0)
                {
                    currentRegionMap.Polygon = CreateConcaveEnvelope(geometries);
                }

                return currentRegionMap;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private NetTopologySuite.Geometries.Geometry CreateConcaveEnvelope(List<NetTopologySuite.Geometries.Polygon> cluster, double bufferDistance = 0.001)
        {
            var factory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);

            var bufferedGeometries = cluster.Select(p => p.Buffer(bufferDistance)).ToList();

            var union = bufferedGeometries[0];
            for (int i = 1; i < bufferedGeometries.Count; i++)
            {
                union = union.Union(bufferedGeometries[i]);
            }

            var concaveHull = union.Buffer(-bufferDistance);

            return concaveHull;
        }


        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<List<RegionMapShort>>> CustomGetPolygonOnly([FromBody] BaseGetQuery<RegionMap> query)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetByQuery)}";
            try
            {
                var currentRegionMaps = (await base.GetByQuery(query)).Value;
                if (currentRegionMaps == null)
                {
                    return NotFound();
                }

                Dictionary<Guid, Guid> regionByMap = await DbContext.Regions
                    .Where(r => r.RegionMapId != null && currentRegionMaps.Select(rm => rm.Id).ToList().Contains(r.RegionMapId.Value))
                    .ToDictionaryAsync(r => r.RegionMapId ?? Guid.Empty, r => r.Id);

                var childRegionsByParent = await DbContext.Regions
                    .Where(r => r.ParentId != null && regionByMap.Values.ToList().Contains(r.ParentId.Value) && r.RegionMapId != null)
                    .GroupBy(r => r.ParentId ?? Guid.Empty)
                    .ToDictionaryAsync(x => x.Key, x => x.ToList());

                List<Guid> childRegionMapIds = [];
                foreach (var (parentId, childRegions) in childRegionsByParent)
                {
                    foreach (var childRegion in childRegions)
                    {
                        if (childRegion.RegionMapId != null)
                        {
                            childRegionMapIds.Add(childRegion.RegionMapId.Value);
                        }
                    }
                }

                var childRegionMapsById = await DbContext.RegionMaps.Where(rm => rm.Polygon != null && childRegionMapIds.Contains(rm.Id))
                    .ToDictionaryAsync(rm => rm.Id, rm => rm);

                foreach (var regionMap in currentRegionMaps)
                {
                    if (regionByMap.TryGetValue(regionMap.Id, out var parentRegionId))
                    {
                        if (childRegionsByParent.TryGetValue(parentRegionId, out var childRegions))
                        {
                            List<NetTopologySuite.Geometries.Polygon> geometries = [];
                            foreach (var childRegion in childRegions)
                            {
                                if (childRegion.Id == childRegion.ParentId) continue;

                                if (childRegion.RegionMapId != null)
                                {
                                    if (childRegionMapsById.TryGetValue(childRegion.RegionMapId.Value, out var childRegionMap))
                                    {
                                        if (childRegionMap.Polygon != null)
                                        {
                                            if (childRegionMap.Polygon is NetTopologySuite.Geometries.Polygon polygon)
                                            {
                                                geometries.Add(polygon);
                                            }
                                            else if (childRegionMap.Polygon is NetTopologySuite.Geometries.MultiPolygon multiPolygon)
                                            {
                                                geometries.AddRange(multiPolygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToArray());
                                            }
                                        }
                                    }
                                }
                            }

                            if (geometries.Count > 0)
                            {
                                regionMap.Polygon = CreateConcaveEnvelope(geometries);
                            }
                        }
                    }
                }

                return currentRegionMaps.Select(m => new RegionMapShort()
                {
                    Id = m.Id,
                    PolygonArrayJson = m.PolygonArrayJson,
                    Lat = m.Lat,
                    Lng = m.Lng,
                }).ToList();
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
}
