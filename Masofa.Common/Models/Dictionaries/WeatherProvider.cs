namespace Masofa.Common.Models.Dictionaries
{

    public partial class WeatherProvider : BaseNamedEntity
    {
        public double? Z { get; set; }

        public int? FrequencyId { get; set; }

        public bool? Editable { get; set; }
    }

    public class WeatherProviderHistory : BaseHistoryEntity<WeatherProvider> { }
}