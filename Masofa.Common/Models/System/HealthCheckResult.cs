using Masofa.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.SystemCrical
{
    public class HealthCheckResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public List<HealthCheckModuleResult> Modules { get; set; } = new List<HealthCheckModuleResult>();

        public string ModulesJson 
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Modules);
            }
            set
            {
                Modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HealthCheckModuleResult>>(value ?? string.Empty) ?? new List<HealthCheckModuleResult>();
            } 
        }

        [NotMapped]
        public List<string> Errors
        {
            get
            {
                var result = new List<string>();

                foreach (var item in Modules)
                {
                    result.AddRange(item.Errors);
                }

                return result;
            }
        }
    }

    public class HealthCheckModuleResult
    {
        public string ModuleName { get; set; } //Название модуля - common, Crop Monitoring и т.п.
        public LocalizationString ModuleNames { get; set; } = new LocalizationString();
        public List<HealthCheckModuleModelResult> Mоdels { get; set; } = new List<HealthCheckModuleModelResult>();
        public List<string> Errors
        {
            get
            {
                var result = new List<string>();

                foreach (var item in Mоdels)
                {
                    result.AddRange(item.Errors);
                }

                return result;
            }
        }

        [NotMapped]
        public List<LocalizationString> LocalizedErrors
        {
            get
            {
                var result = new List<LocalizationString>();

                foreach (var item in Mоdels)
                {
                    result.AddRange(item.LocalizedErrors);
                }

                return result;
            }
        }
    }

    public class HealthCheckModuleModelResult
    {
        public string ModelName { get; set; } //Имя модели - Bid, Field и т.п.
        public LocalizationString ModelNames { get; set; } = new LocalizationString();
        public bool DbLayerCheck { get; set; } //Проверка, что таблица существует в БД
        public bool LogicLayerCheck { get; set; } //Проверка, что команды и запросы существует в DI
        public bool WebApi { get; set; } //Проверка, что WebApi существует
        public List<string> Errors { get; set; } = new List<string>();
        public List<LocalizationString> LocalizedErrors { get; set; } = new List<LocalizationString>();
    }
}
