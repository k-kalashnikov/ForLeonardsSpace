using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Masofa.Common.Models.Dictionaries
{
    public class RecommendedPlantingDates : BaseDictionaryItem
    {
        public Guid CropId { get; set; }
        public Guid RegionId { get; set; }

        [JsonIgnore]
        public string? PeriodsJson { get; set; }

        [NotMapped]
        [JsonProperty("periods")]
        public List<PlantingPeriod> Periods { get; set; } = new();
    }

    public class PlantingPeriod
    {
        [JsonProperty("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonProperty("seedingStartDate")]
        public DateTime SeedingStartDate { get; set; }

        [JsonProperty("seedingEndDate")]
        public DateTime SeedingEndDate { get; set; }

        [JsonProperty("harvestingStartDate")]
        public DateTime HarvestingStartDate { get; set; }

        [JsonProperty("harvestingEndDate")]
        public DateTime HarvestingEndDate { get; set; }
    }

    public class RecommendedPlantingDatesHistory : BaseHistoryEntity<RecommendedPlantingDates> { }
}