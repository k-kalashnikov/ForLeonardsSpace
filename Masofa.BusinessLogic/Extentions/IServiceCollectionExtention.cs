using Masofa.BusinessLogic;
using Masofa.BusinessLogic.AnaliticReport;
using Masofa.BusinessLogic.Common.BusinessLogic;
using Masofa.BusinessLogic.Common.History;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Reports;
using Masofa.Common.Models;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.Reports;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtention
    {
        //Дженери больше обрабатываются автоматом через RegisterServicesFromAssembly, поэтому хендлеры для базовых команд и запросов нужно ручками в DI закинуть
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            // Регистрация хелперов для спутникового поиска
            // ProductMappingHelper теперь статический в Masofa.Common.Helper

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(BaseGetRequest<,>).Assembly);

                cfg.RegisterServicesFromAssembly(typeof(BuildFarmerReportCommand).Assembly);

                cfg.RegisterServicesFromAssembly(typeof(StartQwenAnalysisCommand).Assembly);
            });

            // Регистрируем хендлер для сводной информации справочников
            services.AddTransient<IRequestHandler<DictionarySummaryRequest, DictionarySummaryResponse>, DictionarySummaryRequestHandler>();

            var assembly = typeof(BaseEntity).Assembly;

            var baseEntityTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)));

            foreach (var baseEntityType in baseEntityTypes)
            {
                var tempSqp = baseEntityType.FullName;
                var dbContextType = GetDbContextType(baseEntityType);
                if (dbContextType == null)
                {
                    continue;
                }

                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseGetRequest<,>).MakeGenericType(baseEntityType, dbContextType), typeof(List<>).MakeGenericType(baseEntityType)), typeof(BaseGetRequestHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseGetByIdRequest<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType), typeof(BaseGetByIdRequestHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseUpdateCommand<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType), typeof(BaseUpdateCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseCreateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Guid)), typeof(BaseCreateCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseDeleteCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(bool)), typeof(BaseDeleteCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseImportFromCSVCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)), typeof(BaseImportFromCSVCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseExportToExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(BaseExportToExcelCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseImportFromExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)), typeof(BaseImportFromExcelCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseImportCsvTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(BaseImportCsvTemplateCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseImportExcelTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(BaseImportExcelTemplateCommandHandler<,>).MakeGenericType(baseEntityType, dbContextType));

                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseCreateEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(BusinessLogicCreateEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseUpdateEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(BusinessLogicUpdateEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseDeleteEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(BusinessLogicDeleteEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));

                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseCreateEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(HistoryBaseCreateEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseUpdateEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(HistoryBaseUpdateEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));
                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(typeof(BaseDeleteEvent<,>).MakeGenericType(baseEntityType, dbContextType)), typeof(HistoryBaseDeleteEventHandler<,>).MakeGenericType(baseEntityType, dbContextType));

                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseGetRequest<,>).MakeGenericType(baseEntityType, dbContextType), typeof(List<>).MakeGenericType(baseEntityType)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseGetRequest<,>).MakeGenericType(baseEntityType, dbContextType), typeof(List<>).MakeGenericType(baseEntityType)));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseGetByIdRequest<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseGetByIdRequest<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseUpdateCommand<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseUpdateCommand<,>).MakeGenericType(baseEntityType, dbContextType), baseEntityType));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseCreateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Guid)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseCreateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Guid)));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseDeleteCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(bool)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseDeleteCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(bool)));

                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseImportFromCSVCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseImportFromCSVCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseExportToExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseExportToExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseImportFromExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseImportFromExcelCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(Unit)));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseImportCsvTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseImportCsvTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])));
                services.AddTransient(typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseImportExcelTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseImportExcelTemplateCommand<,>).MakeGenericType(baseEntityType, dbContextType), typeof(byte[])));
            }

            var baseIndexesTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseIndex)));

            //foreach (var baseIndexesType in baseIndexesTypes)
            //{
            //    services.AddTransient(
            //        typeof(IRequestHandler<,>).MakeGenericType(
            //            typeof(BaseIndexesGetRequest<>).MakeGenericType(baseIndexesType), 
            //            typeof(List<>).MakeGenericType(baseIndexesType)
            //            ), 
            //        typeof(BaseGetRequestHandler<,>).MakeGenericType(baseIndexesType));
            //}

            var baseEra5WeatherReportTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEra5WeatherReport)));
            foreach (var baseEra5WeatherReportType in baseEra5WeatherReportTypes)
            {
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseEra5WeatherReportGetRequest<>).MakeGenericType(baseEra5WeatherReportType),
                    typeof(List<>).MakeGenericType(baseEra5WeatherReportType)), typeof(BaseEra5WeatherReportGetRequestHandler<>).MakeGenericType(baseEra5WeatherReportType)
                    );

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseEra5WeatherReportGetRequest<>).MakeGenericType(baseEra5WeatherReportType),
                    typeof(List<>).MakeGenericType(baseEra5WeatherReportType)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseEra5WeatherReportGetRequest<>).MakeGenericType(baseEra5WeatherReportType), typeof(List<>).MakeGenericType(baseEra5WeatherReportType))
                    );
            }

            var baseIndexReportTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseIndexReport)));
            foreach (var baseIndexReportType in baseIndexReportTypes)
            {
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseGetIndexReportRequest<>).MakeGenericType(baseIndexReportType),
                    typeof(List<>).MakeGenericType(baseIndexReportType)), typeof(BaseGetIndexRequestHandler<>).MakeGenericType(baseIndexReportType)
                    );

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseGetIndexReportRequest<>).MakeGenericType(baseIndexReportType),
                    typeof(List<>).MakeGenericType(baseIndexReportType)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseGetIndexReportRequest<>).MakeGenericType(baseIndexReportType), typeof(List<>).MakeGenericType(baseIndexReportType))
                    );
            }

            // Регистрируем handlers для GetIndexAverageByRegionAndDateRangeRequest для всех типов IndexReportShared
            var indexReportSharedTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(IndexReportShared)));
            foreach (var indexReportSharedType in indexReportSharedTypes)
            {
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        typeof(GetIndexAverageByRegionAndDateRangeRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(Dictionary<DateOnly, double>)
                    ),
                    typeof(GetIndexAverageByRegionAndDateRangeRequestHandler<>).MakeGenericType(indexReportSharedType)
                );

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(
                        typeof(GetIndexAverageByRegionAndDateRangeRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(Dictionary<DateOnly, double>)
                    ),
                    typeof(PermissionPipelineBehavior<,>).MakeGenericType(
                        typeof(GetIndexAverageByRegionAndDateRangeRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(Dictionary<DateOnly, double>)
                    )
                );

                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        typeof(GetIndexAvailableDatesRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(List<DateOnly>)
                    ),
                    typeof(GetIndexAvailableDatesRequestHandler<>).MakeGenericType(indexReportSharedType)
                );

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(
                        typeof(GetIndexAvailableDatesRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(List<DateOnly>)
                    ),
                    typeof(PermissionPipelineBehavior<,>).MakeGenericType(
                        typeof(GetIndexAvailableDatesRequest<>).MakeGenericType(indexReportSharedType),
                        typeof(List<DateOnly>)
                    )
                );
            }

            var baseIndexTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseIndex)));
            foreach (var baseIndextType in baseIndexTypes)
            {
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseIndexesGetRequest<>).MakeGenericType(baseIndextType),
                    typeof(List<>).MakeGenericType(baseIndextType)), typeof(BaseIndexesGetRequestHandler<>).MakeGenericType(baseIndextType)
                    );

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseIndexesGetRequest<>).MakeGenericType(baseIndextType),
                    typeof(List<>).MakeGenericType(baseIndextType)), typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseIndexesGetRequest<>).MakeGenericType(baseIndextType), typeof(List<>).MakeGenericType(baseIndextType))
                    );
            }

            var baseReportTypes = new List<Type>();
            var allTypes = assembly.GetTypes().ToList();
            foreach (var item in allTypes)
            {
                if (item.IsAbstract)
                {
                    continue;
                }
                if (!(item.BaseType?.IsGenericType ?? false))
                {
                    continue;
                }
                if (item.BaseType?.GetGenericTypeDefinition() == typeof(BaseAIReportEntity<>))
                {
                    baseReportTypes.Add(item);
                }

                if (item.BaseType?.GetGenericTypeDefinition() == typeof(BaseReportEntity<>))
                {
                    baseReportTypes.Add(item);
                }
            }

            foreach (var baseReportType in baseReportTypes)
            {
                var dbContextType = GetDbContextType(baseReportType);
                if (dbContextType == null)
                {
                    continue;
                }

                if (baseReportType.BaseType == null)
                {
                    continue;
                }

                var reportModelType = baseReportType.BaseType.GetGenericArguments()[0];

                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(typeof(BaseReportPrintCommand<,,>).MakeGenericType(baseReportType, reportModelType, dbContextType), typeof(byte[])),
                    typeof(BaseReportPrintCommandHandler<,,>).MakeGenericType(baseReportType, reportModelType, dbContextType));

                services.AddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(typeof(BaseReportPrintCommand<,,>).MakeGenericType(baseReportType, reportModelType, dbContextType), typeof(byte[])),
                    typeof(PermissionPipelineBehavior<,>).MakeGenericType(typeof(BaseReportPrintCommand<,,>).MakeGenericType(baseReportType, reportModelType, dbContextType), typeof(byte[])));
            }

            services.AddTransient<
                IRequestHandler<BuildFarmerReportCommand, BuildFarmerReportResult>,
                BuildFarmerReportHandler>();

            services.AddSingleton(sp =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();

                var templatesRoot = Path.Combine(env.ContentRootPath, "..", "Masofa.Common", "Templates", "FarmerReportTemplates");

                if (!Directory.Exists(templatesRoot))
                    throw new DirectoryNotFoundException($"Templates folder not found: {templatesRoot}");

                return new RazorLight.RazorLightEngineBuilder()
                    .UseFileSystemProject(templatesRoot)
                    .EnableDebugMode()
                    .UseMemoryCachingProvider()
                    .Build();
            });

            return services;
        }

        private static Type GetDbContextType(Type entityType)
        {
            var dbContextTypes = typeof(MasofaIdentityDbContext).Assembly
                .GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DbContext)));

            foreach (var dbContextType in dbContextTypes)
            {
                var dbSetProps = dbContextType.GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(p => p.PropertyType.GetGenericArguments()[0]);

                if (dbSetProps.Contains(entityType))
                {
                    return dbContextType;
                }
            }

            return null;
        }
    }
}
