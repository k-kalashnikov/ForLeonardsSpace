using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.SystemCrical
{
    public class AccessMapItem : BaseEntity
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        [NotMapped]
        public Dictionary<string, bool> Roles { get; set; } = new Dictionary<string, bool>();

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string RolesJson
        {
            get => System.Text.Json.JsonSerializer.Serialize(Roles);
            set => Roles = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(value);
        }
    }
}
