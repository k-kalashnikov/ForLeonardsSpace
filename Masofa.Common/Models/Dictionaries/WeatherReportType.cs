namespace Masofa.Common.Models.Dictionaries
{

    public partial class WeatherReportType : BaseNamedEntity
    {
        public string? Css { get; set; }
    }

    public class WeatherReportTypeHistory : BaseHistoryEntity<WeatherReportType> { }
}