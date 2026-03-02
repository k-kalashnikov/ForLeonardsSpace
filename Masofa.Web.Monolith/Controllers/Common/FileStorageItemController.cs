using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Masofa.Web.Monolith.Controllers.Common
{
    /// <summary>
    /// Provides API endpoints for managing file storage items.
    /// </summary>
    /// <remarks>This controller is part of the "Common" API group and provides functionality for interacting
    /// with files stored in the system. It inherits from <see cref="BaseCrudController{TEntity, TDbContext}"/> to
    /// provide basic CRUD operations, and extends it with additional file-specific functionality.</remarks>
    [Route("common/[controller]")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class FileStorageItemController : BaseCrudController<FileStorageItem, MasofaCommonDbContext>
    {
        public FileStorageItemController(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext,
            ILogger<FileStorageItemController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor)
            : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {

        }

        /// <summary>
        /// Получает файл по идентификатору из файлового хранилища
        /// </summary>
        /// <param name="id">Идентификатор файла в хранилище</param>
        /// <returns>Байты файла</returns>
        /// <response code="200">Файл успешно получен</response>
        /// <response code="404">Файл с указанным ID не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<byte[]>> GetFile(Guid id)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetFile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var fileStorageItem = DbContext.FileStorageItems.FirstOrDefault(x => x.Id.Equals(id));

                if (fileStorageItem == null)
                {
                    var errorMsg = LogMessageResource.EntityNotFound(typeof(FileStorageItem).FullName, id.ToString());
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }
                var result = await FileStorageProvider.GetFileBytesAsync(fileStorageItem);
                var fileName = $"{id.ToString()}{Path.GetExtension(fileStorageItem.FileStoragePath)}";
                return File(result, fileStorageItem.FileContentTypeName, fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
