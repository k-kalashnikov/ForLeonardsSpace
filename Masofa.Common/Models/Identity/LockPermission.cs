using Masofa.Common.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Identity
{
    public class LockPermission : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string EntityTypeName { get; set; }

        public string? EntityAction { get; set; }

        public Guid? EntityId { get; set; }

        public LockPermissionType LockPermissionType { get; set; } = LockPermissionType.Entity;

        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Type? EntityType
        {
            get
            {
                return TypeHelper.GetTypeFromAllAssemblies(EntityTypeName);
            }
            set
            {
                EntityTypeName = value.FullName;
            }
        }
    }

    public enum LockPermissionType
    {
        Action = 0,
        Entity = 1,
        Instance = 2
    }

    public enum BaseActionType
    {
        Read,
        Create,
        Update,
        Delete,
    }

    public class LockPermissionHistory : BaseHistoryEntity<LockPermission> { }
}
