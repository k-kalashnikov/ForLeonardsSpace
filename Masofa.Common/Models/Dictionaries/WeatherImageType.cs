namespace Masofa.Common.Models.Dictionaries
{
    public partial class WeatherImageType : BaseNamedEntity
    {
        public int? Code { get; set; }
    }

    public class WeatherImageTypeHistory : BaseHistoryEntity<WeatherImageType> { }
}