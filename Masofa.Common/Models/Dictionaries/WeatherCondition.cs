namespace Masofa.Common.Models.Dictionaries
{
    public partial class WeatherCondition : BaseNamedEntity
    {
        public Guid? ProviderId { get; set; }

        public int? ProviderCode { get; set; }
    }

    public class WeatherConditionHistory : BaseHistoryEntity<WeatherCondition> { }
}