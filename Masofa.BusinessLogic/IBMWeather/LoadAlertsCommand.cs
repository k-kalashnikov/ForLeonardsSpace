using MediatR;
using Masofa.Common.Models.IBMWeather;
using Microsoft.EntityFrameworkCore;
using Masofa.BusinessLogic.IBMWeather.BaseCommands;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.IBMWeather;
using Masofa.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Masofa.BusinessLogic.IBMWeather.Commands;

/// <summary>
/// Команда для загрузки алертов IBM Weather
/// </summary>
public class LoadAlertsCommand : BaseIBMWeatherLoadCommand
{
    /// <summary>
    /// Идентификатор метеостанции (если null - для всех активных станций)
    /// </summary>
    public Guid? IBMMeteoStationId { get; set; }

    /// <summary>
    /// Принудительное обновление существующих данных
    /// </summary>
    public bool ForceUpdate { get; set; } = false;

    public LoadAlertsCommand(
        Guid? ibmMeteoStationId = null,
        bool forceUpdate = false,
        CancellationToken cancellationToken = default)
    {
        IBMMeteoStationId = ibmMeteoStationId;
        ForceUpdate = forceUpdate;
    }
}

/// <summary>
/// Обработчик команды для загрузки алертов
/// </summary>
//public class LoadAlertsHandler : BaseIBMWeatherLoadHandler<LoadAlertsCommand>
//{
//    public LoadAlertsHandler(
//    IBMWeatherApiUnitOfWork unitOfWork,
//    MasofaIBMWeatherDbContext dbContext,
//    IBusinessLogicLogger logger,
//    IConfiguration configuration) : base(unitOfWork, dbContext, logger, configuration)
//    {
//    }

//    public override async Task Handle(LoadAlertsCommand request, CancellationToken cancellationToken)
//    {
//        await LogOperationStartAsync("LoadAlerts", 
//            $"StationId: {request.IBMMeteoStationId}, ForceUpdate: {request.ForceUpdate}");

//        try
//        {
//            var stations = await GetStationsToProcessAsync(request.IBMMeteoStationId);
//            var processedCount = 0;

//            foreach (var station in stations)
//            {
//                var count = await LoadAlertsForStationAsync(station, request.ForceUpdate);
//                processedCount += count;
//            }

//            await LogOperationEndAsync("LoadAlerts", processedCount, "алертов");
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadAlerts", ex);
//            throw;
//        }
//    }

//    /// <summary>
//    /// Получение списка станций для обработки
//    /// </summary>
//    private async Task<List<IBMMeteoStation>> GetStationsToProcessAsync(Guid? stationId)
//    {
//        if (stationId.HasValue)
//        {
//            var station = await _dbContext.IBMMeteoStations
//                .FirstOrDefaultAsync(s => s.Id == stationId.Value);

//            if (station == null)
//            {
//                throw new ArgumentException($"Метеостанция с ID {stationId.Value} не найдена");
//            }

//            return new List<IBMMeteoStation> { station };
//        }

//        // Получаем все активные станции
//        return await _dbContext.IBMMeteoStations
//            .Where(s => s.IsActive && s.Status == Masofa.Common.Models.StatusType.Active)
//            .ToListAsync();
//    }

//    /// <summary>
//    /// Загрузка алертов для конкретной станции
//    /// </summary>
//    private async Task<int> LoadAlertsForStationAsync(IBMMeteoStation station, bool forceUpdate)
//    {
//        try
//        {
//            var alertsResponse = await _unitOfWork.IBMWeatherRepository.GetWeatherAlertsAsync(station.Point);

//            var processedCount = 0;

//            foreach (var alert in alertsResponse.Alerts)
//            {
//                // Проверяем, существует ли уже такой алерт
//                var existingAlert = await _dbContext.IBMWeatherAlerts
//                    .FirstOrDefaultAsync(a => a.Identifier == alert.Identifier);

//                if (existingAlert != null && !forceUpdate)
//                    continue;

//                var weatherAlert = new IBMWeatherAlert
//                {
//                    Id = Guid.NewGuid(),
//                    IBMMeteoStationId = station.Id,
//                    AdminDistrict = alert.AdminDistrict,
//                    AdminDistrictCode = alert.AdminDistrictCode,
//                    AreaId = alert.AreaId,
//                    AreaName = alert.AreaName,
//                    AreaTypeCode = alert.AreaTypeCode,
//                    Certainty = alert.Certainty,
//                    CertaintyCode = alert.CertaintyCode,
//                    CountryCode = alert.CountryCode,
//                    CountryName = alert.CountryName,
//                    DetailKey = alert.DetailKey,
//                    Disclaimer = alert.Disclaimer,
//                    DisplayRank = alert.DisplayRank,
//                    EffectiveTimeLocal = !string.IsNullOrEmpty(alert.EffectiveTimeLocal) ? DateTime.Parse(alert.EffectiveTimeLocal) : null,
//                    EffectiveTimeLocalTimeZone = alert.EffectiveTimeLocalTimeZone,
//                    EventDescription = alert.EventDescription,
//                    EventTrackingNumber = alert.EventTrackingNumber,
//                    ExpireTimeLocal = DateTime.Parse(alert.ExpireTimeLocal),
//                    ExpireTimeLocalTimeZone = alert.ExpireTimeLocalTimeZone,
//                    ExpireTimeUTC = DateTimeOffset.FromUnixTimeSeconds(alert.ExpireTimeUTC).DateTime,
//                    HeadlineText = alert.HeadlineText,
//                    IanaTimeZone = alert.IanaTimeZone,
//                    Identifier = alert.Identifier,
//                    IssueTimeLocal = DateTime.Parse(alert.IssueTimeLocal),
//                    IssueTimeLocalTimeZone = alert.IssueTimeLocalTimeZone,
//                    Latitude = alert.Latitude,
//                    Longitude = alert.Longitude,
//                    MessageType = alert.MessageType,
//                    MessageTypeCode = alert.MessageTypeCode,
//                    OfficeAdminDistrict = alert.OfficeAdminDistrict,
//                    OfficeAdminDistrictCode = alert.OfficeAdminDistrictCode,
//                    OfficeCode = alert.OfficeCode,
//                    OfficeCountryCode = alert.OfficeCountryCode,
//                    OfficeName = alert.OfficeName,
//                    OnsetTimeLocal = !string.IsNullOrEmpty(alert.OnsetTimeLocal) ? DateTime.Parse(alert.OnsetTimeLocal) : null,
//                    OnsetTimeLocalTimeZone = alert.OnsetTimeLocalTimeZone,
//                    Phenomena = alert.Phenomena,
//                    ProcessTimeUTC = DateTimeOffset.FromUnixTimeSeconds(alert.ProcessTimeUTC).DateTime,
//                    ProductIdentifier = alert.ProductIdentifier,
//                    Severity = alert.Severity,
//                    SeverityCode = alert.SeverityCode,
//                    Significance = alert.Significance,
//                    Source = alert.Source,
//                    Urgency = alert.Urgency,
//                    UrgencyCode = alert.UrgencyCode,
//                    EndTimeLocal = !string.IsNullOrEmpty(alert.EndTimeLocal) ? DateTime.Parse(alert.EndTimeLocal) : null,
//                    EndTimeLocalTimeZone = alert.EndTimeLocalTimeZone,
//                    EndTimeUTC = alert.EndTimeUTC.HasValue ? DateTimeOffset.FromUnixTimeSeconds(alert.EndTimeUTC.Value).DateTime : null,
//                    CreateAt = DateTime.UtcNow,
//                    Status = Masofa.Common.Models.StatusType.Active,
//                    LastUpdateAt = DateTime.UtcNow,
//                    Names = new Masofa.Common.Models.LocalizationString 
//                    { 
//                        ValuesJson = alert.HeadlineText 
//                    }
//                };

//                if (existingAlert != null)
//                {
//                    // Обновляем существующий алерт
//                    UpdateExistingAlert(existingAlert, weatherAlert);
//                }
//                else
//                {
//                    _dbContext.IBMWeatherAlerts.Add(weatherAlert);
//                }

//                // Обрабатываем категории алерта
//                ProcessAlertCategories(weatherAlert, alert.Category);

//                // Обрабатываем типы ответов алерта
//                ProcessAlertResponseTypes(weatherAlert, alert.ResponseTypes);

//                // Обрабатываем информацию о наводнении
//                if (alert.Flood != null)
//                {
//                    await ProcessFloodInfoAsync(weatherAlert, alert.Flood);
//                }

//                processedCount++;
//            }

//            if (processedCount > 0)
//            {
//                await SaveChangesAsync();
//            }

//            return processedCount;
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadAlertsForStation", ex, $"StationId: {station.Id}");
//            return 0;
//        }
//    }

//    /// <summary>
//    /// Обновление существующего алерта
//    /// </summary>
//    private void UpdateExistingAlert(IBMWeatherAlert existing, IBMWeatherAlert updated)
//    {
//        existing.AdminDistrict = updated.AdminDistrict;
//        existing.AdminDistrictCode = updated.AdminDistrictCode;
//        existing.AreaId = updated.AreaId;
//        existing.AreaName = updated.AreaName;
//        existing.AreaTypeCode = updated.AreaTypeCode;
//        existing.Certainty = updated.Certainty;
//        existing.CertaintyCode = updated.CertaintyCode;
//        existing.CountryCode = updated.CountryCode;
//        existing.CountryName = updated.CountryName;
//        existing.DetailKey = updated.DetailKey;
//        existing.Disclaimer = updated.Disclaimer;
//        existing.DisplayRank = updated.DisplayRank;
//        existing.EffectiveTimeLocal = updated.EffectiveTimeLocal;
//        existing.EffectiveTimeLocalTimeZone = updated.EffectiveTimeLocalTimeZone;
//        existing.EventDescription = updated.EventDescription;
//        existing.EventTrackingNumber = updated.EventTrackingNumber;
//        existing.ExpireTimeLocal = updated.ExpireTimeLocal;
//        existing.ExpireTimeLocalTimeZone = updated.ExpireTimeLocalTimeZone;
//        existing.ExpireTimeUTC = updated.ExpireTimeUTC;
//        existing.HeadlineText = updated.HeadlineText;
//        existing.IanaTimeZone = updated.IanaTimeZone;
//        existing.IssueTimeLocal = updated.IssueTimeLocal;
//        existing.IssueTimeLocalTimeZone = updated.IssueTimeLocalTimeZone;
//        existing.Latitude = updated.Latitude;
//        existing.Longitude = updated.Longitude;
//        existing.MessageType = updated.MessageType;
//        existing.MessageTypeCode = updated.MessageTypeCode;
//        existing.OfficeAdminDistrict = updated.OfficeAdminDistrict;
//        existing.OfficeAdminDistrictCode = updated.OfficeAdminDistrictCode;
//        existing.OfficeCode = updated.OfficeCode;
//        existing.OfficeCountryCode = updated.OfficeCountryCode;
//        existing.OfficeName = updated.OfficeName;
//        existing.OnsetTimeLocal = updated.OnsetTimeLocal;
//        existing.OnsetTimeLocalTimeZone = updated.OnsetTimeLocalTimeZone;
//        existing.Phenomena = updated.Phenomena;
//        existing.ProcessTimeUTC = updated.ProcessTimeUTC;
//        existing.ProductIdentifier = updated.ProductIdentifier;
//        existing.Severity = updated.Severity;
//        existing.SeverityCode = updated.SeverityCode;
//        existing.Significance = updated.Significance;
//        existing.Source = updated.Source;
//        existing.Urgency = updated.Urgency;
//        existing.UrgencyCode = updated.UrgencyCode;
//        existing.EndTimeLocal = updated.EndTimeLocal;
//        existing.EndTimeLocalTimeZone = updated.EndTimeLocalTimeZone;
//        existing.EndTimeUTC = updated.EndTimeUTC;
//        existing.LastUpdateAt = DateTime.UtcNow;
//    }

//    /// <summary>
//    /// Обработка категорий алерта
//    /// </summary>
//    private void ProcessAlertCategories(IBMWeatherAlert weatherAlert, List<Masofa.Client.IBMWeather.Models.AlertCategory> categories)
//    {
//        var categoryFlags = IBMWeatherAlertCategoryEnum.undefined;

//        foreach (var category in categories)
//        {
//            switch (category.CategoryCode)
//            {
//                case 1:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Geo;
//                    break;
//                case 2:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Met;
//                    break;
//                case 3:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Safety;
//                    break;
//                case 4:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Security;
//                    break;
//                case 5:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Rescue;
//                    break;
//                case 6:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Fire;
//                    break;
//                case 7:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Health;
//                    break;
//                case 8:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Env;
//                    break;
//                case 9:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Transport;
//                    break;
//                case 10:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Infra;
//                    break;
//                case 11:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.CBRNE;
//                    break;
//                case 12:
//                    categoryFlags |= IBMWeatherAlertCategoryEnum.Other;
//                    break;
//            }
//        }

//        weatherAlert.WeatherAlertCategory = categoryFlags;
//    }

//    /// <summary>
//    /// Обработка типов ответов алерта
//    /// </summary>
//    private void ProcessAlertResponseTypes(IBMWeatherAlert weatherAlert, List<Masofa.Client.IBMWeather.Models.AlertResponseType> responseTypes)
//    {
//        var responseTypeFlags = IBMWeatherAlertResponseTypeEnum.undefined;

//        foreach (var responseType in responseTypes)
//        {
//            switch (responseType.ResponseTypeCode)
//            {
//                case 1:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Shelter;
//                    break;
//                case 2:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Evacuate;
//                    break;
//                case 3:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Prepare;
//                    break;
//                case 4:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Execute;
//                    break;
//                case 5:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Avoid;
//                    break;
//                case 6:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Monitor;
//                    break;
//                case 7:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.Assess;
//                    break;
//                case 8:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.AllClear;
//                    break;
//                case 9:
//                    responseTypeFlags |= IBMWeatherAlertResponseTypeEnum.None;
//                    break;
//            }
//        }

//        weatherAlert.WeatherAlertResponseType = responseTypeFlags;
//    }

//    /// <summary>
//    /// Обработка информации о наводнении
//    /// </summary>
//    private async Task ProcessFloodInfoAsync(IBMWeatherAlert weatherAlert, Masofa.Client.IBMWeather.Models.FloodAlertInfo floodInfo)
//    {
//        var existingFloodInfo = await _dbContext.IBMWeatherAlertFloodInfos
//            .FirstOrDefaultAsync(f => f.WeatherAlertId == weatherAlert.Id);

//        var floodData = new IBMWeatherAlertFloodInfo
//        {
//            Id = Guid.NewGuid(),
//            WeatherAlertId = weatherAlert.Id,
//            FloodCrestTimeLocal = !string.IsNullOrEmpty(floodInfo.FloodCrestTimeLocal) ? DateTime.Parse(floodInfo.FloodCrestTimeLocal) : null,
//            FloodCrestTimeLocalTimeZone = floodInfo.FloodCrestTimeLocalTimeZone,
//            FloodEndTimeLocal = !string.IsNullOrEmpty(floodInfo.FloodEndTimeLocal) ? DateTime.Parse(floodInfo.FloodEndTimeLocal) : null,
//            FloodEndTimeLocalTimeZone = floodInfo.FloodEndTimeLocalTimeZone,
//            FloodImmediateCause = floodInfo.FloodImmediateCause,
//            FloodImmediateCauseCode = floodInfo.FloodImmediateCauseCode,
//            FloodLocationId = floodInfo.FloodLocationId,
//            FloodLocationName = floodInfo.FloodLocationName,
//            FloodRecordStatus = floodInfo.FloodRecordStatus,
//            FloodRecordStatusCode = floodInfo.FloodRecordStatusCode,
//            FloodSeverity = floodInfo.FloodSeverity,
//            FloodSeverityCode = floodInfo.FloodSeverityCode,
//            FloodStartTimeLocal = !string.IsNullOrEmpty(floodInfo.FloodStartTimeLocal) ? DateTime.Parse(floodInfo.FloodStartTimeLocal) : null,
//            FloodStartTimeLocalTimeZone = floodInfo.FloodStartTimeLocalTimeZone,
//            CreateAt = DateTime.UtcNow,
//            Status = Masofa.Common.Models.StatusType.Active,
//            LastUpdateAt = DateTime.UtcNow,
//        };

//        if (existingFloodInfo != null)
//        {
//            // Обновляем существующую информацию о наводнении
//            existingFloodInfo.FloodCrestTimeLocal = floodData.FloodCrestTimeLocal;
//            existingFloodInfo.FloodCrestTimeLocalTimeZone = floodData.FloodCrestTimeLocalTimeZone;
//            existingFloodInfo.FloodEndTimeLocal = floodData.FloodEndTimeLocal;
//            existingFloodInfo.FloodEndTimeLocalTimeZone = floodData.FloodEndTimeLocalTimeZone;
//            existingFloodInfo.FloodImmediateCause = floodData.FloodImmediateCause;
//            existingFloodInfo.FloodImmediateCauseCode = floodData.FloodImmediateCauseCode;
//            existingFloodInfo.FloodLocationId = floodData.FloodLocationId;
//            existingFloodInfo.FloodLocationName = floodData.FloodLocationName;
//            existingFloodInfo.FloodRecordStatus = floodData.FloodRecordStatus;
//            existingFloodInfo.FloodRecordStatusCode = floodData.FloodRecordStatusCode;
//            existingFloodInfo.FloodSeverity = floodData.FloodSeverity;
//            existingFloodInfo.FloodSeverityCode = floodData.FloodSeverityCode;
//            existingFloodInfo.FloodStartTimeLocal = floodData.FloodStartTimeLocal;
//            existingFloodInfo.FloodStartTimeLocalTimeZone = floodData.FloodStartTimeLocalTimeZone;
//            existingFloodInfo.LastUpdateAt = DateTime.UtcNow;
//        }
//        else
//        {
//            _dbContext.IBMWeatherAlertFloodInfos.Add(floodData);
//        }
//    }
//}
