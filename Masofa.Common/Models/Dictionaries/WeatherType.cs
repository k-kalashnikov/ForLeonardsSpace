namespace Masofa.Common.Models.Dictionaries
{

    public partial class WeatherType : BaseNamedEntity
    {
        public int? Code { get; set; }

        public bool Gis { get; set; }

        public bool TiledMap { get; set; }
    }

    public class WeatherTypeHistory : BaseHistoryEntity<WeatherType> { }
}