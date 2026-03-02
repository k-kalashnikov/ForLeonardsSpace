namespace Masofa.Common.Models.Era
{
    public class Era5DayWeatherForecast : BaseEra5WeatherReport, IIsFrostDanger
    {
        public DateOnly Date { get; set; }
        public bool IsFrostDanger { get; set; }
    }
}
