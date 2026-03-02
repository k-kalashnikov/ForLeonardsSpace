using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Masofa.BusinessLogic.ImportedFIeld
{
    public class MigrationFromImportedFieldsCommand : IRequest<List<MigrationImportedFieldResult>>
    {
        /// <summary>
        /// Идентификатор сессии загружаемых полей
        /// </summary>
        public Guid ImportedFieldReportId { get; set; }

        /// <summary>
        /// Сопоставление атрибутов сущности ImportedField и полей сущности Season
        /// Key: SeasonPropertyName, Value: List of ImportedFieldAttributeNames
        /// </summary>
        public Dictionary<string, List<string>> FieldMappings { get; set; } = [];

        /// <summary>
        /// Значения по умолчанию для посева
        /// </summary>
        public Masofa.Common.Models.CropMonitoring.Season? DefaultSeason { get; set; }

        /// <summary>
        /// Автор изменения
        /// </summary>
        [Required]
        public required string Author { get; set; }
    }

    public class MigrationFromImportedFieldsCommandHandler : IRequestHandler<MigrationFromImportedFieldsCommand, List<MigrationImportedFieldResult>>
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger<MigrationFromImportedFieldsCommandHandler> Logger { get; set; }
        private IMediator Mediator { get; set; }

        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; set; }

        public MigrationFromImportedFieldsCommandHandler(IBusinessLogicLogger businessLogicLogger, ILogger<MigrationFromImportedFieldsCommandHandler> logger, MasofaCropMonitoringDbContext cropMonitoringDbContext, MasofaDictionariesDbContext dictionariesDbContext, IMediator mediator)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            CropMonitoringDbContext = cropMonitoringDbContext;
            DictionariesDbContext = dictionariesDbContext;
            Mediator = mediator;
        }

        public async Task<List<MigrationImportedFieldResult>> Handle(MigrationFromImportedFieldsCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(
                    $"Start MigrationFromImportedFieldsCommand: ImportedFieldReportId={request.ImportedFieldReportId}, FieldMappings={Newtonsoft.Json.JsonConvert.SerializeObject(request.FieldMappings)}",
                    requestPath);

                var ifr = CropMonitoringDbContext.ImportedFieldReports.FirstOrDefault(r => r.Id == request.ImportedFieldReportId && r.Status == Masofa.Common.Models.StatusType.Active);
                if (ifr == null)
                {
                    var msg = $"Cannot find session with Id={request.ImportedFieldReportId}.";
                    await BusinessLogicLogger.LogErrorAsync(msg, requestPath);
                    Logger.LogError(msg);
                    throw new InvalidOperationException(msg);
                }

                var importedFields = await CropMonitoringDbContext.ImportedFields
                    .Where(f => f.ImportedFieldReportId == request.ImportedFieldReportId
                             && f.Status == Masofa.Common.Models.StatusType.Active
                             && f.Polygon != null)
                    .ToListAsync(cancellationToken);
                if (importedFields.Count == 0)
                {
                    var msg = $"There is no imported fields within session with Id={request.ImportedFieldReportId}.";
                    await BusinessLogicLogger.LogErrorAsync(msg, requestPath);
                    Logger.LogError(msg);
                    throw new InvalidOperationException(msg);
                }

                Dictionary<string, string> seasonPropByAttr = [];
                foreach (var (k, vs) in request.FieldMappings)
                {
                    foreach (var v in vs)
                    {
                        seasonPropByAttr[v] = k;
                    }
                }

                List<MigrationImportedFieldResult> result = [];

                foreach (var importedField in importedFields)
                {
                    var tmp = new MigrationImportedFieldResult()
                    {
                        ImportedFieldId = importedField.Id
                    };

                    List<string> errors = [];

                    if (importedField.Polygon == null)
                    {
                        errors.Add("ImportedField has no polygon");
                        tmp.Errors = errors;
                        tmp.IsSuccess = false;
                        result.Add(tmp);
                        continue;
                    }

                    try
                    {
                        if (importedField.Polygon.SRID == 0) importedField.Polygon.SRID = 4326;

                        var fieldForSeason = CropMonitoringDbContext.Fields.FirstOrDefault(f => f.Polygon != null && f.Polygon.SRID == 4326
                            && f.Polygon.Covers(importedField.Polygon) && f.Status == Masofa.Common.Models.StatusType.Active);

                        if (fieldForSeason == null)
                        {
                            var regionMaps = await DictionariesDbContext.RegionMaps
                                .Where(rm => rm.Polygon != null && rm.Polygon.SRID == 4326 && rm.Polygon.Covers(importedField.Polygon.Centroid) && rm.Status == Masofa.Common.Models.StatusType.Active)
                                .ToListAsync(cancellationToken);

                            var regions = await DictionariesDbContext.Regions
                                .Where(r => r.RegionMapId != null && regionMaps.Select(rm => rm.Id).Contains(r.RegionMapId.Value) && r.Status == Masofa.Common.Models.StatusType.Active)
                                .OrderByDescending(r => r.Level)
                                .ToListAsync(cancellationToken);

                            var newField = new Masofa.Common.Models.CropMonitoring.Field()
                            {
                                Name = importedField.FieldName,
                                Polygon = importedField.Polygon,
                                FieldArea = importedField.PolygonSquare,
                                RegionId = regions.Count > 0 ? regions[0].Id : null
                            };

                            var newFieldId = await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Field, MasofaCropMonitoringDbContext>()
                            {
                                Model = newField,
                                Author = request.Author
                            }, cancellationToken);

                            tmp.FieldId = newFieldId;
                        }
                        else
                        {
                            tmp.FieldId = fieldForSeason.Id;
                        }

                        var newSeason = new Masofa.Common.Models.CropMonitoring.Season()
                        {
                            Polygon = importedField.Polygon,
                            FieldId = tmp.FieldId
                        };

                        if (request.DefaultSeason != null)
                        {
                            //newSeason.CopyFrom(request.DefaultSeason);
                            var seasonJson = Newtonsoft.Json.JsonConvert.SerializeObject(request.DefaultSeason);
                            var seasonDefaultValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(seasonJson);
                            foreach (var (k, v) in seasonDefaultValues ?? [])
                            {
                                if (v != null)
                                {
                                    SetPropertyValue(newSeason, k, v);
                                }
                            }
                        }
                        newSeason.Title ??= importedField.FieldName;

                        if (importedField.DataJson != null)
                        {
                            var attributes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(importedField.DataJson);

                            foreach (var (attribute, value) in attributes ?? [])
                            {
                                if (seasonPropByAttr.TryGetValue(attribute, out var seasonProp))
                                {
                                    if (seasonProp == "CropId" || seasonProp == "Crop" || seasonProp == "crop")
                                    {
                                        var filter = new FieldFilter()
                                        {
                                            FilterField = "Names",
                                            FilterValue = value,
                                            FilterOperator = FilterOperator.Levenshtein
                                        };
                                        var crops = await Mediator.Send(new BaseGetRequest<Masofa.Common.Models.Dictionaries.Crop, MasofaDictionariesDbContext>()
                                        {
                                            Query = new BaseGetQuery<Masofa.Common.Models.Dictionaries.Crop>()
                                            {
                                                Filters = [filter]
                                            }
                                        }, cancellationToken);
                                        newSeason.CropId = crops.Count > 0 ? crops[0].Id : null;
                                    }
                                    else if (seasonProp == "VarietyId" || seasonProp == "Variety" || seasonProp == "variety")
                                    {
                                        var filter = new FieldFilter()
                                        {
                                            FilterField = "Names",
                                            FilterValue = value,
                                            FilterOperator = FilterOperator.Levenshtein
                                        };
                                        var varieties = await Mediator.Send(new BaseGetRequest<Masofa.Common.Models.Dictionaries.Variety, MasofaDictionariesDbContext>()
                                        {
                                            Query = new BaseGetQuery<Masofa.Common.Models.Dictionaries.Variety>()
                                            {
                                                Filters = [filter]
                                            }
                                        }, cancellationToken);
                                        newSeason.VarietyId = varieties.Count > 0 ? varieties[0].Id : null;
                                    }
                                    else
                                    {
                                        SetPropertyValue(newSeason, seasonProp, value);
                                    }
                                }
                            }
                        }

                        var newSeasonId = await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.Season, MasofaCropMonitoringDbContext>()
                        {
                            Model = newSeason,
                            Author = request.Author
                        }, cancellationToken);

                        var seasons = await CropMonitoringDbContext.Seasons
                            .Where(s => s.Polygon != null && (s.Polygon.Intersects(newSeason.Polygon)
                                                           || s.Polygon.Covers(newSeason.Polygon)
                                                           || newSeason.Polygon.Covers(s.Polygon)
                                                           || s.Polygon.EqualsTopologically(newSeason.Polygon)))
                            .ToListAsync();

                        await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.CropMonitoring.ImportedFieldLog, MasofaCropMonitoringDbContext>()
                        {
                            Model = new ImportedFieldLog()
                            {
                                SeasonId = newSeasonId,
                                IntersectedSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.Intersects(newSeason.Polygon)).Select(s => s.Id).ToList(),
                                CoveredSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.Covers(newSeason.Polygon)).Select(s => s.Id).ToList(),
                                CoveredBySeasons = seasons.Where(s => s.Polygon != null && newSeason.Polygon.Covers(s.Polygon)).Select(s => s.Id).ToList(),
                                EqualPolygonsSeasons = seasons.Where(s => s.Polygon != null && s.Polygon.EqualsTopologically(newSeason.Polygon)).Select(s => s.Id).ToList(),
                            },
                            Author = request.Author,
                        }, cancellationToken);

                        tmp.SeasonId = newSeasonId;
                        tmp.IsSuccess = true;
                        result.Add(tmp);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                        tmp.Errors = errors;
                        tmp.IsSuccess = false;
                        result.Add(tmp);
                    }
                }

                await Mediator.Send(new BaseDeleteCommand<Masofa.Common.Models.CropMonitoring.ImportedFieldReport, MasofaCropMonitoringDbContext>()
                {
                    Id = request.ImportedFieldReportId,
                    Author = request.Author
                }, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                var msg = $"Something wrong in {requestPath}. {ex.Message}";
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw;
            }
        }

        private static void SetPropertyValue(object obj, string propertyName, object value)
        {
            ArgumentNullException.ThrowIfNull(obj);
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name is null or empty", nameof(propertyName));
            }

            Type type = obj.GetType();
            PropertyInfo? prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Property '{propertyName}' not found on {type.Name}");

            if (!prop.CanWrite)
            {
                throw new InvalidOperationException($"Property '{propertyName}' is read-only");
            }

            Type targetType = prop.PropertyType;
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            object? convertedValue = null;

            if (value == null)
            {
                if (targetType.IsValueType && !IsNullable(targetType))
                {
                    throw new InvalidCastException($"Cannot assign null to non-nullable property '{propertyName}'");
                }

                convertedValue = null;
            }
            else
            {
                if (underlyingType == typeof(DateOnly))
                {
                    if (value is DateOnly dateOnly)
                    {
                        convertedValue = dateOnly;
                    }
                    else if (value is DateTime dt)
                    {
                        convertedValue = DateOnly.FromDateTime(dt);
                    }
                    else if (value is string dateString && !string.IsNullOrWhiteSpace(dateString))
                    {
                        if (DateOnly.TryParse(dateString, out var parsedDate))
                        {
                            convertedValue = parsedDate;
                        }
                        else
                        {
                            throw new FormatException($"Cannot parse '{dateString}' to DateOnly");
                        }
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot convert {value.GetType()} to DateOnly");
                    }
                }
                else if (underlyingType == typeof(Guid))
                {
                    if (value is Guid g)
                    {
                        convertedValue = g;
                    }
                    else if (value is string guidStr)
                    {
                        guidStr = guidStr?.Trim();
                        if (string.IsNullOrEmpty(guidStr))
                        {
                            if (!IsNullable(targetType))
                            {
                                throw new ArgumentException($"Cannot assign null to non-nullable Guid property");
                            }

                            convertedValue = null;
                        }
                        else if (Guid.TryParse(guidStr, out var parsedGuid))
                        {
                            convertedValue = parsedGuid;
                        }
                        else
                        {
                            throw new FormatException($"Invalid GUID format: '{guidStr}'");
                        }
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot convert {value?.GetType()} to Guid");
                    }
                }
                else if (underlyingType.IsEnum)
                {
                    if (value is string str)
                    {
                        try
                        {
                            convertedValue = Enum.Parse(underlyingType, str, ignoreCase: true);
                        }
                        catch (ArgumentException)
                        {
                            throw new InvalidCastException($"Invalid value '{str}' for enum {underlyingType.Name}");
                        }
                    }
                    else if (value is IConvertible convertible)
                    {
                        try
                        {
                            var numericValue = convertible.ToInt32(CultureInfo.InvariantCulture);
                            if (Enum.IsDefined(underlyingType, numericValue))
                            {
                                convertedValue = Enum.ToObject(underlyingType, numericValue);
                            }
                            else
                            {
                                throw new InvalidCastException($"Value {numericValue} is not defined in enum {underlyingType.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidCastException($"Cannot convert {value} to {underlyingType.Name}", ex);
                        }
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot convert {value?.GetType()} to enum {underlyingType.Name}");
                    }
                }
                else
                {
                    try
                    {
                        convertedValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Failed to convert {value} ({value.GetType()}) to {underlyingType.Name}", ex);
                    }
                }

                if (!IsNullable(targetType) && convertedValue == null)
                {
                    throw new InvalidCastException($"Cannot assign null to non-nullable property '{propertyName}'");
                }
            }

            prop.SetValue(obj, convertedValue);
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    /// <summary>
    /// Результат миграции загруженного поля
    /// </summary>
    public class MigrationImportedFieldResult
    {
        /// <summary>
        /// Мигрированное загруженное поле
        /// </summary>
        public Guid ImportedFieldId { get; set; }

        /// <summary>
        /// Поле куда добавился посев
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Новый посев
        /// </summary>
        public Guid SeasonId { get; set; }

        /// <summary>
        /// Флаг что миграция прошла успешно
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Ошибки при импорте
        /// </summary>
        public List<string> Errors { get; set; } = [];
    }
}
