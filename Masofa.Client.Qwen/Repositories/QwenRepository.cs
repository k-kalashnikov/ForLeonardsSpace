using Masofa.Client.Qwen.Models;
using Masofa.Common.Models.MasofaAnaliticReport;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masofa.Client.Qwen.Repositories
{
    public class QwenRepository
    {
        private HttpClient HttpClient { get; }
        private ILogger Logger { get; }

        private static readonly global::System.Text.Json.JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public QwenRepository(HttpClient httpClient, ILogger logger)
        {
            HttpClient = httpClient;
            HttpClient.BaseAddress = new Uri("http://185.100.234.107:42020/api/v1/");
            Logger = logger;
        }

        /// <summary>
        /// Универсальная отправка архива на диагностику.
        /// express=true  -> экспресс (только аномалии, без рекомендаций)
        /// express=false -> фулл (аномалии + рекомендации)
        /// </summary>
        public async Task<string> SubmitArchiveAsync(
            Stream archiveStream,
            string fileName,
            bool express,
            string? cropType = null)
        {
            if (archiveStream == null) throw new ArgumentNullException(nameof(archiveStream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Имя файла не может быть пустым", nameof(fileName));

            Logger.LogInformation("Sending archive to Qwen. express={Express}, cropType={CropType}, fileName={FileName}",
                express, cropType ?? "<auto>", fileName);

            try
            {
                if (archiveStream.CanSeek)
                    archiveStream.Position = 0;

                using var form = new MultipartFormDataContent();

                // file
                var streamContent = new StreamContent(archiveStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                form.Add(streamContent, "file", fileName);

                // crop_type (optional)
                if (!string.IsNullOrWhiteSpace(cropType))
                {
                    form.Add(new StringContent(cropType, Encoding.UTF8), "crop_type");
                }

                // express (required)
                form.Add(new StringContent(express ? "true" : "false", Encoding.UTF8), "express");

                var response = await HttpClient.PostAsync("diag", form);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Qwen API error ({response.StatusCode}): {content}");
                }

                var result = JsonSerializer.Deserialize<QwenJobResponse>(content, JsonOptions)
                             ?? throw new InvalidOperationException("Ответ не удалось десериализовать в QwenJobResponse");

                if (string.IsNullOrWhiteSpace(result.job_id))
                    throw new InvalidOperationException("Ответ не содержит job_id");

                return result.job_id;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to submit archive to Qwen. express={Express}, fileName={FileName}", express, fileName);
                throw;
            }
        }

        public Task<string> SubmitArchiveExpressAsync(Stream archiveStream, string fileName, string? cropType = null)
            => SubmitArchiveAsync(archiveStream, fileName, express: true, cropType: cropType);

        public Task<string> SubmitArchiveFullAsync(Stream archiveStream, string fileName, string? cropType = null)
            => SubmitArchiveAsync(archiveStream, fileName, express: false, cropType: cropType);

        public async Task<QwenJobStatusResponse> GetJobStatusAsync(string jobId)
        {
            var response = await HttpClient.GetAsync($"job/status/{jobId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get job status. Status: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<QwenJobStatusResponse>(content, JsonOptions)!;
        }

        public async Task<QwenJobResultResponse> GetJobResultAsync(string jobId)
        {
            var response = await HttpClient.GetAsync($"job/result/{jobId}?file_type=json");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get job result. Status: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<QwenJobResultResponse>(content, JsonOptions)!
                   ?? throw new Exception("Failed to deserialize Qwen job result response.");
        }

        public async Task<bool> StopJobAsync(string jobId)
        {
            var response = await HttpClient.PostAsync($"job/stop/{jobId}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
