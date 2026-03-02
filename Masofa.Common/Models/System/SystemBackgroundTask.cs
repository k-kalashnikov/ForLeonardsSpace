using Masofa.Common.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    /// <summary>  
    /// Фоновая задача системы  
    /// </summary>  
    public class SystemBackgroundTask : BaseNamedEntity
    {
        /// <summary>  
        /// Тип задачи  
        /// </summary>  
        public SystemBackgroundTaskType TaskType { get; set; }
        public LocalizationString Names { get; set; }

        /// <summary>  
        /// Полное имя типа класса для выполнения  
        /// </summary>  
        public string ExecuteTypeName { get; set; } = null!;

        /// <summary>  
        /// Системный тип класса для выполнения (не сохраняется в БД)  
        /// </summary>  
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Type? ExecuteType
        {
            get => TypeHelper.GetTypeFromAllAssemblies(ExecuteTypeName);
        }

        /// <summary>  
        /// Активна ли задача  
        /// </summary>  
        public bool IsActive { get; set; } = true;

        /// <summary>  
        /// Максимальное количество выполнений (-1 = бесконечно)  
        /// </summary>  
        public int MaxExecutions { get; set; } = -1;

        /// <summary>  
        /// Сколько раз уже выполнилась  
        /// </summary>  
        public int ExecutionCount { get; set; } = 0;

        /// <summary>  
        /// Можно ли повторять при ошибке  
        /// </summary>  
        public bool IsRetryable { get; set; } = false;

        /// <summary>  
        /// Максимальное количество повторов при ошибке  
        /// </summary>  
        public int MaxRetryCount { get; set; } = 0;

        /// <summary>  
        /// Текущее количество повторов  
        /// </summary>  
        public int CurrentRetryCount { get; set; } = 0;

        /// <summary>  
        /// JSON с опциями задачи (расписание, retry и т.д.)  
        /// </summary>
        public string TaskOptionJson {
            get
            {
                switch (TaskType)
                {
                    case SystemBackgroundTaskType.Condition: return Newtonsoft.Json.JsonConvert.SerializeObject(ConditionTaskOptions);
                    case SystemBackgroundTaskType.Command: return Newtonsoft.Json.JsonConvert.SerializeObject(CommandTaskOptions);
                    case SystemBackgroundTaskType.Schedule: return Newtonsoft.Json.JsonConvert.SerializeObject(ScheduleTaskOptions);
                    default: return string.Empty;
                }
            }
            set
            {
                switch (TaskType)
                {
                    case SystemBackgroundTaskType.Condition: ConditionTaskOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<ConditionTaskOptions>(value) ?? new ConditionTaskOptions(); break;
                    case SystemBackgroundTaskType.Command: CommandTaskOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandTaskOptions>(value) ?? new CommandTaskOptions(); break;
                    case SystemBackgroundTaskType.Schedule: ScheduleTaskOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<ScheduleTaskOptions>(value) ?? new ScheduleTaskOptions(); break;
                    default:
                        break;
                }
            }
        }

        /// <summary>  
        /// Тип объекта для десериализации опций (не сохраняется в БД)  
        /// </summary>  
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Type? TaskOptionsJsonType
        {
            get
            {
                switch (TaskType)
                {
                    case SystemBackgroundTaskType.Condition: return typeof(ConditionTaskOptions);
                    case SystemBackgroundTaskType.Command: return typeof(CommandTaskOptions);
                    case SystemBackgroundTaskType.Schedule: return typeof(ScheduleTaskOptions);
                    default: return null;
                }
            }
        }

        [NotMapped]
        /// <summary>  
        /// Это поле нужно только для того чтобы десериализовать опции задачи после получения из БД, в запросах его использовать нельзя  
        /// </summary>   
        public ScheduleTaskOptions? ScheduleTaskOptions { get; set; } = new ScheduleTaskOptions();

        [NotMapped]
        /// <summary>  
        /// Это поле нужно только для того чтобы десериализовать опции задачи после получения из БД, в запросах его использовать нельзя  
        /// </summary>  
        public CommandTaskOptions? CommandTaskOptions { get; set; } = new CommandTaskOptions();

        [NotMapped]
        /// <summary>  
        /// Это поле нужно только для того чтобы десериализовать опции задачи после получения из БД, в запросах его использовать нельзя  
        /// </summary>  
        public ConditionTaskOptions? ConditionTaskOptions { get; set; } = new ConditionTaskOptions();

        /// <summary>  
        /// JSON с входными параметрами  
        /// </summary>  
        public string ParametrsJson { get; set; } = string.Empty;

        /// <summary>  
        /// Тип объекта для десериализации входных параметров (не сохраняется в БД)  
        /// </summary>  
        public string ParametrsTypeName { get; set; } = string.Empty;

        /// <summary>  
        /// Результаты выполнения задачи  
        /// </summary> 
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<SystemBackgroundTaskResult> Results { get; set; } = new List<SystemBackgroundTaskResult>();
    }

    /// <summary>
    /// Типы фоновых задач
    /// </summary>
    public enum SystemBackgroundTaskType
    {
        /// <summary>
        /// Для Quartz - выполнение по расписанию
        /// </summary>
        Schedule = 0,

        /// <summary>
        /// Для CLI команд из DevOps Utils
        /// </summary>
        Command = 1,

        /// <summary>
        /// Для MediatR событий - условное выполнение
        /// </summary>
        Condition = 2
    }
    public class ScheduleTaskOptions
    {
        /// <summary>
        /// Тип расписания
        /// </summary>
        public ScheduleType Type { get; set; }

        /// <summary>
        /// Интервал (для Interval типа)
        /// </summary>
        public ScheduleInterval Interval { get; set; }

        /// <summary>
        /// Частота (количество единиц интервала)
        /// </summary>
        public int Frequency { get; set; } = 1;

        /// <summary>
        /// Время запуска (для ежедневных/еженедельных задач)
        /// </summary>
        public TimeSpan? TimeOfDay { get; set; }

        /// <summary>
        /// Дни недели (для еженедельных задач)
        /// </summary>
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        /// <summary>
        /// Дни месяца (для ежемесячных задач)
        /// </summary>
        public List<int> DaysOfMonth { get; set; } = new();

        /// <summary>
        /// Месяцы (для ежегодных задач)
        /// </summary>
        public List<int> Months { get; set; } = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        /// <summary>
        /// Часовой пояс
        /// </summary>
        public string? TimeZone { get; set; }

        /// <summary>
        /// Задержка перед первым запуском (в секундах)
        /// </summary>
        public int StartDelaySeconds { get; set; } = 0;

        /// <summary>
        /// Имя группы для Job
        /// </summary>
        public string? GroupName { get; set; } = "DEFAULT";
    }


    public class CommandTaskOptions
    {

    }
    public class ConditionTaskOptions
    {

    }
    public enum ScheduleType
    {
        /// <summary>
        /// Интервал (каждые N секунд/минут/часов)
        /// </summary>
        Interval = 0,

        /// <summary>
        /// Ежедневно
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Еженедельно
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// Ежемесячно
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// Ежегодно
        /// </summary>
        Yearly = 4,

        /// <summary>
        /// Один раз в определенное время
        /// </summary>
        Once = 5
    }
    public enum ScheduleInterval
    {
        /// <summary>
        /// Секунды
        /// </summary>
        Seconds = 0,

        /// <summary>
        /// Минуты
        /// </summary>
        Minutes = 1,

        /// <summary>
        /// Часы
        /// </summary>
        Hours = 2,

        /// <summary>
        /// Дни
        /// </summary>
        Days = 3
    }

    public class SystemBackgroundTaskHistory : BaseHistoryEntity<SystemBackgroundTask> { }
}
