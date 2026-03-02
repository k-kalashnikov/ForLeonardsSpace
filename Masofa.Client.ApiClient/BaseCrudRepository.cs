using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Masofa.Common.Models;

namespace Masofa.Client.ApiClient
{
    public class BaseCrudRepository<TModel> : BaseRepository
        where TModel : BaseEntity
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // WriteIndented = false, // по умолчанию false, можно не указывать
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            // Атрибуты вроде [JsonIgnore] учитываются по умолчанию
        };
        public class SystemTextJsonLocalizationStringConverter : JsonConverter<LocalizationString>
        {
            public override LocalizationString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var jsonDoc = JsonDocument.ParseValue(ref reader);
                var dict = jsonDoc.RootElement.Deserialize<Dictionary<string, string>>(options)
                           ?? new Dictionary<string, string>();

                var result = new LocalizationString();
                foreach (var kvp in dict)
                {
                    // Проверка поддерживаемых языков уже есть в индексаторе
                    result[kvp.Key] = kvp.Value;
                }
                return result;
            }

            public override void Write(Utf8JsonWriter writer, LocalizationString value, JsonSerializerOptions options)
            {
                // Сериализуем внутренний словарь напрямую как JSON-объект
                var dict = new Dictionary<string, string>();
                foreach (var lang in LocalizationString.SupportedLanguages)
                {
                    var val = value[lang];
                    if (!string.IsNullOrEmpty(val))
                        dict[lang] = val;
                }

                JsonSerializer.Serialize(writer, dict, options);
            }
        }

        static BaseCrudRepository()
        {
            // Добавляем конвертер для LocalizationString
            DefaultJsonOptions.Converters.Add(new SystemTextJsonLocalizationStringConverter());
        }

        public BaseCrudRepository(HttpClient httpClient, string baseUrl) : base(httpClient, baseUrl)
        {
        }

        public async Task<List<TModel>> GetByQueryAsync(BaseGetQuery<TModel> query, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{BaseUrl}/GetByQuery"));
            // Сериализуем вручную с нужными опциями
            var json = System.Text.Json.JsonSerializer.Serialize(query, DefaultJsonOptions);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await SendRequestAsync<List<TModel>>(AddAuthenticationJWT(message), nameof(GetByQueryAsync), cancellationToken);
            return result;
        }

        public async Task<TModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await SendRequestAsync<TModel>(AddAuthenticationJWT(new HttpRequestMessage(HttpMethod.Get, new Uri($"{BaseUrl}/GetById/{id}"))), nameof(GetByIdAsync), cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(BaseGetQuery<TModel> query, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{BaseUrl}/GetTotalCount"));
            var json = System.Text.Json.JsonSerializer.Serialize(query, DefaultJsonOptions);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await SendRequestAsync<int>(AddAuthenticationJWT(message), nameof(GetTotalCountAsync), cancellationToken);
            return result;
        }

        public async Task<Guid> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{BaseUrl}/Create"));
            // Сериализуем вручную с нужными опциями
            var json = System.Text.Json.JsonSerializer.Serialize(model, DefaultJsonOptions);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await SendRequestAsync<Guid>(AddAuthenticationJWT(message), nameof(CreateAsync), cancellationToken);
            return result;
        }

        public async Task<TModel> UpdateAsync(TModel model, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, new Uri($"{BaseUrl}/Update"));
            // Сериализуем вручную с нужными опциями
            var json = System.Text.Json.JsonSerializer.Serialize(model, DefaultJsonOptions);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await SendRequestAsync<TModel>(AddAuthenticationJWT(message), nameof(UpdateAsync), cancellationToken);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{BaseUrl}/Delete/{id}"));
            var result = await SendRequestAsync<bool>(AddAuthenticationJWT(message), nameof(DeleteAsync), cancellationToken);
            return result;
        }

        public async Task<bool> ImportFromCSVAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = content; // Предполагается, что файл передаётся как formFile
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ImportFromCSV");
            return await SendRequestAsync<bool>(AddAuthenticationJWT(httpRequestMessage), nameof(ImportFromCSVAsync), cancellationToken);
        }

        public async Task<byte[]> ExportFromCSVAsync(BaseGetQuery<TModel> query, CancellationToken cancellationToken)
        {
            HttpRequestMessage httpRequestMessage = new();
            httpRequestMessage.Method = HttpMethod.Post;
            // Сериализуем вручную с нужными опциями
            var json = System.Text.Json.JsonSerializer.Serialize(query, DefaultJsonOptions);
            httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            httpRequestMessage.RequestUri = new Uri($"{BaseUrl}/ExportFromCSV");
            // Для бинарных данных (файлов) лучше использовать отдельный метод или вручную обрабатывать HttpResponseMessage
            var response = await SendRequestAndGetResponseAsync(AddAuthenticationJWT(httpRequestMessage), nameof(ExportFromCSVAsync), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }
            // Обработка ошибок, как в SendRequestAsync
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    throw new System.Net.Http.HttpRequestException("Page not found", null, System.Net.HttpStatusCode.NotFound);
                case System.Net.HttpStatusCode.Unauthorized:
                    throw new System.Net.Http.HttpRequestException("Unauthorized user", null, System.Net.HttpStatusCode.Unauthorized);
                case System.Net.HttpStatusCode.BadRequest:
                    var badRequestContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new System.Net.Http.HttpRequestException($"Model not valid: {badRequestContent}", null, System.Net.HttpStatusCode.BadRequest);
                case System.Net.HttpStatusCode.InternalServerError:
                    var serverErrorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new System.Net.Http.HttpRequestException($"Internal Server Error: {serverErrorContent}", null, System.Net.HttpStatusCode.InternalServerError);
                default:
                    var defaultContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new System.Net.Http.HttpRequestException($"Unhandled code: {response.StatusCode}. With message: {defaultContent}", null, response.StatusCode);
            }
        }

        // Вспомогательный метод для получения HttpResponseMessage (например, для бинарных данных)
        private async Task<System.Net.Http.HttpResponseMessage> SendRequestAndGetResponseAsync(HttpRequestMessage request, string methodName, CancellationToken cancellationToken)
        {
            try
            {
                return await HttpClient.SendAsync(request, cancellationToken);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw;
            }
            catch (System.OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new System.InvalidOperationException($"Failed to send request in {methodName}.", ex);
            }
        }
    }
}