using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2
{
    /// <summary>
    /// Описывает один управляющий элемент (контрол) шаблона заявки.
    /// </summary>
    public class Control
    {
        /// <summary>
        /// Тип контрола.
        /// </summary>
        [Required]
        [JsonPropertyName("type")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ControlType Type { get; set; }

        /// <summary>
        /// Уникальное имя контрола (латинскими буквами, нижнее подчеркивание и цифры).
        /// </summary>
        [Required]
        [RegularExpression("^[a-z][a-z_0-9]+$")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Автоматическое заполнение (true — автозаполнение).
        /// </summary>
        [Required]
        [JsonPropertyName("auto")]
        public bool Auto { get; set; }

        /// <summary>
        /// Обязательное поле (true — обязательно к заполнению).
        /// </summary>
        [Required]
        [JsonPropertyName("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Источник значения контрола (пользователь, API, приложение и т.д.).
        /// </summary>
        [Required]
        [JsonPropertyName("source")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ControlSource Source { get; set; }

        /// <summary>
        /// Только для чтения (true — нельзя редактировать).
        /// </summary>
        [Required]
        [JsonPropertyName("readonly")]
        public bool Readonly { get; set; }

        /// <summary>
        /// Метка на английском языке.
        /// </summary>
        [JsonPropertyName("label_en")]
        public string LabelEn { get; set; } = string.Empty;

        /// <summary>
        /// Метка на русском языке.
        /// </summary>
        [JsonPropertyName("label_ru")]
        public string LabelRu { get; set; } = string.Empty;

        /// <summary>
        /// Метка на узбекском языке.
        /// </summary>
        [JsonPropertyName("label_uz")]
        public string LabelUz { get; set; } = string.Empty;

        /// <summary>
        /// Метка на арабском языке.
        /// </summary>
        [JsonPropertyName("label_ar")]
        public string LabelAr { get; set; } = string.Empty;

        /// <summary>
        /// Описание контрола на английском языке (опционально).
        /// </summary>
        [JsonPropertyName("descr_en")]
        public string? DescrEn { get; set; }

        /// <summary>
        /// Описание контрола на русском языке (опционально).
        /// </summary>
        [JsonPropertyName("descr_ru")]
        public string? DescrRu { get; set; }

        /// <summary>
        /// Описание контрола на узбекском языке (опционально).
        /// </summary>
        [JsonPropertyName("descr_uz")]
        public string? DescrUz { get; set; }

        /// <summary>
        /// Описание контрола на арабском языке (опционально).
        /// </summary>
        [JsonPropertyName("descr_ar")]
        public string? DescrAr { get; set; }

        /// <summary>
        /// Допустимые значения на английском (опционально).
        /// </summary>
        [JsonPropertyName("values_en")]
        public string? ValuesEn { get; set; }

        /// <summary>
        /// Допустимые значения на русском (опционально).
        /// </summary>
        [JsonPropertyName("values_ru")]
        public string? ValuesRu { get; set; }

        /// <summary>
        /// Допустимые значения на узбекском (опционально).
        /// </summary>
        [JsonPropertyName("values_uz")]
        public string? ValuesUz { get; set; }

        /// <summary>
        /// Допустимые значения на арабском (опционально).
        /// </summary>
        [JsonPropertyName("values_ar")]
        public string? ValuesAr { get; set; }
    }

    /// <summary>
    /// Описывает шаг в блоке шаблона (номер шага, диапазон дней и контролы).
    /// </summary>
    public class Step
    {
        /// <summary>
        /// Номер шага (строка с числом).
        /// </summary>
        [Required]
        [RegularExpression("^\\d+$")]
        [JsonPropertyName("num")]
        public string Num { get; set; } = string.Empty;

        /// <summary>
        /// Диапазон дней в формате "start-end".
        /// </summary>
        [Required]
        [RegularExpression("^\\d+-\\d+$")]
        [JsonPropertyName("days")]
        public string Days { get; set; } = string.Empty;

        /// <summary>
        /// Название шага на английском языке.
        /// </summary>
        [JsonPropertyName("name_en")]
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Название шага на русском языке.
        /// </summary>
        [JsonPropertyName("name_ru")]
        public string NameRu { get; set; } = string.Empty;

        /// <summary>
        /// Название шага на узбекском языке.
        /// </summary>
        [JsonPropertyName("name_uz")]
        public string NameUz { get; set; } = string.Empty;

        /// <summary>
        /// Название шага на арабском языке.
        /// </summary>
        [JsonPropertyName("name_ar")]
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Описание шага на английском языке.
        /// </summary>
        [JsonPropertyName("description_en")]
        public string DescriptionEn { get; set; } = string.Empty;

        /// <summary>
        /// Описание шага на русском языке.
        /// </summary>
        [JsonPropertyName("description_ru")]
        public string DescriptionRu { get; set; } = string.Empty;

        /// <summary>
        /// Описание шага на узбекском языке.
        /// </summary>
        [JsonPropertyName("description_uz")]
        public string DescriptionUz { get; set; } = string.Empty;

        /// <summary>
        /// Описание шага на арабском языке.
        /// </summary>
        [JsonPropertyName("description_ar")]
        public string DescriptionAr { get; set; } = string.Empty;

        /// <summary>
        /// Список контролов, принадлежащих шагу.
        /// </summary>
        [Required]
        [JsonPropertyName("controls")]
        public List<Control> Controls { get; set; } = new();
    }

    /// <summary>
    /// Описывает блок шаблона — набор шагов или контролов, а также локализованные имена блока.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Тип блока.
        /// </summary>
        [Required]
        [JsonPropertyName("blockType")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public BlockType BlockType { get; set; }

        /// <summary>
        /// Список контролов в блоке (опционально).
        /// </summary>
        [JsonPropertyName("controls")]
        public List<Control>? Controls { get; set; }

        /// <summary>
        /// Список шагов в блоке (опционально).
        /// </summary>
        [JsonPropertyName("steps")]
        public List<Step>? Steps { get; set; }

        /// <summary>
        /// Локализованное название блока на английском языке.
        /// </summary>
        [JsonPropertyName("blockName_en")]
        public string BlockNameEn { get; set; } = string.Empty;

        /// <summary>
        /// Локализованное название блока на русском языке.
        /// </summary>
        [JsonPropertyName("blockName_ru")]
        public string BlockNameRu { get; set; } = string.Empty;

        /// <summary>
        /// Локализованное название блока на узбекском языке.
        /// </summary>
        [JsonPropertyName("blockName_uz")]
        public string BlockNameUz { get; set; } = string.Empty;

        /// <summary>
        /// Локализованное название блока на арабском языке.
        /// </summary>
        [JsonPropertyName("blockName_ar")]
        public string BlockNameAr { get; set; } = string.Empty;
    }

    /// <summary>
    /// Корневая модель схемы шаблона заявки версии 2.
    /// </summary>
    public class BidTemplateSchemaVersion2
    {
        /// <summary>
        /// Идентификатор культуры/культуры растения (GUID).
        /// </summary>
        [Required]
        [RegularExpression("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$")]
        [JsonPropertyName("cropId")]
        public Guid CropId { get; set; } = Guid.Empty;

        /// <summary>
        /// Название культуры на русском языке (опционально).
        /// </summary>
        [JsonPropertyName("cropNameRu")]
        public string CropNameRu { get; set; } = string.Empty;

        /// <summary>
        /// Название культуры на английском языке (опционально).
        /// </summary>
        [JsonPropertyName("cropNameEn")]
        public string CropNameEn { get; set; } = string.Empty;

        /// <summary>
        /// Название культуры на узбекском языке (опционально).
        /// </summary>
        [JsonPropertyName("cropNameUz")]
        public string CropNameUz { get; set; } = string.Empty;

        /// <summary>
        /// Название культуры на арабском языке (опционально).
        /// </summary>
        [JsonPropertyName("cropNameAr")]
        public string CropNameAr { get; set; } = string.Empty;

        /// <summary>
        /// Версия схемы (фиксированное значение 2).
        /// </summary>
        [Required]
        [Range(2, 2, ErrorMessage = "schemaVersion must be exactly 2")]
        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } // const: 2

        /// <summary>
        /// Версия содержимого (целое число, >=1).
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        [JsonPropertyName("contentVersion")]
        public int ContentVersion { get; set; }

        /// <summary>
        /// Версия формата (строка в формате major.minor, например "1.0").
        /// </summary>
        [Required]
        [RegularExpression("^\\d+\\.\\d+$")]
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Список блоков, составляющих шаблон.
        /// </summary>
        [Required]
        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; } = new();
    }
}