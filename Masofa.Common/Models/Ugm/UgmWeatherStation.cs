using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Ugm
{
    public class UgmWeatherStation : BaseEntity
    {
        /// <summary>
        /// Id по УГМ
        /// </summary>
        public int UgmRegionId { get; set; }

        /// <summary>
        /// Идентификатор агроклиматической зоны
        /// </summary>
        public Guid? AgroclimaticZoneId { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Является ли региональным центром
        /// </summary>
        public bool? IsRegionalCenter { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Наименование (рус?)
        /// </summary>
        public string? Title { get; set; }

        [NotMapped]
        public string BoundingCirclePointJson => CalculateCircleEdge();

        private string CalculateCircleEdge()
        {
            if (Latitude != null && Longitude != null)
            {
                var (newLat, newLon) = GetPointAtDistance(Latitude.Value, Longitude.Value, 10.0, 0);
                var result = new Point(newLon, newLat) { SRID = 4326 };
                return result.AsText();
            }

            return string.Empty;
        }

        private static (double lat, double lon) GetPointAtDistance(double lat, double lon, double distanceKm, double bearingDegrees)
        {
            double latRad = DegreesToRadians(lat);
            double lonRad = DegreesToRadians(lon);
            double bearingRad = DegreesToRadians(bearingDegrees);

            double angularDistance = distanceKm / 6371.0;

            double newLatRad = Math.Asin(
                Math.Sin(latRad) * Math.Cos(angularDistance) +
                Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad)
            );

            double newLonRad = lonRad + Math.Atan2(
                Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(latRad),
                Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(newLatRad)
            );

            return (RadiansToDegrees(newLatRad), RadiansToDegrees(newLonRad));
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
        private static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;
    }
}
