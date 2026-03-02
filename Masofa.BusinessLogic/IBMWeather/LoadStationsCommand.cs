using MediatR;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Distance;
using Masofa.BusinessLogic.IBMWeather.BaseCommands;
using Masofa.Client.IBMWeather;
using Microsoft.Extensions.Configuration;

namespace Masofa.BusinessLogic.IBMWeather.Commands;

/// <summary>
/// Команда для загрузки метеостанций IBM Weather
/// </summary>
public class LoadStationsCommand : BaseIBMWeatherLoadCommand
{
    /// <summary>
    /// Идентификатор региона для поиска (механизм 1)
    /// </summary>
    public Guid? RegionId { get; set; }

    /// <summary>
    /// Широта для поиска (механизм 2)
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Долгота для поиска (механизм 2)
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Код страны для фильтрации
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Тип локаций для поиска (airport,metar,pws)
    /// </summary>
    public string? LocationType { get; set; }

    public LoadStationsCommand(
        Guid? regionId = null,
        double? latitude = null,
        double? longitude = null,
        string? countryCode = null,
        string? locationType = null)
    {
        RegionId = regionId;
        Latitude = latitude;
        Longitude = longitude;
        CountryCode = countryCode;
        LocationType = locationType;
    }
}

/// <summary>
/// Обработчик команды для загрузки метеостанций
/// </summary>
//public class LoadStationsHandler : BaseIBMWeatherLoadHandler<LoadStationsCommand>
//{
//    private readonly MasofaDictionariesDbContext _dictionariesContext;

//    public LoadStationsHandler(
//        IBMWeatherApiUnitOfWork unitOfWork,
//        MasofaIBMWeatherDbContext dbContext,
//        MasofaDictionariesDbContext dictionariesContext,
//        IBusinessLogicLogger logger,
//        IConfiguration configuration) : base(unitOfWork, dbContext, logger, configuration)
//    {
//        _dictionariesContext = dictionariesContext;
//    }

//    public override async Task Handle(LoadStationsCommand request, CancellationToken cancellationToken)
//    {
//        await LogOperationStartAsync("LoadStations", $"RegionId: {request.RegionId}, Lat: {request.Latitude}, Lon: {request.Longitude}");

//        try
//        {
//            var stationsToProcess = new List<(NetTopologySuite.Geometries.Point point, string? countryCode, string? locationType)>();

//            // Механизм 1: Поиск по RegionId
//            if (request.RegionId.HasValue)
//            {
//                var regionStations = await GetStationsByRegionAsync(request.RegionId.Value, request.CountryCode, request.LocationType);
//                stationsToProcess.AddRange(regionStations);
//            }
//            // Механизм 2: Поиск по координатам
//            else if (request.Latitude.HasValue && request.Longitude.HasValue)
//            {
//                stationsToProcess.Add((new Point(request.Longitude.Value, request.Latitude.Value), request.CountryCode, request.LocationType));
//            }
//            else
//            {
//                throw new ArgumentException("Необходимо указать либо RegionId, либо координаты (Latitude и Longitude)");
//            }

//            var processedCount = 0;

//            foreach (var (point, countryCode, locationType) in stationsToProcess)
//            {
//                var newStations = await ProcessLocationSearchAsync(point, countryCode, locationType);
//                processedCount += newStations;
//            }

//            await LogOperationEndAsync("LoadStations", processedCount, "станций");
//        }
//        catch (Exception ex)
//        {
//            await LogErrorAsync("LoadStations", ex);
//            throw;
//        }
//    }

//    /// <summary>
//    /// Получение станций по региону (механизм 1)
//    /// </summary>
//    private async Task<List<(NetTopologySuite.Geometries.Point point, string? countryCode, string? locationType)>> GetStationsByRegionAsync(
//        Guid regionId, string? countryCode, string? locationType)
//    {
//        var region = await _dictionariesContext.Regions
//            .FirstOrDefaultAsync(r => r.Id == regionId);

//        if (region == null)
//        {
//            throw new ArgumentException($"Регион с ID {regionId} не найден");
//        }

//        // Получаем центр полигона региона
//        var centerPoint = await GetRegionCenterAsync(regionId);
        
//        return new List<(Point, string?, string?)> 
//        { 
//            (centerPoint, countryCode, locationType) 
//        };
//    }

//    /// <summary>
//    /// Получение центра полигона региона
//    /// </summary>
//    private async Task<Point> GetRegionCenterAsync(Guid regionId)
//    {
//        // Используем PostGIS функцию ST_Centroid для получения центра полигона
//        var centerQuery = @"
//            SELECT ST_Centroid(rm.Geometry) as Center
//            FROM ""RegionMaps"" rm
//            WHERE rm.""RegionId"" = {0}";

//        var center = await _dictionariesContext.Database
//            .SqlQueryRaw<Point>(centerQuery, regionId)
//            .FirstOrDefaultAsync();

//        if (center == null)
//        {
//            throw new InvalidOperationException($"Не удалось получить центр полигона для региона {regionId}");
//        }

//        return center;
//    }

//    /// <summary>
//    /// Обработка поиска локаций и создание станций
//    /// </summary>
//    private async Task<int> ProcessLocationSearchAsync(Point point, string? countryCode, string? locationType)
//    {
//        var searchResponse = await _unitOfWork.IBMWeatherRepository.SearchLocationsAsync(
//            point, countryCode, locationType);

//        var newStationsCount = 0;

//        for (int i = 0; i < searchResponse.Location.Latitude.Count; i++)
//        {
//            var lat = searchResponse.Location.Latitude[i];
//            var lon = searchResponse.Location.Longitude[i];
//            var city = searchResponse.Location.City.Count > i ? searchResponse.Location.City[i] : null;
//            var country = searchResponse.Location.Country.Count > i ? searchResponse.Location.Country[i] : null;
//            var countryCodeResult = searchResponse.Location.CountryCode.Count > i ? searchResponse.Location.CountryCode[i] : null;
//            var displayName = searchResponse.Location.DisplayName.Count > i ? searchResponse.Location.DisplayName[i] : null;
//            var adminDistrict = searchResponse.Location.AdminDistrict.Count > i ? searchResponse.Location.AdminDistrict[i] : null;
//            var adminDistrictCode = searchResponse.Location.AdminDistrictCode.Count > i ? searchResponse.Location.AdminDistrictCode[i] : null;
//            var iataCode = searchResponse.Location.IataCode.Count > i ? searchResponse.Location.IataCode[i] : null;
//            var icaoCode = searchResponse.Location.IcaoCode.Count > i ? searchResponse.Location.IcaoCode[i] : null;
//            var pwsId = searchResponse.Location.PwsId.Count > i ? searchResponse.Location.PwsId[i] : null;
//            var placeId = searchResponse.Location.PlaceId.Count > i ? searchResponse.Location.PlaceId[i] : null;
//            var postalKey = searchResponse.Location.PostalKey.Count > i ? searchResponse.Location.PostalKey[i] : null;
//            var type = searchResponse.Location.Type.Count > i ? searchResponse.Location.Type[i] : null;

//            // Проверяем, существует ли уже такая станция
//            var existingStation = await _dbContext.IBMMeteoStations
//                .FirstOrDefaultAsync(s => 
//                    s.Point.Y == lat && 
//                    s.Point.X == lon && 
//                    s.PlaceId == placeId);

//            if (existingStation != null)
//            {
//                continue; // Станция уже существует
//            }

//            // Определяем регион типа 3 для станции
//            var targetRegionId = await FindRegionForStationAsync(lat, lon);

//            // Создаем новую станцию
//            var newStation = new IBMMeteoStation
//            {
//                Id = Guid.NewGuid(),
//                RegionId = targetRegionId,
//                Point = new Point(lon, lat),
//                City = city,
//                Country = country,
//                CountryCode = countryCodeResult,
//                DisplayName = displayName,
//                AdminDistrict = adminDistrict,
//                AdminDistrictCode = adminDistrictCode,
//                IataCode = iataCode,
//                IcaoCode = icaoCode,
//                PwsId = pwsId,
//                PlaceId = placeId,
//                PostalKey = postalKey,
//                Type = type,
//                IsActive = true,
//                CreateAt = DateTime.UtcNow,
//                Status = Masofa.Common.Models.StatusType.Active,
//                LastUpdateAt = DateTime.UtcNow,
//                Names = new Masofa.Common.Models.LocalizationString { ValuesJson = displayName ?? "" }
//            };

//            _dbContext.IBMMeteoStations.Add(newStation);
//            newStationsCount++;
//        }

//        if (newStationsCount > 0)
//        {
//            await SaveChangesAsync();
//        }

//        return newStationsCount;
//    }

//    /// <summary>
//    /// Поиск региона типа 3 для станции по координатам
//    /// </summary>
//    private async Task<Guid?> FindRegionForStationAsync(double latitude, double longitude)
//    {
//        var stationPoint = new Point(longitude, latitude);

//        // Ищем регионы типа 3, в полигоне которых находится станция
//        var containingRegionQuery = @"
//            SELECT r.""Id""
//            FROM ""Regions"" r
//            INNER JOIN ""RegionMaps"" rm ON r.""Id"" = rm.""RegionId""
//            WHERE r.""Level"" = 3 
//            AND ST_Contains(rm.""Geometry"", ST_GeomFromText({0}, 4326))
//            LIMIT 1";

//        var containingRegion = await _dictionariesContext.Database
//            .SqlQueryRaw<Guid>(containingRegionQuery, stationPoint.AsText())
//            .FirstOrDefaultAsync();

//        if (containingRegion != Guid.Empty)
//        {
//            return containingRegion;
//        }

//        // Если не найден содержащий регион, ищем ближайший
//        var nearestRegionQuery = @"
//            SELECT r.""Id"", ST_Distance(rm.""Geometry"", ST_GeomFromText({0}, 4326)) as distance
//            FROM ""Regions"" r
//            INNER JOIN ""RegionMaps"" rm ON r.""Id"" = rm.""RegionId""
//            WHERE r.""Level"" = 3
//            ORDER BY distance
//            LIMIT 1";

//        var nearestRegion = await _dictionariesContext.Database
//            .SqlQueryRaw<Guid>(nearestRegionQuery, stationPoint.AsText())
//            .FirstOrDefaultAsync();

//        return nearestRegion != Guid.Empty ? nearestRegion : null;
//    }
//}
