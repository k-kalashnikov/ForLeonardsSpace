//using Masofa.Common.Models;
//using Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;
//using NetTopologySuite.Geometries;
//using NodaTime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Converters
//{
//    public class WeatherConverter
//    {
//        public static Common.Models.Weather.Alert Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Alert source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.Alert();

//            target.ProviderId = source.ProviderId;
//            target.AgroClimaticZonesId = source.AgroClimaticZonesId;
//            target.Value = source.Value;
//            target.RegionId = source.RegionId;
//            target.TypeId = source.TypeId;
//            target.Date = ResolveDateTime(source.Date);

//            return target;
//        }

//        public static Common.Models.Weather.Job Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Job source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.Job();

//            target.ProviderId = source.ProviderId;
//            target.Application = source.Application;
//            target.Action = source.Action;
//            target.Date = ResolveDateTime((LocalDateTime)source.Date);
//            target.Result = source.Result;
//            target.Path = source.Path;
//            target.JobStatusId = source.JobStatusId;
            
//            return target;
//        }

//        public static Common.Models.Weather.Log Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Log source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.Log();

//            target.ProviderId = source.ProviderId;
//            target.JobStatus = source.JobStatus;
//            target.Date = ResolveDateTime((LocalDateTime)source.Date);
//            target.JobId = source.JobId;
//            target.Details = source.Details;
//            target.ContentSize = source.ContentSize;
//            target.UserInfo = source.UserInfo;
            
//            return target;
//        }

//        public static Common.Models.Weather.Region Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Region source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.Region();

//            target.RegionLevel = source.RegionLevel;
//            target.UpdateDate = ResolveDateTime(source.UpdateDate);
//            target.RegionNameRu = source.RegionName;
//            target.RegionNameEn = source.RegionNameEn;
//            target.Active = source.Active;
//            target.Iso = source.Iso;
//            target.PolygonGeom = source.PolygonGeom;
//            target.Lat = source.Lat;
//            target.Lon = source.Lon;
//            target.RowX = source.RowX;
//            target.ColumnY = source.ColumnY;
//            target.Polygon = source.Polygon;
//            target.ParentId = source.ParentId;
//            target.Active = source.Active;
//            target.Mhobt = source.Mhobt;
            
//            return target;
//        }
//        public static Common.Models.Weather.Report Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Report source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.Report();

//            target.SourceQuery = source.SourceQuery;
//            target.ReportType = source.ReportType;
//            target.Name = source?.Name ?? string.Empty;
//            target.Link = source.Link;
//            target.Description = source.Description;
//            target.UpdateDate = ResolveDateTime((LocalDate)source.UpdateDate);
//            return target;
//        }

//        public static Common.Models.Weather.RegionsWeather Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsWeather1 source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.RegionsWeather();

//            target.Date = ResolveDateTime(source.Date);
//            target.RegionId = source.RegionId;
//            target.ProviderId = source.ProviderId;
//            target.Temp = source.Temp;
//            target.Precipitation = source.Precipitation;
//            target.TempDev = source.TempDev;
//            target.PrecDev = source.PrecDev;

//            return target;
//        }

//        public static Common.Models.Weather.AgroClimaticZoneMonthNorm Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZoneMonthNorm source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.AgroClimaticZoneMonthNorm();

//            target.ProviderId = source.ProviderId;
//            target.PrecipitationAvgNorm = source.PrecipitationAvgNorm;
//            target.PrecipitationMedNorm = source.PrecipitationMedNorm;
//            target.AgroClimaticZoneId = source.AgroClimaticZoneId;
//            target.M = source.M;
//            target.SolarRadiationMedNorm = source.SolarRadiationMedNorm;
//            target.SolarRadiationAvgNorm = source.SolarRadiationAvgNorm;
//            target.TemperatureMedNorm = source.TemperatureMedNorm;
//            target.TemperatureAvgNorm = source.TemperatureAvgNorm;

//            return target;
//        }

//        public static Common.Models.Weather.AgroClimaticZoneNorm Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZoneNorm source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.AgroClimaticZoneNorm();

//            target.ProviderId = source.ProviderId;
//            target.PrecipitationAvgNorm = source.PrecipitationAvgNorm;
//            target.PrecipitationMedNorm = source.PrecipitationMedNorm;
//            target.AgroClimaticZoneId = source.AgroClimaticZoneId;
//            target.M = source.M;
//            target.D = source.D;
//            target.SolarRadiationMedNorm = source.SolarRadiationMedNorm;
//            target.SolarRadiationAvgNorm = source.SolarRadiationAvgNorm;
//            target.TemperatureMedNorm = source.TemperatureMedNorm;
//            target.TemperatureAvgNorm = source.TemperatureAvgNorm;

//            return target;
//        }

//        public static Common.Models.Weather.AgroClimaticZonesWeatherRate Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZonesWeatherRate source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.AgroClimaticZonesWeatherRate();

//            target.Date = ResolveDateTime((LocalDate)source.Date);
//            target.TempRate = source.TempRate;
//            target.PrecRate = source.PrecRate;

//            return target;
//        }

//        public static Common.Models.Weather.ApplicationProperty Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.ApplicationProperty source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.ApplicationProperty();

//            target.Application = source.Application;
//            target.Active = source.Active;
//            target.Value = source.Value;
//            target.Key = source.Key;
//            target.Description = source.Description;

//            return target;
//        }

//        public static Common.Models.Weather.RegionsAgroClimaticZone Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsAgroClimaticZone source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.RegionsAgroClimaticZone();

//            target.AgroClimaticZonesId = source.AgroClimaticZonesId;
//            target.RegionId = source.RegionId;

//            return target;
//        }

//        public static Common.Models.Weather.RegionsDump Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsDump source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.RegionsDump();

//            target.RegionLevel = source.RegionLevel;
//            target.RegionName = source.RegionName;
//            target.Iso = source.Iso;
//            target.PolygonGeom = source.PolygonGeom;
//            target.Active = source.Active;
//            target.ColumnY = source.ColumnY;
//            target.Lat = source.Lat;
//            target.Lon = source.Lon;
//            target.RowX = source.RowX;
//            target.Mhobt = source.Mhobt;
//            target.ParentId = source.ParentId;
//            target.Polygon = source.Polygon;

//            return target;
//        }

//        public static Common.Models.Weather.WeatherStationAgroClimaticZone Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationAgroClimaticZone source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.WeatherStationAgroClimaticZone();

//            target.AgroClimaticZonesId = source.AgroClimaticZonesId;
//            target.WeatherStationId = source.WeatherStationId;

//            return target;
//        }

//        public static Common.Models.Weather.WeatherStationsDataEx Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationsDataEx source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.WeatherStationsDataEx();

//            target.TemperatureSoil = source.TemperatureSoil;
//            target.Temperature2mUnder = source.Temperature2mUnder;
//            target.TemperatureGroundLevel = source.TemperatureGroundLevel;
//            target.Temperature1mAbove = source.Temperature1mAbove;
//            target.Temp1030cm = source.Temp1030cm;
//            target.Temp30100cm = source.Temp30100cm;
//            target.HumiditySoil2m = source.HumiditySoil2m;
//            target.HumiditySoil50cm = source.HumiditySoil50cm;
//            target.Temp10cmUnder = source.Temp10cmUnder;

//            return target;
//        }

//        public static Common.Models.Weather.WeatherStationsDatum Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationsDatum source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.WeatherStationsDatum();

//            target.Temperature = source.Temperature;
//            target.TemperatureMin = source.TemperatureMin;
//            target.TemperatureMax = source.TemperatureMax;
//            target.HumidityMax = source.HumidityMax;
//            target.HumidityMin = source.HumidityMin;
//            target.CloudCover = source.CloudCover;
//            target.ConditionCode = source.ConditionCode;
//            target.DewPoint = source.DewPoint;
//            target.Precipitation = source.Precipitation;
//            target.RelativeHumidity = source.RelativeHumidity;
//            target.SolarRadiation = source.SolarRadiation;
//            target.WindDirection = source.WindDirection;
//            target.Windchill = source.Windchill;
//            target.WeatherStationId = source.WeatherStationId;
//            target.WindSpeed = source.WindSpeed;
//            target.WindSpeedMin = source.WindSpeedMin;
//            target.WindSpeedMax = source.WindSpeedMax;
//            target.Date = ResolveDateTime((LocalDateTime)source.Date);

//            return target;
//        }

//        public static Common.Models.Weather.XslsUzUnputColumn Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.XslsUzUnputColumn source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.XslsUzUnputColumn();

//            target.XlsColumnName = source.XlsColumnName;
//            target.DbColumnName = source.DbColumnName;
//            target.DbTableName = source.DbTableName;

//            return target;
//        }

//        public static Common.Models.Weather.AgroClimaticZone Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZone source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.AgroClimaticZone();

//            target.Polygon = source.Polygon;
//            target.PolygonGeom = source.PolygonGeom;
//            target.Active = source.Active;
//            target.NameEn = source?.NameEn ?? string.Empty;
//            target.Name = source?.Name ?? string.Empty;
//            target.NameUz = source?.NameUz ?? string.Empty;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherAlertType Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AlertType source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherAlertType();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.NameEn = source?.NameEn ?? string.Empty;
//            target.Type = source.Type;
//            target.DescriptionRu = source.Description;
//            target.DescriptionEn = source.DescriptionEn;
//            target.FieldName = source.FieldName;
//            target.Type = source.Type;
//            target.Value = source.Value;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherCondition Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Condition source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherCondition();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.ProviderCode = source.ProviderCode;
//            target.ProviderId = source.ProviderId;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherFrequency Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Frequency source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherFrequency();

//            target.NameRu = source?.Name ?? string.Empty;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherImageType Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.ImageType source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherImageType();

//            target.Code = source.Code;
//            target.NameRu = source?.Name ?? string.Empty;
//            target.NameEn = source?.NameEn ?? string.Empty;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherJobStatus Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.JobStatus source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherJobStatus();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.NameEn = source?.NameEn ?? string.Empty;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherProvider Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.Provider source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherProvider();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.Editable = source.Editable;
//            target.FrequencyId = source.FrequencyId;
//            target.Z = source.Z;
//            target.Status = source.Active == true 
//                ? StatusType.Active 
//                : StatusType.Hiden; ;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherReportType Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.ReportType source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherReportType();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.NameEn = source?.NameEn ?? string.Empty;
//            target.Css = source.Css;

//            return target;
//        }

//        public static Common.Models.Weather.WeatherStation Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStation source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Weather.WeatherStation();

//            target.Application = source.Application;
//            target.Name = source?.Name ?? string.Empty;
//            target.ProviderId = source.ProviderId;
//            target.Code = source.Code;
//            target.Lat = source.Lat;
//            target.Lon = source.Lon;
//            target.X = source.X;
//            target.Y = source.Y;

//            return target;
//        }

//        public static Common.Models.Dictionaries.WeatherType Convert(Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherType source)
//        {
//            if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherType();

//            target.NameRu = source?.Name ?? string.Empty;
//            target.NameEn = source?.NameEn ?? string.Empty;
//            target.TiledMap = source.TiledMap;
//            target.Gis = source.Gis;
//            target.Code = source.Code;

//            return target;
//        }

//        #region Resolvers
//        private static DateTime ResolveDateTime(LocalDateTime localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
//                 .AddHours(localDateTime.Hour)
//                 .AddMinutes(localDateTime.Minute)
//                 .ToUniversalTime();
//        }

//        private static DateTime ResolveDateTime(LocalDate localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
//        }
//        #endregion
//    }
//}
