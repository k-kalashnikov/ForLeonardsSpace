using Masofa.Common.Models.Satellite.Parse.Landsat.STAC.ST;
using System.Text.Json;

namespace Masofa.Common.Models
{
    /// <summary>
	/// Именование сущностей
	/// </summary>
    public partial interface IBaseNamedEntity
    {
        public LocalizationString Names { get; set; }
    }

    [Newtonsoft.Json.JsonConverter(typeof(LocalizationStringConverter))]
    public struct LocalizationString
    {
        public LocalizationString() { }

        private static List<string> _langKeys = new List<string>()
        {
            "ru-RU",
            "en-US",
            "ar-LB",
            "uz-Latn-UZ",
            "uz-Cyrl-UZ"
        };

        private Dictionary<string, string> _values = new Dictionary<string, string>();


        public static List<string> SupportedLanguages
        { 
            get
            {
                return _langKeys;
            }
        }
        public string this[string key]
        {
            get 
            {
                if (!_langKeys.Contains(key))
                {
                    throw new ArgumentException($"Language key '{key}' is not supported");
                }
                return _values.ContainsKey(key) ? _values[key] : string.Empty;
            }
            set
            {
                if (!_langKeys.Contains(key))
                {
                    throw new ArgumentException($"Language key '{key}' is not supported");
                }
                if (!_values.ContainsKey(key))
                {
                    _values.Add(key, value);
                }
                _values[key] = value ?? string.Empty;
            }
        }
        public string ValuesJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(_values);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _values = new Dictionary<string, string>();
                    return;
                }

                var s = value.Trim().TrimStart('\uFEFF'); // на всякий случай убираем BOM
                //if (!(string.IsNullOrWhiteSpace(value)) && (!string.IsNullOrEmpty(value)) && (value != "null"))

                    if (s.StartsWith("{") || s.StartsWith("["))
                {
                    var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(s)
                               ?? new Dictionary<string, string>();
                    foreach (var item in temp)
                    {
                        if (_langKeys.Contains(item.Key)) this[item.Key] = item.Value;
                    }
                }
                else
                {
                    // строка не JSON — трактуем как значение по умолчанию
                    _values = new Dictionary<string, string> { { "ru-RU", s } }; // или твой дефолтный ключ
                }

            }
        }

        public override string ToString()
        {
            return ValuesJson;
        }

        public static implicit operator LocalizationString(Dictionary<string, string> values)
        {
            var result = new LocalizationString();
            foreach (var item in values)
            {
                result[item.Key] = item.Value;
            }
            return result;
        }

        public static implicit operator string(LocalizationString localizationString)
        {
            return localizationString.ValuesJson;
        }
    }

    public class LocalizationStringConverter : Newtonsoft.Json.JsonConverter<LocalizationString>
    {
        public override LocalizationString ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, LocalizationString existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var token = Newtonsoft.Json.Linq.JToken.Load(reader);
            var dict = token.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            var result = new LocalizationString();

            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value;
            }

            return result;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, LocalizationString value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value.ValuesJson);
            serializer.Serialize(writer, temp);
        }
    }

    //public class LocalizationStringConverter : System.Text.Json.Serialization.JsonConverter<LocalizationString>
    //{
    //    public override LocalizationString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        // Читаем весь JSON-объект как Dictionary<string, string>
    //        var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ref reader, options) ?? new Dictionary<string, string>();
    //        var result = new LocalizationString();

    //        foreach (var kvp in dict)
    //        {
    //            result[kvp.Key] = kvp.Value;
    //        }

    //        return result;
    //    }

    //    public override void Write(Utf8JsonWriter writer, LocalizationString value, JsonSerializerOptions options)
    //    {
    //        var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value.ValuesJson, options);
    //        Newtonsoft.Json.JsonConvert.SerializeObject(writer, temp, options);
    //    }
    //}

    /// <summary>
    /// Локализованное хранилище файлов - аналогично LocalizationString, но хранит Guid для каждого языка
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(LocalizationFileStorageItemConverter))]
    public struct LocalizationFileStorageItem
    {
        public LocalizationFileStorageItem() { }

        private static List<string> _langKeys = new List<string>()
        {
            "ru-RU",
            "en-US",
            "ar-LB",
            "uz-Latn-UZ",
            "uz-Cyrl-UZ"
        };

        private Dictionary<string, Guid> _values = new Dictionary<string, Guid>();

        public static List<string> SupportedLanguages
        {
            get
            {
                return _langKeys;
            }
        }

        public Guid? this[string key]
        {
            get
            {
                if (!_langKeys.Contains(key))
                {
                    throw new ArgumentException($"Language key '{key}' is not supported");
                }
                return _values.ContainsKey(key) && _values[key] != Guid.Empty ? _values[key] : null;
            }
            set
            {
                if (!_langKeys.Contains(key))
                {
                    throw new ArgumentException($"Language key '{key}' is not supported");
                }
                if (value.HasValue && value.Value != Guid.Empty)
                {
                    if (!_values.ContainsKey(key))
                    {
                        _values.Add(key, value.Value);
                    }
                    else
                    {
                        _values[key] = value.Value;
                    }
                }
                else if (_values.ContainsKey(key))
                {
                    _values.Remove(key);
                }
            }
        }

        public string ValuesJson
        {
            get
            {
                var dict = new Dictionary<string, string>();
                foreach (var kvp in _values)
                {
                    dict[kvp.Key] = kvp.Value.ToString();
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(dict);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _values = new Dictionary<string, Guid>();
                    return;
                }

                var s = value.Trim().TrimStart('\uFEFF'); // на всякий случай убираем BOM

                if (s.StartsWith("{") || s.StartsWith("["))
                {
                    var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(s)
                               ?? new Dictionary<string, string>();
                    foreach (var item in temp)
                    {
                        if (_langKeys.Contains(item.Key) && Guid.TryParse(item.Value, out var guidValue))
                        {
                            this[item.Key] = guidValue;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return ValuesJson;
        }

        public static implicit operator LocalizationFileStorageItem(Dictionary<string, Guid> values)
        {
            var result = new LocalizationFileStorageItem();
            foreach (var item in values)
            {
                result[item.Key] = item.Value;
            }
            return result;
        }

        public static implicit operator string(LocalizationFileStorageItem localizationFileStorageItem)
        {
            return localizationFileStorageItem.ValuesJson;
        }

        public void ForceSet(string key, Guid value)
        {
            if (!_langKeys.Contains(key))
            {
                throw new ArgumentException($"Language key '{key}' is not supported");
            }

            // Логика без проверки на Guid.Empty
            if (!_values.ContainsKey(key))
            {
                _values.Add(key, value);
            }
            else
            {
                _values[key] = value;
            }
        }
    }

    public class LocalizationFileStorageItemConverter : Newtonsoft.Json.JsonConverter<LocalizationFileStorageItem>
    {
        public override LocalizationFileStorageItem ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, LocalizationFileStorageItem existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var token = Newtonsoft.Json.Linq.JToken.Load(reader);
            var dict = token.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            var result = new LocalizationFileStorageItem();

            foreach (var kvp in dict)
            {
                if (Guid.TryParse(kvp.Value, out var guidValue))
                {
                    result[kvp.Key] = guidValue;
                }
            }

            return result;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, LocalizationFileStorageItem value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var dict = new Dictionary<string, string>();
            var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value.ValuesJson) ?? new Dictionary<string, string>();
            foreach (var kvp in temp)
            {
                if (Guid.TryParse(kvp.Value, out _))
                {
                    dict[kvp.Key] = kvp.Value;
                }
            }
            serializer.Serialize(writer, dict);
        }
    }
}
