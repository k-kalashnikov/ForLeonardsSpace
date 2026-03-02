using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Archives;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Masofa.Web.Monolith.Controllers.Dictionaries
{
    /// <summary>
    /// Предоставляет API-методы для управления периодами развития культур
    /// </summary>
    [Route("dictionaries/[controller]")]
    [ApiExplorerSettings(GroupName = "Dictionaries")]
    public class CropPeriodController : BaseDictionaryController<CropPeriod, MasofaDictionariesDbContext>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CropPeriodController"/>.
        /// </summary>
        /// <param name="fileStorageProvider">Провайдер для работы с хранилищем файлов</param>
        /// <param name="dbContext">Контекст базы данных</param>

        /// <param name="logger">Логгер</param>
        /// <param name="configuration">Конфигурация приложения</param>
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        public CropPeriodController(
            IFileStorageProvider fileStorageProvider,
            MasofaDictionariesDbContext dbContext,
            ILogger<CropPeriodController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaDictionariesDbContext masofaDictionariesDbContext) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        {
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ImportSvg(IFormFile archiveFile)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ImportSvg)}";

            try
            {
                if (archiveFile is null || archiveFile.Length == 0)
                    return BadRequest("Файл не передан или пуст.");

                await using var stream = archiveFile.OpenReadStream();

                var byCrop = new Dictionary<string, List<(int order, string svg)>>();

                var ro = new SharpCompress.Readers.ReaderOptions
                {
                    LeaveStreamOpen = true,
                    ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding
                    {
                        Default = Encoding.UTF8
                    },
                    LookForHeader = true
                };

                using (var reader = SharpCompress.Readers.ReaderFactory.Open(stream, ro))
                {
                    while (reader.MoveToNextEntry())
                    {
                        var entry = reader.Entry;
                        if (entry.IsDirectory) continue;

                        var key = entry.Key;
                        if (!key.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) continue;

                        var parts = key.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length < 3)
                        {
                            await BusinessLogicLogger.LogInformationAsync($"Skip entry '{key}': path too short", nameof(ImportSvg));
                            continue;
                        }

                        var cropFolderName = parts[1].Trim();
                        var fileName = parts[^1];

                        var normCrop = Normalize(cropFolderName);
                        var order = ExtractOrder(fileName);
                        if (order == int.MaxValue) continue;

                        try
                        {
                            using var ms = new MemoryStream();
                            reader.WriteEntryTo(ms);
                            var svgText = Encoding.UTF8.GetString(ms.ToArray());

                            if (!byCrop.TryGetValue(normCrop, out var list))
                                list = byCrop[normCrop] = new List<(int, string)>();
                            list.Add((order, svgText));
                        }
                        catch (Exception ex)
                        {
                            await BusinessLogicLogger.LogInformationAsync($"Skip entry '{key}': {ex.Message}", nameof(ImportSvg));
                        }
                    }
                }


                // --- та же логика, что и раньше ---
                foreach (var key in byCrop.Keys.ToList())
                    byCrop[key] = byCrop[key].OrderBy(t => t.order).ToList();

                var periods = await MasofaDictionariesDbContext.CropPeriods
                    .Where(x => x.CropId != null)
                    .ToListAsync();

                var cropIds = periods.Where(p => p.CropId != null).Select(p => p.CropId!.Value).Distinct().ToList();
                var crops = await MasofaDictionariesDbContext.Crops
                    .Where(c => cropIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c);

                int updated = 0, skipped = 0, mismatched = 0;

                var cropsDict = await MasofaDictionariesDbContext.Crops
                .ToDictionaryAsync(
                    c => Normalize(c.Names["ru-RU"]),
                    c => c);

                foreach (var group in periods.Where(p => p.CropId != null).GroupBy(p => p.CropId!.Value))
                {
                    if (!crops.TryGetValue(group.Key, out var crop))
                    {
                        skipped += group.Count();
                        continue;
                    }

                    var cropNameRu = crop.Names["ru-RU"];
                    var normCropName = Normalize(cropNameRu);

                    if (!byCrop.TryGetValue(cropNameRu, out var svgsForCrop) || svgsForCrop.Count == 0)
                    {
                        skipped += group.Count();
                        continue;
                    }

                    var orderedPeriods = group.OrderBy(p => p.DayStart ?? int.MinValue)
                                              .ThenBy(p => p.DayEnd ?? int.MaxValue)
                                              .ToList();

                    var take = Math.Min(orderedPeriods.Count, svgsForCrop.Count);

                    for (int i = 0; i < take; i++)
                    {
                        orderedPeriods[i].ImageSvg = svgsForCrop[i].svg;
                        MasofaDictionariesDbContext.Entry(orderedPeriods[i]).State = EntityState.Modified;
                        updated++;
                    }
                }

                await MasofaDictionariesDbContext.SaveChangesAsync();

                await BusinessLogicLogger.LogInformationAsync(
                    $"Finish request in {requestPath}. Updated={updated}, Skipped={skipped}, Mismatched={mismatched}", requestPath);

                return Ok(new { Updated = updated, Skipped = skipped, Mismatched = mismatched });
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            // локальные функции
            static string Normalize(string s)
                => string.Concat((s ?? string.Empty)
                    .ToLowerInvariant()
                    .Where(ch => !char.IsWhiteSpace(ch) && ch != '_' && ch != '-' && ch != '.'));

            static int ExtractOrder(string fileName)
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                var m = Regex.Match(name, @"(\d+)$");
                return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : int.MaxValue;
            }
        }

        //[HttpPost]
        //[Route("[action]")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ImportSvg()
        //{
        //    var requestPath = $"{GetType().FullName}=>{nameof(ImportSvg)}";
        //    try
        //    {
        //        await BusinessLogicLogger.LogInformationAsync($"Start request in {requestPath}", requestPath);

        //        var cropPeriods = await MasofaDictionariesDbContext.CropPeriods.Where(x => x.CropId != null).ToListAsync();

        //        foreach (var cropPeriod in cropPeriods)
        //        {
        //            var crop = await MasofaDictionariesDbContext.Crops.Where(c => c.Id == cropPeriod.CropId).FirstAsync();

        //            var cropName = crop.Names["ru-RU"];


        //        }
                

        //        await BusinessLogicLogger.LogInformationAsync($"Finish request in {requestPath} with result: {Newtonsoft.Json.JsonConvert.SerializeObject(fields)}", requestPath);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = $"Something wrong in {requestPath}. {ex.Message}";
        //        await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
        //        Logger.LogCritical(ex, msg);
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}
    }
}
