using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Common.EmailSender;
using Masofa.BusinessLogic.Services;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Copernicus;
using Masofa.Client.EarthExplorer;
using Masofa.Client.Era5;
using Masofa.Client.IBMWeather;
using Masofa.Client.OneId;
using Masofa.Client.Qwen;
using Masofa.Client.Ugm;
using Masofa.Common;
using Masofa.Common.Converters;
using Masofa.Common.Helper;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services;
using Masofa.Common.Services.EmailSender;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne;
using Masofa.Web.Monolith.Helpers;
using Masofa.Web.Monolith.Jobs.AnaliticReport;
using Masofa.Web.Monolith.Jobs.IBMWeather;
using Masofa.Web.Monolith.Jobs.QwenAnalysis;
using Masofa.Web.Monolith.Jobs.SatelliteIndices;
using Masofa.Web.Monolith.Jobs.Sentinel2;
using Masofa.Web.Monolith.Jobs.System;
using Masofa.Web.Monolith.Jobs.Weather;
using Masofa.Web.Monolith.Jobs.WeatherReport;
using Masofa.Web.Monolith.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using NetTopologySuite;
using Quartz;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtention
    {
        public static List<Action<IServiceCollectionQuartzConfigurator>> QuartConfigJobs = new List<Action<IServiceCollectionQuartzConfigurator>>();
        public static IServiceCollection AddConfigurableControllers(this IServiceCollection services, IConfiguration configuration)
        {
            var mvcBuilder = services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new UtcDateTimeConverter());
                });
            var assembly = Assembly.GetExecutingAssembly();

            var foldersToInclude = new List<string>();
            foldersToInclude.Add("Controllers.Common");
            foldersToInclude.Add("Controllers.Notifications");

            if (configuration.GetValue<bool>("MonolithConfiguration:StartIdentity"))
            {
                foldersToInclude.Add("Controllers.Identity");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartDictionaries"))
            {
                foldersToInclude.Add("Controllers.Dictionaries");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartCropMonitoring"))
            {
                foldersToInclude.Add("Controllers.CropMonitoring");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteSentinel"))
            {
                foldersToInclude.Add("Controllers.Satellite.Sentinel");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteLandsat"))
            {
                foldersToInclude.Add("Controllers.Satellite.Landsat");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartIndices"))
            {
                foldersToInclude.Add("Controllers.Satellite.Indices");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartTiles"))
            {
                foldersToInclude.Add("Controllers.Tiles");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartWeather"))
            {
                foldersToInclude.Add("Controllers.Weather");
                foldersToInclude.Add("Controllers.WeatherReports");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartIBM"))
            {
                foldersToInclude.Add("Controllers.IBMWeather");
            }

            if (configuration.GetValue<bool>("MonolithConfiguration:StartUav"))
            {
                foldersToInclude.Add("Controllers.Uav");
            }

            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                manager.ApplicationParts.Add(new AssemblyPart(assembly));
                foreach (var folder in foldersToInclude)
                {
                    manager.FeatureProviders.Add(new ConfigurableControllerFeatureProvider(folder));
                }
            });

            return services;
        }
        public static void AddJWTAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var authConfig = configuration.GetSection("AuthOptions").Get<AuthOptions>();

            if (authConfig == null)
            {
                throw new ArgumentNullException(nameof(authConfig));
            }

            services.AddOptions<AuthOptions>("AuthOptions");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuer = authConfig.ISSUER,
                    ValidateAudience = false,
                    ValidAudience = authConfig.AUDIENCE,
                    //ValidateLifetime = false,
                    IssuerSigningKey = authConfig.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = false,
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Challenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Challenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });
        }
        public static void AddDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(cfg =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                cfg.IncludeXmlComments(xmlPath);

                var foldersToInclude = new List<(string, string)>();
                foldersToInclude.Add(("Common", "Common"));
                foldersToInclude.Add(("Notifications", "Notifications"));

                if (configuration.GetValue<bool>("MonolithConfiguration:StartIdentity"))
                {
                    foldersToInclude.Add(("Identity", "Identity"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartDictionaries"))
                {
                    foldersToInclude.Add(("Dictionaries", "Dictionaries"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartCropMonitoring"))
                {
                    foldersToInclude.Add(("CropMonitoring", "Crop Monitoring"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteSentinel"))
                {
                    foldersToInclude.Add(("SatelliteSentinel", "Satellite Sentinel"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartSatelliteLandsat"))
                {
                    foldersToInclude.Add(("SatelliteLandsat", "Satellite Landsat"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartTiles"))
                {
                    foldersToInclude.Add(("Tiles", "Smart field map"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartWeather"))
                {
                    foldersToInclude.Add(("Weather", "Weather"));
                    foldersToInclude.Add(("WeatherReports", "WeatherReports"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartIndices"))
                {
                    foldersToInclude.Add(("SatelliteIndices", "Satillite Indices"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartIBM"))
                {
                    foldersToInclude.Add(("IBMWeather", "IBM Weather"));
                }

                if (configuration.GetValue<bool>("MonolithConfiguration:StartUav"))
                {
                    foldersToInclude.Add(("Uav", "Uav"));
                }

                foreach (var folder in foldersToInclude)
                {
                    cfg.SwaggerDoc(folder.Item1, new OpenApiInfo { Title = $"{folder.Item2} API", Version = "v2.0" });
                }

                cfg.DocInclusionPredicate((docName, description) =>
                {
                    if (!description.TryGetMethodInfo(out var methodInfo))
                    {
                        return false;
                    }

                    var controllerType = methodInfo.ReflectedType;
                    if (controllerType == null)
                    {
                        return false;
                    }

                    var aesAttr = controllerType.GetCustomAttribute<ApiExplorerSettingsAttribute>();
                    if (aesAttr == null)
                    {
                        return false;
                    }

                    if (string.IsNullOrEmpty(aesAttr.GroupName))
                    {
                        return false;
                    }


                    return aesAttr.GroupName.ToLower().Equals(docName.ToLower());
                });

                var assem = Assembly.GetEntryAssembly();
                //c.IncludeXmlComments(assem);
                cfg.OperationFilter<FilterFieldsOperationFilter>();
                cfg.SchemaFilter<StringEnumSchemaFilter>();
                cfg.SchemaFilter<LocalizationStringSchemaFilter>();

                var securityScheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен в формате: Bearer {your-token}",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                cfg.AddSecurityDefinition("Bearer", securityScheme);

                // Требование использования токена для всех методов
                cfg.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
                });
            });
        }
        public static void AddAllowCORS(this IServiceCollection services, IConfiguration configuration)
        {
            var allowCors = configuration.GetSection("AllowCORS").Get<string[]>() ?? (new List<string>()).ToArray();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(allowCors)  // Разрешаем все источники (домены)
                          .AllowAnyMethod() // Разрешаем все методы HTTP (GET, POST, PUT, DELETE и т.д.)
                          .AllowAnyHeader() // Разрешаем все заголовки
                          .AllowCredentials();
                });
            });
        }
        public static void AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
        {

            QuartConfigJobs.Add(qConf =>
            {
                var jobPartialKey = new JobKey("createPartitionJob", "system");

                qConf.AddJob<CreatePartitionJob>(jobOpt => jobOpt.WithIdentity(jobPartialKey));

                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobPartialKey)
                    .WithIdentity("createPartitionTrigger", "system")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobHealthKey = new JobKey("healthCheckJob", "system");

                qConf.AddJob<HealthCheckJob>(jobOpt => jobOpt.WithIdentity(jobHealthKey));

                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobHealthKey)
                    .WithIdentity("healthCheckJob", "system")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));
            });

            var startJobs = configuration.GetValue<bool>("MonolithConfiguration:StartJobs");

            services.AddQuartz(qConf =>
            {
                qConf.SchedulerId = "Masofa";
                qConf.SchedulerName = "Masofa Crop Monitoring";

                if (!startJobs)
                {
                    return;
                }

                qConf.UsePersistentStore(store =>
                {
                    store.UseNewtonsoftJsonSerializer();
                    store.UsePostgres(sql =>
                    {
                        sql.ConnectionString = configuration.GetConnectionString("PgSqlQuartZConnection");
                    });
                });

                foreach (var item in QuartConfigJobs)
                {
                    item.Invoke(qConf);
                }
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }
        public static void AddAllDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрируем ВСЕ контексты ВСЕГДА
            services.AddDbContext<MasofaCommonDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PgSqlCommonConnection"), x => x.UseNetTopologySuite()));

            services.AddDbContext<MasofaIdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PgSqlIdentityConnection")));

            services.AddDbContext<MasofaDictionariesDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PgSqlDictionaryConnection"), x => x.UseNetTopologySuite()));

            services.AddDbContext<MasofaCropMonitoringDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlBidsConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaSentinelDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PgSqlSentinelConnection")));

            services.AddDbContext<MasofaLandsatDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PgSqlLandsatConnection")));

            services.AddDbContext<MasofaWeatherDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlWeatherConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaTileDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlTilesConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaEraDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlEraConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaIndicesDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlIndicesConnection"), x => x.UseNetTopologySuite());
            });


            services.AddDbContext<MasofaAnaliticReportDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlAnaliticReportConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaUgmDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlUgmConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaIBMWeatherDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlIBMConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaUAVDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlUAVConnection"), x => x.UseNetTopologySuite());
            });

            services.AddDbContext<MasofaDictionariesHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlDictionaryHistoryConnection"));
            });

            services.AddDbContext<MasofaCropMonitoringHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlCropMonitoringHistoryConnection"));
            });

            services.AddDbContext<MasofaAnaliticReportHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlAnaliticReportHistoryConnection"));
            });

            services.AddDbContext<MasofaCommonHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlCommonHistoryConnection"));
            });

            services.AddDbContext<MasofaIdentityHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlIdentityHistoryConnection"));
            });

            services.AddDbContext<MasofaLandsatHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlLandsatHistoryConnection"));
            });

            services.AddDbContext<MasofaSentinelHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlSentinelHistoryConnection"));
            });

            services.AddDbContext<MasofaTileHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlTilesHistoryConnection"));
            });

            services.AddDbContext<MasofaWeatherHistoryDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PgSqlWeatherHistoryConnection"));
            });

            // Deprecated контексты регистрируем условно
            services.AddDepricatedDataAccess(configuration);
        }
        public static void AddCommon(this IServiceCollection services, IConfiguration configuration)
        {

            GdalInitializer.Initialize();
            services.AddSingleton<GdalInitializer>();
            services.AddSingleton<IHealthCheckLocalizationProvider, HealthCheckLocalizationProvider>();
            services.AddSingleton<IModuleLocalizationService, ModuleLocalizationService>();

            QuartConfigJobs.Add(qConf =>
            {


                #region IndicesDB
                //EviDbJob
                //var jobEviKey = new JobKey("EviDbJob", "indices");
                //qConf.AddJob<EviDbJob>(jobOpt => jobOpt.WithIdentity(jobEviKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobEviKey)
                //    .WithIdentity("EviDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// GndviDbJob
                //var jobGndviKey = new JobKey("GndviDbJob", "indices");
                //qConf.AddJob<GndviDbJob>(jobOpt => jobOpt.WithIdentity(jobGndviKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobGndviKey)
                //    .WithIdentity("GndviDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// MndwiDbJob
                //var jobMndwiKey = new JobKey("MndwiDbJob", "indices");
                //qConf.AddJob<MndwiDbJob>(jobOpt => jobOpt.WithIdentity(jobMndwiKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobMndwiKey)
                //    .WithIdentity("MndwiDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// NdmiDbJob
                //var jobNdmiKey = new JobKey("NdmiDbJob", "indices");
                //qConf.AddJob<NdmiDbJob>(jobOpt => jobOpt.WithIdentity(jobNdmiKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobNdmiKey)
                //    .WithIdentity("NdmiDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// NdvIbJob
                //var jobNdvIbKey = new JobKey("NdviDbJob", "indices");
                //qConf.AddJob<NdviDbJob>(jobOpt => jobOpt.WithIdentity(jobNdvIbKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobNdvIbKey)
                //    .WithIdentity("NdviDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// OrviDbJob
                //var jobOrviKey = new JobKey("OrviDbJob", "indices");
                //qConf.AddJob<OrviDbJob>(jobOpt => jobOpt.WithIdentity(jobOrviKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobOrviKey)
                //    .WithIdentity("OrviDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// OsaviDbJob
                //var jobOsaviKey = new JobKey("OsaviDbJob", "indices");
                //qConf.AddJob<OsaviDbJob>(jobOpt => jobOpt.WithIdentity(jobOsaviKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobOsaviKey)
                //    .WithIdentity("OsaviDbJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));
                #endregion

                #region IndicesTiff
                //// EVIJob
                //var jobEVIKey = new JobKey("EVIJob", "indices");
                //qConf.AddJob<EVIJob>(jobOpt => jobOpt.WithIdentity(jobEVIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobEVIKey)
                //    .WithIdentity("EVIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// GNDVIJob
                //var jobGNDVIKey = new JobKey("GNDVIJob", "indices");
                //qConf.AddJob<GNDVIJob>(jobOpt => jobOpt.WithIdentity(jobGNDVIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobGNDVIKey)
                //    .WithIdentity("GNDVIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// MNDWIJob
                //var jobMNDWIKey = new JobKey("MNDWIJob", "indices");
                //qConf.AddJob<MNDWIJob>(jobOpt => jobOpt.WithIdentity(jobMNDWIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobMNDWIKey)
                //    .WithIdentity("MNDWIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// NDMIJob
                //var jobNDMIKey = new JobKey("NDMIJob", "indices");
                //qConf.AddJob<NDMIJob>(jobOpt => jobOpt.WithIdentity(jobNDMIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobNDMIKey)
                //    .WithIdentity("NDMIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// NDVIJob
                //var jobNDVIKey = new JobKey("NDVIJob", "indices");
                //qConf.AddJob<NDVIJob>(jobOpt => jobOpt.WithIdentity(jobNDVIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobNDVIKey)
                //    .WithIdentity("NDVIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// ORVIJob
                //var jobORVIKey = new JobKey("ORVIJob", "indices");
                //qConf.AddJob<ORVIJob>(jobOpt => jobOpt.WithIdentity(jobORVIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobORVIKey)
                //    .WithIdentity("ORVIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                //// OSAVIJob
                //var jobOSAVIKey = new JobKey("OSAVIJob", "indices");
                //qConf.AddJob<OSAVIJob>(jobOpt => jobOpt.WithIdentity(jobOSAVIKey));
                //qConf.AddTrigger(triggerOpt => triggerOpt
                //    .ForJob(jobOSAVIKey)
                //    .WithIdentity("OSAVIJobTrigger", "indices")
                //    .WithSimpleSchedule(builder =>
                //    {
                //        builder.WithIntervalInHours(24);
                //        builder.RepeatForever();
                //    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));
                #endregion

                #region sentinel
                var jobS2SPKey = new JobKey("Sentinel2SearchProductJob", "sentinel");
                qConf.AddJob<Sentinel2SearchProductJob>(jobOpt => jobOpt.WithIdentity(jobS2SPKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2SPKey)
                    .WithIdentity("Sentinel2SearchProductJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobS2MetaKey = new JobKey("Sentinel2MetadataLoaderJob", "sentinel");
                qConf.AddJob<Sentinel2MetadataLoaderJob>(jobOpt => jobOpt.WithIdentity(jobS2MetaKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2MetaKey)
                    .WithIdentity("Sentinel2MetadataLoaderJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobS2MediaKey = new JobKey("Sentinel2MediaLoaderJob", "sentinel");
                qConf.AddJob<Sentinel2MediaLoaderJob>(jobOpt => jobOpt.WithIdentity(jobS2MediaKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2MediaKey)
                    .WithIdentity("Sentinel2MediaLoaderJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                // Sentinel2ArchiveParsingJob
                var jobS2ArchiveParsingKey = new JobKey("Sentinel2ArchiveParsingJob", "sentinel");
                qConf.AddJob<Sentinel2ArchiveParsingJob>(jobOpt => jobOpt.WithIdentity(jobS2ArchiveParsingKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2ArchiveParsingKey)
                    .WithIdentity("Sentinel2ArchiveParsingJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));


                // ParallelesPointAndTiffFetchJob
                var jobS2ParallelesPointAndTiffFetchKey = new JobKey("ParallelesPointAndTiffFetchJob", "sentinel");
                qConf.AddJob<ParallelesPointAndTiffFetchJob>(jobOpt => jobOpt.WithIdentity(jobS2ParallelesPointAndTiffFetchKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2ParallelesPointAndTiffFetchKey)
                    .WithIdentity("ParallelesPointAndTiffFetchJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));


                // AnomaliesPointAndTiffJob
                var jobAnomaliesKey = new JobKey("AnomaliesPointAndTiffJob", "sentinel");
                qConf.AddJob<AnomaliesPointAndTiffJob>(jobOpt => jobOpt.WithIdentity(jobAnomaliesKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobAnomaliesKey)
                    .WithIdentity("AnomaliesPointAndTiffJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(30)));


                // Sentinel2PreviewImageJob
                var jobS2PreviewKey = new JobKey("Sentinel2PreviewImageJob", "sentinel");
                qConf.AddJob<Sentinel2PreviewImageJob>(jobOpt => jobOpt.WithIdentity(jobS2PreviewKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2PreviewKey)
                    .WithIdentity("Sentinel2PreviewImageJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(30)));


                // Sentinel2ConvertTilesJob
                var jobS2ConvertTilesKey = new JobKey("Sentinel2ConvertTilesJob", "sentinel");
                qConf.AddJob<Sentinel2ConvertTilesJob>(jobOpt => jobOpt.WithIdentity(jobS2ConvertTilesKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobS2ConvertTilesKey)
                    .WithIdentity("Sentinel2ConvertTilesJobTrigger", "sentinel")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));
                #endregion

                #region Weather
                var jobEra5WeatherLoadKey = new JobKey("EraWeatherDataLoaderJob", "weatherreport");
                qConf.AddJob<EraWeatherDataLoaderJob>(jobOpt => jobOpt.WithIdentity(jobEra5WeatherLoadKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobEra5WeatherLoadKey)
                    .WithIdentity("EraWeatherDataLoaderJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobEra5WeatherReportsKey = new JobKey("Era5WeatherReportsCalculationJob", "weatherreport");
                qConf.AddJob<Era5WeatherReportsCalculationJob>(jobOpt => jobOpt.WithIdentity(jobEra5WeatherReportsKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobEra5WeatherReportsKey)
                    .WithIdentity("Era5WeatherReportsCalculationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddHours(1)));

                var jobEra5WeatherReportsTilesGenerationKey = new JobKey("Era5WeatherReportsTilesGenerationJob", "weatherreport");
                qConf.AddJob<Era5WeatherReportsTilesGenerationJob>(jobOpt => jobOpt.WithIdentity(jobEra5WeatherReportsTilesGenerationKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobEra5WeatherReportsTilesGenerationKey)
                    .WithIdentity("Era5WeatherReportsTilesGenerationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddHours(2)));

                var jobUgmWeatherCurrentDataLoaderKey = new JobKey("UgmWeatherCurrentDataLoaderJob", "weatherreport");
                qConf.AddJob<UgmWeatherCurrentDataLoaderJob>(jobOpt => jobOpt.WithIdentity(jobUgmWeatherCurrentDataLoaderKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobUgmWeatherCurrentDataLoaderKey)
                    .WithIdentity("UgmWeatherCurrentDataLoaderJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(3);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(5)));

                var jobUgmWeatherForecastDataLoaderKey = new JobKey("UgmWeatherForecastDataLoaderJob", "weatherreport");
                qConf.AddJob<UgmWeatherForecastDataLoaderJob>(jobOpt => jobOpt.WithIdentity(jobUgmWeatherForecastDataLoaderKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobUgmWeatherForecastDataLoaderKey)
                    .WithIdentity("UgmWeatherForecastDataLoaderJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobUgmCurrentWeatherTilesGenerationKey = new JobKey("UgmCurrentWeatherReportsTilesGenerationJob", "weatherreport");
                qConf.AddJob<UgmCurrentWeatherReportsTilesGenerationJob>(jobOpt => jobOpt.WithIdentity(jobUgmCurrentWeatherTilesGenerationKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobUgmCurrentWeatherTilesGenerationKey)
                    .WithIdentity("UgmCurrentWeatherReportsTilesGenerationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(3);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(10)));

                var jobUgmForecastWeatherTilesGenerationKey = new JobKey("UgmForecastWeatherReportsTilesGenerationJob", "weatherreport");
                qConf.AddJob<UgmForecastWeatherReportsTilesGenerationJob>(jobOpt => jobOpt.WithIdentity(jobUgmForecastWeatherTilesGenerationKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobUgmForecastWeatherTilesGenerationKey)
                    .WithIdentity("UgmForecastWeatherReportsTilesGenerationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(20)));

                var jobLoadForecastDataKey = new JobKey("LoadForecastDataJob", "weatherreport");
                qConf.AddJob<LoadForecastDataJob>(jobOpt => jobOpt.WithIdentity(jobLoadForecastDataKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobLoadForecastDataKey)
                    .WithIdentity("LoadForecastDataJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(20)));

                var jobIbmWeatherReportsCalculationKey = new JobKey("IbmWeatherReportsCalculationJob", "weatherreport");
                qConf.AddJob<IbmWeatherReportsCalculationJob>(jobOpt => jobOpt.WithIdentity(jobIbmWeatherReportsCalculationKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobIbmWeatherReportsCalculationKey)
                    .WithIdentity("IbmWeatherReportsCalculationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddHours(1)));

                var jobIbmWeatherReportsTilesGenerationKey = new JobKey("IbmWeatherReportsTilesGenerationJob", "weatherreport");
                qConf.AddJob<IbmWeatherReportsTilesGenerationJob>(jobOpt => jobOpt.WithIdentity(jobIbmWeatherReportsTilesGenerationKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobIbmWeatherReportsTilesGenerationKey)
                    .WithIdentity("IbmWeatherReportsTilesGenerationJobTrigger", "weatherreport")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(24);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddHours(2)));
                #endregion

                #region Report
                var jobCollectQwenResultKey = new JobKey("collectQwenFullAnalysisToReportJob");
                qConf.AddJob<CollectQwenFullAnalysisToReportJob>(jobOpt => jobOpt.WithIdentity(jobCollectQwenResultKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobCollectQwenResultKey)
                    .WithIdentity("collectQwenFullAnalysisToReportJob")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(1);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddSeconds(25)));

                var jobGenerateAnaliticReportKey = new JobKey("generateAnaliticReportJob");
                qConf.AddJob<GenerateAnaliticReportJob>(jobOpt => jobOpt.WithIdentity(jobGenerateAnaliticReportKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobGenerateAnaliticReportKey)
                    .WithIdentity("generateAnaliticReportJob")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(1);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(10)));
                #endregion

                #region BidQwenExpressAnalysis
                var jobFetchQwenResultsKey = new JobKey("fetchQwenResultsJob");
                qConf.AddJob<FetchQwenResultsJob>(jobOpt => jobOpt.WithIdentity(jobFetchQwenResultsKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobFetchQwenResultsKey)
                    .WithIdentity("fetchQwenResultsJob")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(2);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(5)));

                var jobBidSentToQwenAnalysisKey = new JobKey("bidSentToQwenAnalyseJob");
                qConf.AddJob<GenerateAnaliticReportJob>(jobOpt => jobOpt.WithIdentity(jobBidSentToQwenAnalysisKey));
                qConf.AddTrigger(triggerOpt => triggerOpt
                    .ForJob(jobBidSentToQwenAnalysisKey)
                    .WithIdentity("bidSentToQwenAnalyseJob")
                    .WithSimpleSchedule(builder =>
                    {
                        builder.WithIntervalInHours(2);
                        builder.RepeatForever();
                    }).StartAt(DateTimeOffset.Now.AddMinutes(10)));
                #endregion
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1024 * 1024 * 1024;
                options.ValueLengthLimit = 1024 * 1024 * 1024;
                options.MemoryBufferThreshold = 1024 * 1024 * 1024;
            });

            var smtpConfig = configuration.GetSection("GeoServerOptions").Get<GeoServerOptions>();
            if (smtpConfig != null)
            {
                services.Configure<GeoServerOptions>(configuration.GetSection("GeoServerOptions"));
            }

            services.AddAllowCORS(configuration);
            services.AddMinio(cfg =>
            {
                cfg.WithEndpoint(configuration.GetValue<string>("Minio:Endpoint"), configuration.GetValue<int>("Minio:Port"));
                cfg.WithCredentials(configuration.GetValue<string>("Minio:AccessKey"), configuration.GetValue<string>("Minio:SecretKey"));
                cfg.WithSSL(false);
                cfg.WithTimeout(120000);
                cfg.Build();
            }
            );
            services.AddTransient<IFileStorageProvider, MinIOStorageProvider>();
            services.AddScoped<IBusinessLogicLogger, BusinessLogicLogger>();
            services.AddScoped<WeatherReportColors, WeatherReportColors>();
            services.AddScoped<GeoServerService, GeoServerService>();
            services.AddEmailService(configuration);
            services.AddAllDbContexts(configuration);

            services.AddBusinessLogic();
            services.AddHttpClient();
            services.AddEndpointsApiExplorer();
            services.AddConfigurableControllers(configuration);
            services.AddDocumentation(configuration);
            services.AddSingleton(_ => NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
            services.AddIdenity(configuration);
            services.AddOneId(configuration);
            services.AddQuartzJobs(configuration);
            services.AddQwen(configuration);
            services.AddMemoryCache();
        }

        public static void AddIdenity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<MasofaIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddJWTAuth(configuration);
        }

        public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpConfig = configuration.GetSection("SmtpOptions").Get<SmtpOptions>();

            if (smtpConfig == null)
            {
                throw new ArgumentNullException(nameof(smtpConfig));
            }

            services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));

            var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates", "EmailTemplates");

            services.AddTransient<IEmailSender>(sp =>
            {
                var smtpOptions = sp.GetRequiredService<IOptions<SmtpOptions>>();
                var dbContext = sp.GetRequiredService<MasofaCommonDbContext>();
                return new EmailSender(dbContext, smtpOptions, templatesPath);
            });
        }

        public static void AddSatelliteSentinel(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрируем парсеры Sentinel-2
            services.Configure<SentinelServiceOptions>(options =>
            {
                options.ApiUrl = "https://scihub.copernicus.eu/dhus";
                options.UserName = "avazbekiskandarov9812@gmail.com";
                options.Password = "Avazoxun@1998";
                options.TokenApiUrl = "https://identity.dataspace.copernicus.eu/auth/realms/CDSE/protocol/openid-connect/token";
                options.ProductSearchApiUrl = "https://catalogue.dataspace.copernicus.eu/odata/v1/Products";
                options.ProductDownloadApiUrl = "https://download.dataspace.copernicus.eu/odata/v1/Products";
            });

            services.AddTransient<CopernicusApiUnitOfWork>();
        }

        public static void AddIBMWeather(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<IBMWeatherServiceOptions>(configuration.GetSection("IBMWeather"));

            services.AddTransient<IBMWeatherApiUnitOfWork>();


        }

        public static void AddSatelliteLandsat(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LandsatServiceOptions>(configuration.GetSection("Landsat"));
            // Регистрируем парсеры Landsat
            services.AddTransient<LandsatArchiveParserHelper>();


            // Регистрируем LandsatApiUnitOfWork для команд DevOps Utils
            services.AddScoped<LandsatApiUnitOfWork>();
        }

        public static void AddDepricatedDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрируем устаревшие контексты только если включен модуль миграции
            var systemBackgroundTaskConfig = configuration.GetSection("SystemBackgroundTask");
            var migrationEnabled = systemBackgroundTaskConfig.GetValue<bool>("Enabled", true) &&
                                 systemBackgroundTaskConfig.GetSection("Modules").GetValue<bool>("Migration", true);

            if (migrationEnabled)
            {
                // Регистрируем устаревшие контексты с базовыми настройками
                // Они будут использовать свои внутренние строки подключения из OnConfiguring
                services.AddDbContext<DepricatedUmapiServerOneDbContext>();
                services.AddDbContext<DepricatedUmapiServerTwoDbContext>();
                services.AddDbContext<DepricatedUalertsServerOneDbContext>();
                services.AddDbContext<DepricatedUdictServerTwoDbContext>();
                services.AddDbContext<DepricatedUfieldsServerOneDbContext>();
                services.AddDbContext<DepricatedAuthServerOneDbContext>();
                services.AddDbContext<DepricatedAuthServerTwoDbContext>();
                services.AddDbContext<DepricatedWeatherServerOneDbContext>();
            }
        }

        public static void AddEra5(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Era5Options>(configuration.GetSection("Era5"));
            services.AddScoped<Era5Options>();
            services.AddTransient<Era5ApiUnitOfWork>();
        }

        public static void AddUgm(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UgmOptions>(configuration.GetSection("UgmOptions"));
            services.AddScoped<UgmOptions>();
            services.AddTransient<UgmApiUnitOfWork>();
        }

        public static void AddOneId(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OneIdOptions>(configuration.GetSection("OneIdOptions"));
            services.AddScoped<OneIdOptions>();
            services.AddTransient<OneIdUnitOfWork>();
        }

        public static void AddQwen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<QwenUnitOfWork>();
        }

        /// <summary>
        /// Добавляет команды DevOps Utils
        /// </summary>
        //public static void AddDevOpsUtilsCommands(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // Тестовые команды
        //    services.AddTransient<TestSimpleCommand>();

        //    // Команды генерации кода
        //    services.AddTransient<CGetDbSetAddCodeGenCommand>();
        //    services.AddTransient<CImplictCodeGenCommand>();
        //    services.AddTransient<MigrationPartitionerCommand>();

        //    // Команды экспорта
        //    services.AddTransient<BidsWithArichiveExportCommand>();

        //    // Команды миграции - регистрируем только если включены устаревшие контексты
        //    var systemBackgroundTaskConfig = configuration.GetSection("SystemBackgroundTask");
        //    var migrationEnabled = systemBackgroundTaskConfig.GetValue<bool>("Enabled", true) && 
        //                         systemBackgroundTaskConfig.GetSection("Modules").GetValue<bool>("Migration", true);

        //    if (migrationEnabled)
        //    {
        //        services.AddTransient<CompareDataCommand>();
        //        services.AddTransient<MigrateBidsCommand>();
        //        services.AddTransient<MigrateBidsFileResultCommand>();
        //        services.AddTransient<MigrateDictionariesCommand>();
        //        services.AddTransient<MigrateIdentityCommand>();
        //        services.AddTransient<MigrateSeasonCommand>();
        //        services.AddTransient<MigrateTemplatesCommand>();
        //        services.AddTransient<MigrateWeatherExportCsvCommand>();
        //        services.AddTransient<MigrateWeatherImportCsvCommand>();
        //        services.AddTransient<WeatherCreatePartitionCommand>();
        //        services.AddTransient<WeatherConverter>();
        //    }

        //    // Команды Landsat - регистрируем только если включен модуль Landsat
        //    var landsatEnabled = systemBackgroundTaskConfig.GetValue<bool>("Enabled", true) && 
        //                       systemBackgroundTaskConfig.GetSection("Modules").GetValue<bool>("Landsat", true);

        //    if (landsatEnabled)
        //    {
        //        services.AddTransient<LandsatCreatePartitionCommand>();
        //        services.AddTransient<LandsatSearchProductCommand>();
        //        services.AddTransient<LandsatSearchProductByDateCommand>();
        //        //services.AddTransient<LandsatArchiveParsingCommand>();
        //        services.AddTransient<LandsatMetadataLoaderCommand>();
        //        services.AddTransient<LandsatMediaLoaderCommand>();
        //    }

        //    // Команды Sentinel2 - регистрируем только если включен модуль Sentinel
        //    var sentinelEnabled = systemBackgroundTaskConfig.GetValue<bool>("Enabled", true) && 
        //                        systemBackgroundTaskConfig.GetSection("Modules").GetValue<bool>("Sentinel", true);

        //    if (sentinelEnabled)
        //    {
        //        services.AddTransient<Sentinel2SearchProductByDateCommand>();
        //        services.AddTransient<Sentinel2SearchProductCommand>();
        //        services.AddTransient<SentinelCreatePartitionCommand>();
        //        services.AddTransient<Sentinel2MetadataLoaderCommand>();
        //        //services.AddTransient<Sentinel2ArchiveParsingCommand>();
        //        services.AddTransient<Sentinel2MediaLoaderCommand>();
        //    }

        //    // Сервисы парсеров - регистрируем только если включены соответствующие модули
        //    if (landsatEnabled)
        //    {
        //        //services.AddTransient<LandsatArchiveParserHelper>();
        //    }

        //    if (sentinelEnabled)
        //    {
        //        services.AddTransient<Sentinel2ArchiveParserHelper>();
        //    }
        //}
    }
}
