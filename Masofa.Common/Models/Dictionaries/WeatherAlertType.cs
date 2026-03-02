namespace Masofa.Common.Models.Dictionaries
{
    public partial class WeatherAlertType : BaseNamedEntity
    {
        public int? Type { get; set; }
        public double? Value { get; set; }
        public LocalizationString Descriptions { get; set; } = new LocalizationString();
        public string? FieldName { get; set; }
    }

    public class WeatherAlertTypeHistory : BaseHistoryEntity<WeatherAlertType> { }
}