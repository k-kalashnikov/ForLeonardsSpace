using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.DataAccess
{
    public class DbSeeder
    {
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private UserManager<User> UserManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }
        private IConfiguration Configuration { get; set; }

        public DbSeeder(IConfiguration configuration, 
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            MasofaIdentityDbContext dbIdentityContext,
            MasofaCommonDbContext masofaCommonDbContext)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            MasofaIdentityDbContext = dbIdentityContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            Configuration = configuration;
        }

        public async Task SeedAsync()
        {
            if (Configuration.GetValue<bool>("MonolithConfiguration:StartIdentity"))
            {
                await SeedRoles();
                await SeedAdmin();
                await SeedApplications();
            }
            if (Configuration.GetValue<bool>("MonolithConfiguration:StartJobs"))
            {
                //await SeedSystemBackgroundTasksAsync();
            }

        }
        private async Task SeedUsers()
        {
            // Список женских имен
            var femaleNames = new List<string>
            {
                "Alice", "Emily", "Sophia", "Olivia", "Ava", "Mia", "Isabella", "Charlotte", "Amelia", "Harper",
                "Evelyn", "Abigail", "Elizabeth", "Chloe", "Victoria", "Madison", "Luna", "Grace", "Avery", "Zoey",
                "Nora", "Layla", "Camila", "Penelope", "Riley", "Eleanor", "Hannah", "Lily", "Ellie", "Sofia",
                "Aria", "Violet", "Nova", "Isla", "Leah", "Aurora", "Savannah", "Audrey", "Brooklyn", "Bella",
                "Claire", "Skylar", "Lucy", "Paisley", "Maggie", "Stella", "Everly", "Anna", "Caroline", "Genesis",
                "Maya", "Kinsley", "Naomi", "Cora", "Gianna", "Kennedy", "Allison", "Serenity", "Autumn", "Quinn",
                "Natalie", "Elena", "Willow", "Gabriella", "Sadie", "Delilah", "Josephine", "Ruby", "Ivy", "Madelyn"
            };

            Random random = new Random();

            string outputFilePath = $"GeneratedAccounts_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}__{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.md";

            using (StreamWriter writer = new StreamWriter(File.Open(outputFilePath, FileMode.OpenOrCreate)))
            {
                await writer.WriteLineAsync($"| Login | Email | Password |");
                await writer.WriteLineAsync($"| - | - | - |");

                foreach (var name in femaleNames)
                {
                    // Генерация уникального email
                    string email = $"{name.ToLower()}@sfck.ru";

                    // Генерация сильного пароля
                    string password = RandomGenerator.GenerateStrongPassword(12);

                    // Создание пользователя
                    var user = new User
                    {
                        UserName = name,
                        Email = email
                    };

                    // Проверка, существует ли пользователь
                    var existingUser = await UserManager.FindByNameAsync(name);
                    if (existingUser != null)
                    {
                        Console.WriteLine($"User {name} already exists.");
                        continue;
                    }

                    var result = await UserManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        user = await UserManager.FindByNameAsync(name);
                        await UserManager.AddToRoleAsync(user, "Employee");
                        Console.WriteLine($"Created user: {name} with email: {email}");
                        await writer.WriteLineAsync($"| {name} | {email} | {password} |");
                        continue;
                    }


                    Console.WriteLine($"Failed to create user: {name}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                }
            }
        }
        private async Task SeedRoles()
        {
            //var roleNames = new List<string>()
            //{
            //    "Admin", "Manager", "Operator", "Foreman", "FieldWorker", "Employee", "Application"
            //};
            var roleNames = new List<string>()
            {
                "Admin", "System Admin", "Module Admin", "Manager", "Operator", "Internal", "Application", "Subscriber", "User", "Foreman", "FieldWorker"
            };
            var roleNamesRu = new Dictionary<string, string>
            {
                ["Admin"] = "Администратор",
                ["System Admin"] = "Системный администратор",
                ["Module Admin"] = "Администратор подсистемы",
                ["Manager"] = "Служебный пользователь",
                ["Operator"] = "Оператор подсистемы",
                ["Internal"] = "Программный модуль",
                ["Application"] = "Внешний пользователь",
                ["Subscriber"] = "Подписчик",
                ["User"] = "Пользователь личного кабинета",
                ["Foreman"] = "Бригадир",
                ["FieldWorker"] = "Полевой работник"
            };
            var roleDescRu = new Dictionary<string, string>
            {
                ["Admin"] = "Супер администратор",
                ["System Admin"] = "Эта роль принадлежит пользователям, из числа сотрудников Центра Цифровизации и включает: все полномочия для всех информационных объектов Системы; управление пользователями Системы; просмотр логов (журналов операций); операции экспорта данных; просмотр статусов HM на уровне Системы",
                ["Module Admin"] = "Все полномочия для всех информационных объектов определенной подсистемы (модуля); просмотр логов (журналов операций); операции экспорта данных подсистемы; просмотр статусов HM на уровне подсистемы; блокировка доступа для пользователей подсистемы; блокировка публикации данных подсистемой; отправка запросов в службу технической поддержки Системы",
                ["Manager"] = "Роль пользователя из числа сотрудников государственных органов имеет различный доступ по правам запроса и отправки информации, а также по различным категориям",
                ["Operator"] = "Полномочия для информационных объектов определенной подсистемы или модуля в пределах установленных для пользователя прав; отправка запросов в службу технической поддержки Системы",
                ["Internal"] = "Полномочия для программных модулей (подсистем, сервисов)",
                ["Application"] = "Роль представителей субъектов агропромышленного и продовольственного сектора, а также других отраслей",
                ["Subscriber"] = "Получение информации из подсистемы (модуля), помеченной как разрешенная для публикации",
                ["User"] = "Доступ к опубликованным Системой данным через личный кабинет в части, касающейся данного пользователя; отправка запросов в службу технической поддержки Системы; отправка корректирующих сведений в службу поддержки Системы в рамках установленных бизнес-процессов обновления информации о пользователе или объектах пользователя ЛК",
                ["Foreman"] = "Описание роли бригадира",
                ["FieldWorker"] = "Описание роли полевого работника"
            };
            var roleDescEn = new Dictionary<string, string>
            {
                ["Admin"] = "Super Administrator",
                ["System Admin"] = "This role belongs to users who are employees of the Digitalization Center and includes: full permissions for all system information objects; system user management; access to logs (operation journals); data export operations; viewing HM statuses at the system level",
                ["Module Admin"] = "Full permissions for all information objects of a specific subsystem (module); access to logs (operation journals); subsystem data export operations; viewing HM statuses at the subsystem level; blocking access for subsystem users; blocking data publication by the subsystem; sending requests to the system technical support service",
                ["Manager"] = "A role for users who are government agency employees, with varying access rights for requesting and submitting information, as well as across different categories",
                ["Operator"] = "Permissions for information objects of a specific subsystem or module within the user's assigned rights; sending requests to the system technical support service",
                ["Internal"] = "Permissions for software modules (subsystems, services)",
                ["Application"] = "A role for representatives of entities in the agro-industrial and food sectors, as well as other industries",
                ["Subscriber"] = "Access to information from a subsystem (module) marked as permitted for publication",
                ["User"] = "Access to data published by the system via the personal account, limited to information concerning the specific user; sending requests to the system technical support service; submitting corrective information to the system support service within established business processes for updating user information or user object data in the personal account",
                ["Foreman"] = "Description of the foreman role",
                ["FieldWorker"] = "Description of the field worker role"
            };

            foreach (var item in roleNames)
            {
                var role = await RoleManager.FindByNameAsync(item);
                if (role != null)
                {
                    continue;
                }

                var result = await RoleManager.CreateAsync(new Role()
                {
                    Name = item,
                    NormalizedName = item.ToUpper()
                });

                if (result.Succeeded)
                {
                    continue;
                }

                Console.WriteLine($"Failed to create role: {item}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        private async Task SeedAdmin()
        {
            var adminUser = await UserManager.FindByNameAsync("Admin");

            if (adminUser != null)
            {
                return;
            }

            var result = await UserManager.CreateAsync(new User()
            {
                UserName = "Admin",
                Email = "admin@admin.com",
                EmailConfirmed = true,
            }, "hIjKlMnOpQrS");

            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create user: Admin. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            adminUser = await UserManager.FindByNameAsync("Admin");

            await UserManager.AddToRoleAsync(adminUser, "Admin");
        }
        private async Task SeedApplications()
        {
            var scpUser = await UserManager.FindByNameAsync("SCP");

            if (scpUser != null)
            {
                return;
            }

            var result = await UserManager.CreateAsync(new User()
            {
                UserName = "SCP",
                Email = "scp@sfck.ru",
                EmailConfirmed = true,
            }, "MegT#f$V%sax!JDL7t5wqz");

            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create user: SCP. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            scpUser = await UserManager.FindByNameAsync("SCP");

            await UserManager.AddToRoleAsync(scpUser, "Application");
        }

        /// <summary>
        /// Создает базовые фоновые задачи системы
        /// </summary>
        //private async Task SeedSystemBackgroundTasksAsync()
        //{
        //    try
        //    {
        //        Console.WriteLine("Starting to seed system background tasks...");

        //        #region jobs_definition

        //        var tasks = new List<SystemBackgroundTask>
        //        {
        //            // ===== NOTIFICATIONS JOBS =====
        //            new SystemBackgroundTask
        //            {
        //                Names = new Common.Models.LocalizationString()
        //                {
        //                    f
        //                },
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Notifications.NotificationsCreatePartitionJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 1 * * ?\"}" // Каждый день в 1:00
        //            },

        //            // ===== SENTINEL JOBS =====
        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel2 Search Product",
        //                NameRu = "Поиск продуктов Sentinel2",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Sentinel2.Sentinel2SearchProductJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 */6 * * ?\"}" // Каждые 6 часов
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel2 Metadata Loader",
        //                NameRu = "Загрузка метаданных Sentinel2",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Sentinel2.Sentinel2MetadataLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 30 */6 * * ?\"}" // Каждые 6 часов + 30 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel2 Media Loader",
        //                NameRu = "Загрузка медиа Sentinel2",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Sentinel2.Sentinel2MediaLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 */12 * * ?\"}" // Каждые 12 часов
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel2 Archive Parsing",
        //                NameRu = "Парсинг архива Sentinel2",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Sentinel2.Sentinel2ArchiveParsingJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 30 */12 * * ?\"}" // Каждые 12 часов + 30 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel Create Partition",
        //                NameRu = "Создание партиций Sentinel",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Sentinel2.SentinelCreatePartitionJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 2 * * ?\"}" // Каждый день в 2:00
        //            },

        //            // ===== LANDSAT JOBS =====
        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Search Product",
        //                NameRu = "Поиск продуктов Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Landsat.LandsatSearchProductJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 15 */6 * * ?\"}" // Каждые 6 часов + 15 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Metadata Loader",
        //                NameRu = "Загрузка метаданных Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Landsat.LandsatMetadataLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 45 */6 * * ?\"}" // Каждые 6 часов + 45 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Media Loader",
        //                NameRu = "Загрузка медиа Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Landsat.LandsatMediaLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 15 */12 * * ?\"}" // Каждые 12 часов + 15 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Archive Parsing",
        //                NameRu = "Парсинг архива Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Landsat.LandsatArchiveParsingJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 45 */12 * * ?\"}" // Каждые 12 часов + 45 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Create Partition",
        //                NameRu = "Создание партиций Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Landsat.LandsatCreatePartitionJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 30 2 * * ?\"}" // Каждый день в 2:30
        //            },

        //            // ===== FIELD JOBS =====
        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel Field Search",
        //                NameRu = "Поиск полей Sentinel",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.SentinelFieldSearchJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 */4 * * ?\"}" // Каждые 4 часа
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel Field Metadata Loader",
        //                NameRu = "Загрузка метаданных полей Sentinel",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.SentinelFieldMetadataLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 30 */4 * * ?\"}" // Каждые 4 часа + 30 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel Field Media Loader",
        //                NameRu = "Загрузка медиа полей Sentinel",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.SentinelFieldMediaLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 15 */4 * * ?\"}" // Каждые 4 часа + 15 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Sentinel Field Archive Parsing",
        //                NameRu = "Парсинг архива полей Sentinel",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.SentinelFieldArchiveParsingJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 45 */4 * * ?\"}" // Каждые 4 часа + 45 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Field Search",
        //                NameRu = "Поиск полей Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.LandsatFieldSearchJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 0 */6 * * ?\"}" // Каждые 6 часов
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Field Metadata Loader",
        //                NameRu = "Загрузка метаданных полей Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.LandsatFieldMetadataLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 30 */6 * * ?\"}" // Каждые 6 часов + 30 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Field Media Loader",
        //                NameRu = "Загрузка медиа полей Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.LandsatFieldMediaLoaderJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 15 */6 * * ?\"}" // Каждые 6 часов + 15 минут
        //            },

        //            new SystemBackgroundTask
        //            {
        //                NameEn = "Default Landsat Field Archive Parsing",
        //                NameRu = "Парсинг архива полей Landsat",
        //                TaskType = SystemBackgroundTaskType.Schedule,
        //                ExecuteTypeName = "Masofa.Web.Monolith.Jobs.Field.LandsatFieldArchiveParsingJob",
        //                IsActive = true,
        //                MaxExecutions = -1,
        //                IsRetryable = true,
        //                MaxRetryCount = 3,
        //                TaskOptionJson = "{\"CronExpression\": \"0 45 */6 * * ?\"}" // Каждые 6 часов + 45 минут
        //            }
        //        };

        //        #endregion

        //        // Создаем задачи
        //        foreach (var task in tasks)
        //        {
        //            var tempTask = MasofaCommonDbContext.SystemBackgroundTasks.FirstOrDefault(m => m.NameEn == task.NameEn);
        //            if (tempTask != null)
        //            {
        //                continue;
        //            }
        //            MasofaCommonDbContext.SystemBackgroundTasks.Add(task);
        //        }

        //        MasofaCommonDbContext.SaveChanges();

        //        Console.WriteLine("System background tasks seeding completed. Created {TaskCount} tasks.", tasks.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error during system background tasks seeding\n{ex.Message}");
        //    }
        //}

    }

    public static class RandomGenerator
    {
        public static string GenerateStrongPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:,.<>?";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
