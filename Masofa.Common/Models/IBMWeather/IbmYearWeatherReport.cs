using Masofa.Common.Models.Era;

namespace Masofa.Common.Models.IBMWeather
{
    public class IbmYearWeatherReport : BaseIbmWeatherReport, IIsFrostDanger
    {
        public DateOnly Date { get; set; }
        public bool IsFrostDanger { get; set; }
    }
}
