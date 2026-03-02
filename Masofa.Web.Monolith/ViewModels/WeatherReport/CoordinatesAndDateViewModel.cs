using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.ViewModels.WeatherReport
{
    public class CoordinatesAndDateViewModel
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public required double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public required double Longitude { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        [Required]
        public required DateTime InputDate { get; set; }
    }

    public class ListCoordinatesViewModel
    {
        /// <summary>
        /// Широта
        /// </summary>
        [Required]
        public required double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [Required]
        public required double Longitude { get; set; }


    }
}
