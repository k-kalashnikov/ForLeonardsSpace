using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion3
{
    public class Control
    {
        [Required]
        [JsonPropertyName("type")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ControlType Type { get; set; }

        [Required]
        [MinLength(1)]
        [RegularExpression("^[a-z][a-z_0-9]+$")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("auto")]
        public bool Auto { get; set; }

        [Required]
        [JsonPropertyName("required")]
        public bool Required { get; set; }

        [Required]
        [JsonPropertyName("source")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ControlSource Source { get; set; }

        [Required]
        [JsonPropertyName("readonly")]
        public bool Readonly { get; set; }

        [Required]
        [MinLength(1)]
        [JsonPropertyName("labels")]
        public LocalizationString Labels { get; set; } = new LocalizationString();

        [JsonPropertyName("descriptions")]
        public LocalizationString Descriptions { get; set; } = new LocalizationString();

        [MinLength(1)]
        [JsonPropertyName("values")]
        public LocalizationString Values { get; set; } = new LocalizationString();

        public static implicit operator Control(Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2.Control bidTemplateControl)
        {
            return new Control()
            {
                Auto = bidTemplateControl.Auto,
                Name = bidTemplateControl.Name,
                Readonly = bidTemplateControl.Readonly,
                Required = bidTemplateControl.Required,
                Source = bidTemplateControl.Source,
                Type = bidTemplateControl.Type,
                Labels = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateControl.LabelRu },
                    { "en-US", bidTemplateControl.LabelEn },
                    { "ar-LB", bidTemplateControl.LabelAr },
                    { "uz-Latn-UZ", bidTemplateControl.LabelUz},
                },
                Descriptions = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateControl.DescrRu ?? string.Empty},
                    { "en-US", bidTemplateControl.DescrEn ?? string.Empty},
                    { "ar-LB", bidTemplateControl.DescrAr ?? string.Empty},
                    { "uz-Latn-UZ", bidTemplateControl.DescrUz ?? string.Empty},
                },
                Values = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateControl.ValuesRu ?? string.Empty},
                    { "en-US", bidTemplateControl.ValuesEn ?? string.Empty},
                    { "ar-LB", bidTemplateControl.ValuesAr ?? string.Empty},
                    { "uz-Latn-UZ", bidTemplateControl.ValuesUz ?? string.Empty},
                }
            };
        }
    }

    public class Step
    {
        [Required]
        [RegularExpression("^\\d+$")]
        [JsonPropertyName("num")]
        public string Num { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^\\d+-\\d+$")]
        [JsonPropertyName("days")]
        public string Days { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [JsonPropertyName("names")]
        public LocalizationString Names { get; set; } = new LocalizationString();

        [Required]
        [MinLength(1)]
        [JsonPropertyName("description_en")]
        public LocalizationString Descriptions { get; set; } = new LocalizationString();

        [Required]
        [JsonPropertyName("controls")]
        public List<Control> Controls { get; set; } = new();

        public static implicit operator Step(Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2.Step bidTemplateStep)
        {
            return new Step()
            {
                Controls = bidTemplateStep.Controls?.Select(m => (Control)m)?.ToList() ?? new List<Control>(),
                Days = bidTemplateStep.Days,
                Num = bidTemplateStep.Num,
                Names = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateStep.NameRu },
                    { "en-US", bidTemplateStep.NameEn },
                    { "ar-LB", bidTemplateStep.NameAr },
                    { "uz-Latn-UZ", bidTemplateStep.NameUz },
                },
                Descriptions = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateStep.DescriptionRu ?? string.Empty},
                    { "en-US", bidTemplateStep.DescriptionEn ?? string.Empty},
                    { "ar-LB", bidTemplateStep.DescriptionAr ?? string.Empty},
                    { "uz-Latn-UZ", bidTemplateStep.DescriptionUz ?? string.Empty},
                },
            };
        }
    }

    public class Block
    {
        [Required]
        [JsonPropertyName("blockType")]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public BlockType BlockType { get; set; }

        [JsonPropertyName("controls")]
        public List<Control>? Controls { get; set; }

        [JsonPropertyName("steps")]
        public List<Step>? Steps { get; set; }

        public LocalizationString Names { get; set; }

        public static implicit operator Block(Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2.Block bidTemplateBlock)
        {
            return new Block()
            {
                BlockType = bidTemplateBlock.BlockType,
                Controls = bidTemplateBlock.Controls?.Select(m => (Control)m)?.ToList() ?? new List<Control>(),
                Steps = bidTemplateBlock.Steps?.Select(m => (Step)m)?.ToList() ?? new List<Step>(),
                Names = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplateBlock.BlockNameRu },
                    { "en-US", bidTemplateBlock.BlockNameEn },
                    { "ar-LB", bidTemplateBlock.BlockNameAr },
                    { "uz-Latn-UZ", bidTemplateBlock.BlockNameUz },
                },
            };
        }
    }

    public class BidTemplateSchemaVersion3
    {
        [Required]
        [RegularExpression("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$")]
        [JsonPropertyName("cropId")]
        public Guid CropId { get; set; } = Guid.Empty;

        [Required]
        [JsonPropertyName("cropNames")]
        public LocalizationString CropNames { get; set; } = new LocalizationString();

        [Required]
        [Range(2, 2, ErrorMessage = "schemaVersion must be exactly 2")]
        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 3;

        [Required]
        [Range(1, int.MaxValue)]
        [JsonPropertyName("contentVersion")]
        public int ContentVersion { get; set; }

        [Required]
        [RegularExpression("^\\d+\\.\\d+$")]
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [Required]
        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; } = new();

        public static implicit operator BidTemplateSchemaVersion3(Masofa.Common.Models.CropMonitoring.BidTemplateSchemaVersion2.BidTemplateSchemaVersion2 bidTemplate)
        {
            return new BidTemplateSchemaVersion3()
            {
                CropId = bidTemplate.CropId,
                Blocks = bidTemplate.Blocks?.Select(m => (Block)m)?.ToList() ?? new List<Block>(),
                SchemaVersion = 3,
                ContentVersion = bidTemplate.ContentVersion,
                CropNames = new Dictionary<string, string>()
                {
                    { "ru-RU", bidTemplate.CropNameRu },
                    { "en-US", bidTemplate.CropNameEn },
                    { "ar-LB", bidTemplate.CropNameAr },
                    { "uz-Latn-UZ", bidTemplate.CropNameUz },
                },
            };
        }
    }
}