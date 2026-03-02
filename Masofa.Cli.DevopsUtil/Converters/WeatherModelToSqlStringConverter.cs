using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Masofa.Cli.DevopsUtil.Converters
{
    public static class WeatherModelToSqlStringConverter
    {
        public static Dictionary<DateTime, (string, int)> Convert(List<Masofa.Common.Models.Weather.WeatherStationsDatum?> items)
        {
            var groupedNewItems = items.GroupBy(m => m.Date).ToDictionary(m => m.Key, m => m.ToList());
            var newType = typeof(Masofa.Common.Models.Weather.WeatherStationsDatum);
            var sqlStrs = new Dictionary<DateTime, (string, int)>();
            foreach (var itemGroup in groupedNewItems)
            {
                if (itemGroup.Key == null)
                {
                    continue;
                }
                var tableNameBase = newType.GetCustomAttribute<TableAttribute>()?.Name ?? newType.Name;
                var partitionTableName = $"{tableNameBase}_{((DateTime)(itemGroup.Key)):yyyy_MM_dd}";
                var sb = new StringBuilder();
                sb.AppendLine($"INSERT INTO \"{partitionTableName}\"");
                sb.AppendLine("(");
                sb.AppendLine("\"Id\",");
                sb.AppendLine("\"WeatherStationId\",");
                sb.AppendLine("\"Date\",");
                sb.AppendLine("\"Temperature\",");
                sb.AppendLine("\"TemperatureMax\",");
                sb.AppendLine("\"TemperatureMin\",");
                sb.AppendLine("\"Precipitation\",");
                sb.AppendLine("\"WindSpeed\",");
                sb.AppendLine("\"WindSpeedMin\",");
                sb.AppendLine("\"WindSpeedMax\",");
                sb.AppendLine("\"WindDirection\",");
                sb.AppendLine("\"Windchill\",");
                sb.AppendLine("\"CloudCover\",");
                sb.AppendLine("\"RelativeHumidity\",");
                sb.AppendLine("\"ConditionCode\",");
                sb.AppendLine("\"SolarRadiation\",");
                sb.AppendLine("\"DewPoint\",");
                sb.AppendLine("\"HumidityMin\",");
                sb.AppendLine("\"HumidityMax\"");
                sb.AppendLine(")");
                sb.AppendLine("VALUES");
                sb.AppendLine();

                foreach (var itemValue in itemGroup.Value) 
                {
                    if (itemValue == null)
                    {
                        continue;
                    }
                    sb.Append("(");
                    sb.Append($"'{Guid.NewGuid()}',");
                    sb.Append($"'{(string.IsNullOrEmpty(itemValue?.WeatherStationId.ToString()) ? "NULL" : itemValue?.WeatherStationId.ToString())}',");
                    sb.Append($"'{(itemValue?.Date.ToString("yyyy-MM-dd HH:mm:ss zzz"))}',");
                    sb.Append($"{((itemValue?.Temperature != null) ? itemValue?.Temperature.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.TemperatureMax != null) ? itemValue?.TemperatureMax.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.TemperatureMin != null) ? itemValue?.TemperatureMin.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.Precipitation != null) ? itemValue?.Precipitation.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.WindSpeed != null) ? itemValue?.WindSpeed.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.WindSpeedMin != null) ? itemValue?.WindSpeedMin.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.WindSpeedMax != null) ? itemValue?.WindSpeedMax.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.WindDirection != null) ? itemValue?.WindDirection.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.Windchill != null) ? itemValue?.Windchill.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.CloudCover != null) ? itemValue?.CloudCover.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.RelativeHumidity != null) ? itemValue?.RelativeHumidity.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.ConditionCode != null) ? itemValue?.ConditionCode.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.SolarRadiation != null) ? itemValue?.SolarRadiation.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.DewPoint != null) ? itemValue?.DewPoint.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.HumidityMin != null) ? itemValue?.HumidityMin.ToString() : "NULL")},");
                    sb.Append($"{((itemValue?.HumidityMax != null) ? itemValue?.HumidityMax.ToString() : "NULL")}");

                    if (itemGroup.Value.Last() == itemValue)
                    {

                        sb.Append(");");
                        sb.AppendLine();
                        continue;
                    }
                    sb.Append("),");
                    sb.AppendLine();
                }

                var sql = sb.ToString();
                sqlStrs.Add(itemGroup.Key, (sql, itemGroup.Value.Count));
            }

            return sqlStrs;
        }

        private static string ResolveToSqlString(PropertyInfo prop, object item)
        {
            if (prop.PropertyType == typeof(DateTime))
            {
                return ((DateTime)(prop.GetValue(item))).ToString("yyyy-MM-dd HH:mm:ss zzz");

            }
            return prop.GetValue(item)?.ToString() ?? "NULL";

        }
    }
}
