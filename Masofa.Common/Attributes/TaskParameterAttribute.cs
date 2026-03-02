namespace Masofa.Common.Attributes
{
    /// <summary>
    /// Универсальный атрибут для описания параметров команд, джобов и триггеров
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class TaskParameterAttribute : Attribute
    {
        /// <summary>
        /// Описание параметра
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Обязательный ли параметр
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Тип параметра (для отображения в UI)
        /// </summary>
        public string ParameterType { get; set; } = string.Empty;

        /// <summary>
        /// Пример значения
        /// </summary>
        public string? Example { get; set; }

        /// <summary>
        /// Минимальное значение (для числовых типов)
        /// </summary>
        public object? MinValue { get; set; }

        /// <summary>
        /// Максимальное значение (для числовых типов)
        /// </summary>
        public object? MaxValue { get; set; }

        /// <summary>
        /// Регулярное выражение для валидации (для строковых типов)
        /// </summary>
        public string? ValidationPattern { get; set; }

        /// <summary>
        /// Список допустимых значений (для enum или ограниченных типов)
        /// </summary>
        public string[]? AllowedValues { get; set; }

        public TaskParameterAttribute() { }

        public TaskParameterAttribute(string description)
        {
            Description = description;
        }

        public TaskParameterAttribute(string description, bool isRequired)
        {
            Description = description;
            IsRequired = isRequired;
        }

        public TaskParameterAttribute(string description, bool isRequired, string? defaultValue)
        {
            Description = description;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }

        public TaskParameterAttribute(string description, bool isRequired, string? defaultValue, string parameterType)
        {
            Description = description;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
            ParameterType = parameterType;
        }
    }
}
