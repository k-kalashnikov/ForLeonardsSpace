using System.ComponentModel.DataAnnotations;

namespace Masofa.Common.Models.Tiles
{
    public class TileLayer : BaseNamedEntity
    {
        /// <summary>
        ///  Имя слоя на ГеоСервере => <ИточникДанных><ТипДанных><ПогодныйПоказатель><Дата>
        ///  ИсточникДанных: ERA, IBM, UGM
        ///  ТипДанных: Normalized, Deviation, Weather
        ///  ПогодныйПоказатель: TemperatureMin, TemperatureMax, TemperatureAverage, SolarRadiationInfluence, Humidity, Fallout
        ///  Дата: YYYYMMDD
        /// </summary>
        [Required]
        public string GeoServerName { get; set; }


        public string GeoServerRelationPath { get; set; }
    }

    public class TileLayerConfigItem
    {
        public string ApiName { get; set; }
        public string EngName { get; set; }
        public string RuName { get; set; }
        public string UzLatName { get; set; }

        public TileLayerConfigItem(string apiName, string englishName, string russianName, string uzbekLatinName)
        {
            ApiName = apiName;
            EngName = englishName;
            RuName = russianName;
            UzLatName = uzbekLatinName;
        }
    }

    public static class TileLayerConfig
    {
        public static readonly List<TileLayerConfigItem> Layers = new List<TileLayerConfigItem>
        {
            new("FrostDanger", "Frost Danger", "Морозоопасность", "Sovuq xavfi"),
            new("Fallout", "Precipitation", "Осадки", "Yog'ingarchilik"),
            new("Humidity", "Humidity", "Влажность", "Namlik"),
            new("SolarRadiationInfluence", "Solar Radiation", "Солнечная радиация", "Quyosh nurlanishi"),
            new("TemperatureAverage", "Temperature Average", "Средняя температура", "O'rtacha harorat"),
            new("TemperatureMax", "Temperature Max", "Максимальная температура", "Maksimal harorat"),
            new("TemperatureMaxTotal", "Temperature Max Total", "Максимальная общая температура", "Maksimal harorat jami"),
            new("TemperatureMin", "Temperature Min", "Минимальная температура", "Harorat minimal"),
            new("TemperatureMinTotal", "Temperature Min Total", "Минимальная общая температура", "Haroratning minimal jami"),
            new("NormalizedFallout", "Norm of Precipitation", "Норма осадков", "Yog'ingarchilik normasi"),
            new("NormalizedHumidity", "Norm of Humidity", "Норма влажности", "Namlik normasi"),
            new("NormalizedSolarRadiationInfluence", "Norm of Solar Radiation", "Норма солнечной радиации", "Quyosh nurlanishining normasi"),
            new("NormalizedTemperatureAverage", "Norm of Temperature Average", "Норма средней температуры", "Harorat o'rtacha normasi"),
            new("NormalizedTemperatureMax", "Norm of Temperature Max", "Норма максимальной температуры", "Maksimal harorat normasi"),
            new("NormalizedTemperatureMaxTotal", "Norm of Temperature Max Total", "Норма максимальной общей температуры", "Maksimal harorat normasi Jami"),
            new("NormalizedTemperatureMin", "Norm of Temperature Min", "Норма минимальной температуры", "Harorat normasi Min"),
            new("NormalizedTemperatureMinTotal", "Norm of Temperature Min Total", "Норма минимальной общей температуры", "Harorat normasi Minimal jami"),
            new("DeviationTemperatureAverage", "Deviation of Temperature Average from Norm", "Отклонение средней температуры от нормы", "Harorat o'rtacha ko'rsatkichining normadan og'ishi"),
            new("DeviationTemperatureMax", "Deviation of Temperature Max from Norm", "Отклонение максимальной температуры от нормы", "Maksimal haroratning normadan og'ishi"),
            new("DeviationTemperatureMaxTotal", "Deviation of Temperature Max Total from Norm", "Отклонение максимальной общей температуры от нормы", "Haroratning maksimal umumiy qiymatining normadan og'ishi"),
            new("DeviationTemperatureMin", "Deviation of Temperature Min from Norm", "Отклонение минимальной температуры от нормы", "Haroratning minimal ko'rsatkichining normadan og'ishi"),
            new("DeviationTemperatureMinTotal", "Deviation of Temperature Min Total from Norm", "Отклонение минимальной общей температуры от нормы", "Haroratning minimal umumiy qiymatining normadan og'ishi"),
            new("ARVI", "ARVI", "ARVI", "ARVI"),
            new("EVI", "EVI", "EVI", "EVI"),
            new("GNDVI", "GNDVI", "GNDVI", "GNDVI"),
            new("MNDWI", "MNDWI", "MNDWI", "MNDWI"),
            new("NDMI", "NDMI", "NDMI", "NDMI"),
            new("NDVI", "NDVI", "NDVI", "NDVI"),
            new("ORVI", "ORVI", "ORVI", "ORVI"),
            new("OSAVI", "OSAVI", "OSAVI", "OSAVI")
        };
    }
}
