using Masofa.Common.Models.Era;

namespace Masofa.Common.Models.IBMWeather
{
    public class IbmDayWeatherReport : BaseIbmWeatherReport, IIsFrostDanger
    {
        public DateOnly Date { get; set; }
        public bool IsFrostDanger { get; set; }
    }
}
