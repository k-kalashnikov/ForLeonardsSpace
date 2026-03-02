namespace Masofa.Common.Models.Era
{
    public class Era5WeekWeatherReport : BaseEra5WeatherReport
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
    }
}
