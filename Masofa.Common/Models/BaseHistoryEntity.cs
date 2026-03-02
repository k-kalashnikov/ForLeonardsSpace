using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models
{
    public class BaseHistoryEntity<TModel> : BaseEntity
    {
        public Guid OwnerId { get; set; }

        [NotMapped]
        public TModel? NewModel { get; set; }

        [NotMapped]
        public TModel? OldModel { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? NewModelJson
        {
            get
            {
                return NewModel == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(NewModel);
            }
            set
            {
                if (value != null)
                {
                    NewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(value);
                }
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? OldModelJson
        {
            get
            {
                return OldModel == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(OldModel);
            }
            set
            {
                if (value != null)
                {
                    OldModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(value);
                }
            }
        }
    }
}