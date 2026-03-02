using System.Collections.Generic;

namespace Masofa.Common.Models.SystemMetadata
{
    /// <summary>
    /// DTO для получения метаданных системы
    /// </summary>
    public class SystemMetadataDto
    {
        /// <summary>
        /// Простой список всех сущностей в системе
        /// </summary>
        public List<EntityInfo> Entities { get; set; } = new List<EntityInfo>();

        /// <summary>
        /// Простой список всех доступных действий
        /// </summary>
        public List<ActionInfo> Actions { get; set; } = new List<ActionInfo>();

        /// <summary>
        /// Список типов разрешений
        /// </summary>
        public List<PermissionTypeInfo> PermissionTypes { get; set; } = new List<PermissionTypeInfo>();
    }

    /// <summary>
    /// Информация о сущности
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// Полное имя типа сущности
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Короткое имя сущности
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Название для отображения
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Можно ли блокировать эту сущность
        /// </summary>
        public bool CanBeLocked { get; set; } = true;
    }

    /// <summary>
    /// Информация о действии
    /// </summary>
    public class ActionInfo
    {
        /// <summary>
        /// Название действия
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое название действия
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Информация о типе разрешения
    /// </summary>
    public class PermissionTypeInfo
    {
        /// <summary>
        /// Название типа разрешения
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое название типа разрешения
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Описание типа разрешения
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Требует ли тип разрешения указания действия
        /// </summary>
        public bool RequiresAction { get; set; } = false;

        /// <summary>
        /// Требует ли тип разрешения указания EntityId
        /// </summary>
        public bool RequiresEntityId { get; set; } = false;

        /// <summary>
        /// Числовое значение типа разрешения
        /// </summary>
        public int Value { get; set; }
    }
}
