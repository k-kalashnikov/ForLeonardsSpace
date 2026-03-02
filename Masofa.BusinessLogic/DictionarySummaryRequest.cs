using MediatR;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Masofa.Common.Models;

namespace Masofa.BusinessLogic
{
    public class DictionarySummaryRequest : IRequest<DictionarySummaryResponse>
    {
    }

    public class DictionarySummaryResponse
    {
        public List<DictionaryGroup> Groups { get; set; } = new List<DictionaryGroup>();
        public int TotalDictionaries { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

    public class DictionaryGroup
    {
        public string GroupKey { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<DictionaryInfo> Items { get; set; } = new List<DictionaryInfo>();
        public int TotalRecords { get; set; }
    }

    public class DictionaryInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public string GroupKey { get; set; } = string.Empty;
    }

    public class DictionarySummaryRequestHandler : IRequestHandler<DictionarySummaryRequest, DictionarySummaryResponse>
    {
        private readonly MasofaDictionariesDbContext _dbContext;

        public DictionarySummaryRequestHandler(MasofaDictionariesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DictionarySummaryResponse> Handle(DictionarySummaryRequest request, CancellationToken cancellationToken)
        {
            var response = new DictionarySummaryResponse();
            
            // Статические группы на основе DICTIONARY_SECTIONS_DATA
            var groups = new List<DictionaryGroup>
            {
                new DictionaryGroup
                {
                    GroupKey = "regions",
                    Title = "regions",
                    Items = new List<DictionaryInfo>
                    {
                        new DictionaryInfo
                        {
                            Id = "regions",
                            Title = "regions",
                            Url = "/dictionaries/regions",
                            RecordCount = await GetRecordCountForType("Region", cancellationToken),
                            GroupKey = "regions"
                        },
                        new DictionaryInfo
                        {
                            Id = "administrative-units",
                            Title = "administrativeUnits",
                            Url = "/dictionaries/administrative-units",
                            RecordCount = await GetRecordCountForType("AdministrativeUnit", cancellationToken),
                            GroupKey = "regions"
                        },
                        new DictionaryInfo
                        {
                            Id = "region-types",
                            Title = "Region Types",
                            Url = "/dictionaries/region-types",
                            RecordCount = await GetRecordCountForType("RegionType", cancellationToken),
                            GroupKey = "regions"
                        }
                    }
                },
                new DictionaryGroup
                {
                    GroupKey = "climate-data",
                    Title = "climateData",
                    Items = new List<DictionaryInfo>
                    {
                        new DictionaryInfo
                        {
                            Id = "agroclimatic-zones",
                            Title = "Agroclimatic Zones",
                            Url = "/dictionaries/agroclimatic-zones",
                            RecordCount = await GetRecordCountForType("AgroclimaticZone", cancellationToken),
                            GroupKey = "climate-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "climatic-standards",
                            Title = "Climatic Standards",
                            Url = "/dictionaries/climatic-standards",
                            RecordCount = await GetRecordCountForType("ClimaticStandard", cancellationToken),
                            GroupKey = "climate-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "weather-condition-providers",
                            Title = "Weather Condition Providers",
                            Url = "/dictionaries/weather-condition-providers",
                            RecordCount = await GetRecordCountForType("WeatherConditionProvider", cancellationToken),
                            GroupKey = "climate-data"
                        }
                    }
                },
                new DictionaryGroup
                {
                    GroupKey = "general-dictionaries",
                    Title = "sharedDictionaries",
                    Items = new List<DictionaryInfo>
                    {
                        new DictionaryInfo
                        {
                            Id = "application-contents-reference",
                            Title = "Application Contents Reference",
                            Url = "/dictionaries/application-contents-reference",
                            RecordCount = await GetRecordCountForType("ApplicationContentsReference", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "application-statuses",
                            Title = "Application Statuses",
                            Url = "/dictionaries/application-statuses",
                            RecordCount = await GetRecordCountForType("ApplicationStatus", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "application-types",
                            Title = "Application Types",
                            Url = "/dictionaries/application-types",
                            RecordCount = await GetRecordCountForType("ApplicationType", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "dictionary-types",
                            Title = "Dictionary types",
                            Url = "/dictionaries/dictionary-types",
                            RecordCount = await GetRecordCountForType("DicitonaryType", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "flight-targets",
                            Title = "flight_targets",
                            Url = "/dictionaries/flight-targets",
                            RecordCount = await GetRecordCountForType("FlightTarget", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "measurement-units",
                            Title = "Measurement units",
                            Url = "/dictionaries/measurement-units",
                            RecordCount = await GetRecordCountForType("MeasurementUnit", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "system-data-sources",
                            Title = "System Data Sources",
                            Url = "/dictionaries/system-data-sources",
                            RecordCount = await GetRecordCountForType("SystemDataSource", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "task-statuses",
                            Title = "Task statuses",
                            Url = "/dictionaries/task-statuses",
                            RecordCount = await GetRecordCountForType("TaskStatus", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "uav-camera-types",
                            Title = "UAV Camera Types",
                            Url = "/dictionaries/uav-camera-types",
                            RecordCount = await GetRecordCountForType("UavCameraType", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "uav-data-types",
                            Title = "UAV Data Types",
                            Url = "/dictionaries/uav-data-types",
                            RecordCount = await GetRecordCountForType("UavDataType", cancellationToken),
                            GroupKey = "general-dictionaries"
                        },
                        new DictionaryInfo
                        {
                            Id = "tags",
                            Title = "Tags",
                            Url = "/dictionaries/tags",
                            RecordCount = await GetRecordCountForType("Tag", cancellationToken),
                            GroupKey = "general-dictionaries"
                        }
                    }
                },
                new DictionaryGroup
                {
                    GroupKey = "natural-resources",
                    Title = "naturalResources",
                    Items = new List<DictionaryInfo>
                    {
                        new DictionaryInfo
                        {
                            Id = "water-resources",
                            Title = "Water resources",
                            Url = "/dictionaries/water-resources",
                            RecordCount = await GetRecordCountForType("WaterResource", cancellationToken),
                            GroupKey = "natural-resources"
                        },
                        new DictionaryInfo
                        {
                            Id = "irrigation-sources",
                            Title = "Irrigation Sources",
                            Url = "/dictionaries/irrigation-sources",
                            RecordCount = await GetRecordCountForType("IrrigationSource", cancellationToken),
                            GroupKey = "natural-resources"
                        },
                        new DictionaryInfo
                        {
                            Id = "soil-types",
                            Title = "Soil Types",
                            Url = "/dictionaries/soil-types",
                            RecordCount = await GetRecordCountForType("SoilType", cancellationToken),
                            GroupKey = "natural-resources"
                        }
                    }
                },
                new DictionaryGroup
                {
                    GroupKey = "agro-data",
                    Title = "agriculturalData",
                    Items = new List<DictionaryInfo>
                    {
                        new DictionaryInfo
                        {
                            Id = "agro-machine-types",
                            Title = "Agricultural Machine Types",
                            Url = "/dictionaries/agro-machine-types",
                            RecordCount = await GetRecordCountForType("AgroMachineType", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "agro-operations",
                            Title = "Сельскохозяйственные операции",
                            Url = "/dictionaries/agro-operations",
                            RecordCount = await GetRecordCountForType("AgroOperation", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "agro-terms",
                            Title = "Агротермины",
                            Url = "/dictionaries/agro-terms",
                            RecordCount = await GetRecordCountForType("AgroTerm", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "agrotechnical-measures",
                            Title = "Агротехнические мероприятия",
                            Url = "/dictionaries/agrotechnical-measures",
                            RecordCount = await GetRecordCountForType("AgrotechnicalMeasure", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "crop-development-period-indices",
                            Title = "Crop Development Period Indices",
                            Url = "/dictionaries/crop-development-period-indices",
                            RecordCount = await GetRecordCountForType("CropDevelopmentPeriodIndex", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "crop-development-periods",
                            Title = "Crop Development Periods",
                            Url = "/dictionaries/crop-development-periods",
                            RecordCount = await GetRecordCountForType("CropDevelopmentPeriod", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "crops",
                            Title = "Crops",
                            Url = "/dictionaries/crops",
                            RecordCount = await GetRecordCountForType("Crop", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "diseases",
                            Title = "Diseases",
                            Url = "/dictionaries/diseases",
                            RecordCount = await GetRecordCountForType("Disease", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "entomophage-types",
                            Title = "Entomophage type",
                            Url = "/dictionaries/entomophage-types",
                            RecordCount = await GetRecordCountForType("EntomophageType", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "experimental-farming-methods",
                            Title = "Experimental farming methods",
                            Url = "/dictionaries/experimental-farming-methods",
                            RecordCount = await GetRecordCountForType("ExperimentalFarmingMethod", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "fertilizers",
                            Title = "Fertilizers",
                            Url = "/dictionaries/fertilizers",
                            RecordCount = await GetRecordCountForType("Fertilizer", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "field-usage-statuses",
                            Title = "Field usage statuses",
                            Url = "/dictionaries/field-usage-statuses",
                            RecordCount = await GetRecordCountForType("FieldUsageStatus", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "irrigation-methods",
                            Title = "Irrigation methods",
                            Url = "/dictionaries/irrigation-methods",
                            RecordCount = await GetRecordCountForType("IrrigationMethod", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "land-improvement-measures",
                            Title = "Land improvement measures",
                            Url = "/dictionaries/land-improvement-measures",
                            RecordCount = await GetRecordCountForType("LandImprovementMeasure", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "pest-types",
                            Title = "Pest Types",
                            Url = "/dictionaries/pest-types",
                            RecordCount = await GetRecordCountForType("PestType", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "pesticides",
                            Title = "Pesticides",
                            Url = "/dictionaries/pesticides",
                            RecordCount = await GetRecordCountForType("Pesticide", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "varieties",
                            Title = "Varieties",
                            Url = "/dictionaries/varieties",
                            RecordCount = await GetRecordCountForType("Variety", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "variety-characteristics",
                            Title = "Variety Characteristics",
                            Url = "/dictionaries/variety-characteristics",
                            RecordCount = await GetRecordCountForType("VarietyCharacteristic", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "vegetation-indices",
                            Title = "Vegetation Indices",
                            Url = "/dictionaries/vegetation-indices",
                            RecordCount = await GetRecordCountForType("VegetationIndex", cancellationToken),
                            GroupKey = "agro-data"
                        },
                        new DictionaryInfo
                        {
                            Id = "vegetation-periods-reference",
                            Title = "Vegetation Periods Reference",
                            Url = "/dictionaries/vegetation-periods-reference",
                            RecordCount = await GetRecordCountForType("VegetationPeriodsReference", cancellationToken),
                            GroupKey = "agro-data"
                        }
                    }
                }
            };

            // Подсчитываем статистику
            var allDictionaries = groups.SelectMany(g => g.Items).ToList();
            foreach (var group in groups)
            {
                group.TotalRecords = group.Items.Sum(d => d.RecordCount);
            }

            response.TotalDictionaries = allDictionaries.Count;
            response.Groups = groups;
            response.LastUpdateTime = DateTime.UtcNow;
            
            return response;
        }

        private async Task<int> GetRecordCountForType(string dictionaryTypeName, CancellationToken cancellationToken)
        {
            try
            {
                var dbSetProperties = typeof(MasofaDictionariesDbContext)
                    .GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && 
                               p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

                var targetDbSet = dbSetProperties.FirstOrDefault(p => 
                    p.PropertyType.GetGenericArguments()[0].Name == dictionaryTypeName);

                if (targetDbSet != null)
                {
                    var dbSet = targetDbSet.GetValue(_dbContext) as IQueryable;
                    if (dbSet != null)
                    {
                        var countMethod = typeof(EntityFrameworkQueryableExtensions)
                            .GetMethod("CountAsync", new[] { typeof(IQueryable<>).MakeGenericType(targetDbSet.PropertyType.GetGenericArguments()[0]), typeof(CancellationToken) });
                        
                        if (countMethod != null)
                        {
                            var task = countMethod.Invoke(null, new object[] { dbSet, cancellationToken }) as Task<int>;
                            return await task;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting count for {dictionaryTypeName}: {ex.Message}");
            }

            return 0;
        }
    }
}
