using System.Collections.Generic;
using Newtonsoft.Json;

namespace Masofa.Common.Models.Identity
{
    /// <summary>
    /// DTO для группированных LockPermission по пользователям
    /// </summary>
    public class LockPermissionGroupedDto
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        [JsonProperty("user")]
        public UserInfo User { get; set; } = new UserInfo();

        /// <summary>
        /// Список разрешений для этого пользователя
        /// </summary>
        [JsonProperty("permissions")]
        public List<LockPermission> Permissions { get; set; } = new List<LockPermission>();

        /// <summary>
        /// Общее количество разрешений
        /// </summary>
        [JsonProperty("totalPermissions")]
        public int TotalPermissions { get; set; }
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email пользователя
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Ответ с группированными LockPermission
    /// </summary>
    public class GetLockPermissionGroupedResponse
    {
        /// <summary>
        /// Группированные данные
        /// </summary>
        [JsonProperty("data")]
        public List<LockPermissionGroupedDto> Data { get; set; } = new List<LockPermissionGroupedDto>();

        /// <summary>
        /// Общее количество записей
        /// </summary>
        [JsonProperty("totalRecords")]
        public int TotalRecords { get; set; }

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }
    }
}
