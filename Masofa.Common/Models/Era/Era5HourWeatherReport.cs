namespace Masofa.Common.Models.Era
{
    public class Era5HourWeatherReport : BaseEra5WeatherReport
    {
        public DateOnly Date { get; set; }
        public int Hour { get; set; }
    }
}
