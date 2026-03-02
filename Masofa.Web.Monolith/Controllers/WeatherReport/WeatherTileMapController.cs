using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Resources;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    /// <summary>
    /// Контроллер для получения данных плиточной карты погоды
    /// </summary>
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WeatherTileMapController : BaseController
    {
        public WeatherTileMapController(
            ILogger<WeatherTileMapController> logger,
            IConfiguration configuration,
            IMediator mediator) : base(logger, configuration, mediator)
        {
        }

        /// <summary>
        /// Массовое получение данных для плиточной карты погоды для нескольких регионов одним запросом
        /// </summary>
        /// <param name="request">Запрос с массивом RegionMapIds и параметрами</param>
        /// <returns>Список данных для плиток с цветами</returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<List<WeatherTileMapDataResponse>>> GetTileMapDataBatch([FromBody] GetWeatherTileMapDataBatchRequestDto request)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTileMapDataBatch)}";
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.RegionMapIds == null || request.RegionMapIds.Count == 0)
                {
                    return BadRequest("RegionMapIds list cannot be empty");
                }

                var mediatorRequest = new GetWeatherTileMapDataBatchRequest
                {
                    RegionMapIds = request.RegionMapIds,
                    DataType = request.DataType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Date = request.Date,
                    CropId = request.CropId
                };

                var result = await Mediator.Send(mediatorRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    /// <summary>
    /// DTO для запроса массового получения данных плиточной карты
    /// </summary>
    public class GetWeatherTileMapDataBatchRequestDto
    {
        /// <summary>
        /// Список идентификаторов RegionMap (плиток)
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<Guid> RegionMapIds { get; set; } = new();

        /// <summary>
        /// Тип данных:
        /// 1 - Уведомления от УГМ о стихийных бедствиях
        /// 2 - Отклонения от температурных норм
        /// 3 - Отклонения от осадочных норм
        /// 4 - Карта заморозков
        /// 5 - Карта неблагоприятных условий для культур
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int DataType { get; set; }

        /// <summary>
        /// Начальная дата (для типа 1 - период)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Конечная дата (для типа 1 - период)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Дата (для типов 2-5)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Идентификатор культуры (для типа 5)
        /// </summary>
        public Guid? CropId { get; set; }
    }
}
