using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Masofa.Common.Models.Identity
{
    /// <summary>
    /// DTO для сохранения блокировок пользователя
    /// </summary>
    public class SaveUserLockPermissionsDto
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; } = Guid.Empty;

        /// <summary>
        /// Список блокировок сущностей
        /// </summary>
        [JsonProperty("entityPermissions")]
        public List<EntityLockPermissionDto> EntityPermissions { get; set; } = new();
    }

    /// <summary>
    /// DTO для блокировки сущности
    /// </summary>
    public class EntityLockPermissionDto
    {
        /// <summary>
        /// Полное имя типа сущности
        /// </summary>
        [Required]
        [JsonProperty("entityTypeName")]
        public string EntityTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Тип блокировки (0 - Action, 1 - Entity, 2 - Instance)
        /// </summary>
        [JsonProperty("lockPermissionType")]
        public int LockPermissionType { get; set; }

        /// <summary>
        /// Заблокирована ли вся сущность
        /// </summary>
        [JsonProperty("entityBlocked")]
        public bool EntityBlocked { get; set; }

        /// <summary>
        /// Заблокированы ли конкретные экземпляры
        /// </summary>
        [JsonProperty("instanceBlocked")]
        public bool InstanceBlocked { get; set; }

        /// <summary>
        /// Список заблокированных действий
        /// </summary>
        [JsonProperty("blockedActions")]
        public List<string> BlockedActions { get; set; } = new();
    }
}
