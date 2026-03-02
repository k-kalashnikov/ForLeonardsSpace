using Masofa.Common.Resources;
using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Контроллер для управления файловым хранилищем (только чтение)
    /// </summary>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SystemAdmin, ModuleAdmin, Operator")]
    public class AdminFileStorageController : BaseController
    {
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public AdminFileStorageController(
            ILogger<AdminFileStorageController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IFileStorageProvider fileStorageProvider,
            IBusinessLogicLogger businessLogicLogger)
            : base(logger, configuration, mediator)
        {
            _fileStorageProvider = fileStorageProvider;
            _businessLogicLogger = businessLogicLogger;
        }

        /// <summary>
        /// Получает список всех бакетов
        /// </summary>
        /// <returns>Список имен бакетов</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> GetBuckets()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetBuckets)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new GetFileStorageBucketsRequest());
                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} buckets", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает список файлов в бакете
        /// </summary>
        /// <param name="bucket">Имя бакета</param>
        /// <param name="prefix">Префикс для фильтрации (путь внутри бакета)</param>
        /// <returns>Список файлов</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<FileStorageItem>>> GetFilesByBucket([FromQuery] string bucket, [FromQuery] string? prefix = null)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFilesByBucket)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                if (string.IsNullOrEmpty(bucket))
                {
                    return BadRequest("Bucket name is required");
                }

                var result = await Mediator.Send(new GetFileStorageFilesByBucketRequest
                {
                    Bucket = bucket,
                    Prefix = prefix
                });
                
                await _businessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {result.Count} files", requestPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Получает информацию о файле по ID
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>Информация о файле</returns>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<FileStorageItem>> GetFileInfo(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFileInfo)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var result = await Mediator.Send(new GetFileInfoRequest { Id = id });
                
                if (result == null)
                {
                    var errorMsg = LogMessageResource.FileNotFound(id.ToString());
                    await _businessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }
                
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, result.FileStoragePath), requestPath);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return NotFound(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }

        /// <summary>
        /// Скачивает файл по ID
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>Файл для скачивания</returns>
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> DownloadFile(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(DownloadFile)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                
                var fileStorageItem = await Mediator.Send(new GetFileInfoRequest { Id = id });
                
                if (fileStorageItem == null)
                {
                    var errorMsg = LogMessageResource.FileNotFound(id.ToString());
                    await _businessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var fileBytes = await _fileStorageProvider.GetFileBytesAsync(fileStorageItem);
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, "result"), requestPath);
                
                return File(fileBytes, fileStorageItem.FileContentTypeName, 
                    System.IO.Path.GetFileName(fileStorageItem.FileStoragePath));
            }
            catch (KeyNotFoundException ex)
            {
                await _businessLogicLogger.LogErrorAsync(LogMessageResource.GenericError(requestPath,ex.Message), requestPath);
                return NotFound(LogMessageResource.GenericError(requestPath,ex.Message));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, LogMessageResource.GenericError(requestPath,ex.Message));
            }
        }
    }
}

