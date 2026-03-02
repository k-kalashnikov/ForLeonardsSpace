using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Masofa.Common.Models.CropMonitoring
{
    public class SoilData : BaseEntity
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public required Point Point { get; set; }
        public string Parameter { get; set; }  // sand, clay, phh2o и т.д.
        public string DepthRange { get; set; } // "0-5cm", "0-30cm" и т.п.
        public double? Value { get; set; }     // значение (может быть null, если NoData)
        public string Unit { get; set; }       // "g/kg", "pH", "kg/m³" и т.д.
        public string Source { get; set; }
        public string TileKey
        {
            get
            {
                const double step = 0.25;
                const double eps = 1e-9;

                double Snap(double v)
                {
                    var bucket = Math.Floor((v + eps) / step) * step;
                    return Math.Round(bucket, 2, MidpointRounding.AwayFromZero);
                }

                double lon = Snap(Point.X);
                double lat = Snap(Point.Y);

                // "G29" — обрезает хвостовые нули, но сохраняет десятичку при необходимости
                string lonStr = lon.ToString("G29", CultureInfo.InvariantCulture);
                string latStr = lat.ToString("G29", CultureInfo.InvariantCulture);

                return $"x_{lonStr}_y_{latStr}";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var parts = value.Split('_');
                    if (parts.Length >= 4)
                    {
                        TileKeyX = $"x_{parts[1]}";
                        TileKeyY = $"y_{parts[3]}";
                    }
                }
            }
        }

        [NotMapped]
        public string TileKeyX { get; set; }

        [NotMapped]
        public string TileKeyY { get; set; }

        [NotMapped]
        public string? PointJson
        {
            get
            {
                var point = Point;
                if (point == null || point.IsEmpty)
                    return null;

                return point.AsText();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Point = null;
                }
                else
                {
                    try
                    {
                        var reader = new NetTopologySuite.IO.WKTReader();
                        var geometry = reader.Read(value);
                        if (geometry is Point polygon)
                        {
                            Point = polygon;
                        }
                        else
                        {
                            Point = null;
                        }
                    }
                    catch
                    {
                        Point = null;
                    }
                }
            }
        }
    }

    public class SoilDataHistory : BaseHistoryEntity<SoilData> { }
}
