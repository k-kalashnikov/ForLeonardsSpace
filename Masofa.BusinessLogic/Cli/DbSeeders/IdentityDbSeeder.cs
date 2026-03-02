using CsvHelper;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Identity;
using Quartz.Util;
using System.Globalization;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class IdentityDbSeeder
    {
        private RoleManager<Role> RoleManager { get; set; }
        private UserManager<User> UserManager { get; set; }

        private readonly List<string> _roleNames =
        [
            "Admin", "SystemAdmin", "ModuleAdmin", "Manager", "Operator", "Internal", "Application", "Subscriber", "User", "Foreman", "FieldWorker"
        ];

        public IdentityDbSeeder(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminAsync();
            await SeedUsersAsync();
            await SeedFromCsv();
        }

        private async Task SeedRolesAsync()
        {
            var roleNamesRu = new Dictionary<string, string>
            {
                ["Admin"] = "Администратор",
                ["SystemAdmin"] = "Системный администратор",
                ["ModuleAdmin"] = "Администратор подсистемы",
                ["Manager"] = "Служебный пользователь",
                ["Operator"] = "Оператор подсистемы",
                ["Internal"] = "Программный модуль",
                ["Application"] = "Внешний пользователь",
                ["Subscriber"] = "Подписчик",
                ["User"] = "Пользователь личного кабинета",
                ["Foreman"] = "Бригадир",
                ["FieldWorker"] = "Полевой работник"
            };
            var roleNamesUz = new Dictionary<string, string>
            {
                ["Admin"] = "Aministrator",
                ["SystemAdmin"] = "Tizim aministratori",
                ["ModuleAdmin"] = "Past tizim aministratori",
                ["Manager"] = "Xizmatchi foydalanuvchi",
                ["Operator"] = "Past tizim operatori",
                ["Internal"] = "Dasturiy modul",
                ["Application"] = "Tashqi foydalanuvchi",
                ["Subscriber"] = "Obunachi",
                ["User"] = "Shaxsiy kabinet foydalanuvchisi",
                ["Foreman"] = "Brigadir",
                ["FieldWorker"] = "Maydon xodimi"
            };
            var roleDescRu = new Dictionary<string, string>
            {
                ["Admin"] = "Супер администратор",
                ["SystemAdmin"] = "Эта роль принадлежит пользователям, из числа сотрудников Центра Цифровизации и включает: все полномочия для всех информационных объектов Системы; управление пользователями Системы; просмотр логов (журналов операций); операции экспорта данных; просмотр статусов HM на уровне Системы",
                ["ModuleAdmin"] = "Все полномочия для всех информационных объектов определенной подсистемы (модуля); просмотр логов (журналов операций); операции экспорта данных подсистемы; просмотр статусов HM на уровне подсистемы; блокировка доступа для пользователей подсистемы; блокировка публикации данных подсистемой; отправка запросов в службу технической поддержки Системы",
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
                ["SystemAdmin"] = "This role belongs to users who are employees of the Digitalization Center and includes: full permissions for all system information objects; system user management; access to logs (operation journals); data export operations; viewing HM statuses at the system level",
                ["ModuleAdmin"] = "Full permissions for all information objects of a specific subsystem (module); access to logs (operation journals); subsystem data export operations; viewing HM statuses at the subsystem level; blocking access for subsystem users; blocking data publication by the subsystem; sending requests to the system technical support service",
                ["Manager"] = "A role for users who are government agency employees, with varying access rights for requesting and submitting information, as well as across different categories",
                ["Operator"] = "Permissions for information objects of a specific subsystem or module within the user's assigned rights; sending requests to the system technical support service",
                ["Internal"] = "Permissions for software modules (subsystems, services)",
                ["Application"] = "A role for representatives of entities in the agro-industrial and food sectors, as well as other industries",
                ["Subscriber"] = "Access to information from a subsystem (module) marked as permitted for publication",
                ["User"] = "Access to data published by the system via the personal account, limited to information concerning the specific user; sending requests to the system technical support service; submitting corrective information to the system support service within established business processes for updating user information or user object data in the personal account",
                ["Foreman"] = "Description of the foreman role",
                ["FieldWorker"] = "Description of the field worker role"
            };
            var roleDescUz = new Dictionary<string, string>
            {
                ["Admin"] = "Super aministrator",
                ["SystemAdmin"] = "Markazning raqamlashtirish markazi xodimlari uchun mo'ljallangan ro'ldir va tizimdagi barcha axborot ob'ektlari bo'yicha to'liq huquqlarni o'z ichiga oladi; tizim foydalanuvchilarini boshqarish; loglar (amallar jurnali) ni ko'rish; ma'lumotlarni eksport qilish amallari; HM holatlarini tizim darajasida ko'rish",
                ["ModuleAdmin"] = "Ma'lum bir past tizim (modul) doirasidagi barcha axborot ob'ektlari bo'yicha to'liq huquqlar; loglarni (amallar jurnalini) ko'rish; past tizim ma'lumotlarini eksport qilish amallari; past tizim darajasida HM holatlarini ko'rish; past tizim foydalanuvchilari uchun kirishni blokirovka qilish; past tizim tomonidan ma'lumotlarni e'lon qilishni blokirovka qilish; tizim texnik yordam xizmatiga so'rov yuborish",
                ["Manager"] = "Davlat organlari xodimlari uchun mo'ljallangan rol bo'lib, turli toifalarga qarab axborot so'rash va yuborish bo'yicha har xil huquqlarga ega",
                ["Operator"] = "Foydalanuvchiga belgilangan huquqlar chegarasida ma'lum bir past tizim yoki modulning axborot ob'ektlari bo'yicha huquqlar; tizimning texnik yordam xizmatiga so'rov yuborish",
                ["Internal"] = "Dasturiy modullar (past tizimlar, xizmatlar) uchun huquqlar",
                ["Application"] = "Agro-sanoat va ovqatlanish sohasi, shuningdek, boshqa sohalardagi sub'ektlar vakillari uchun mo'ljallangan rol",
                ["Subscriber"] = "E'lon qilish uchun ruxsat berilgan sifatida belgilangan ma'lumotlarni past tizimdan (moduldan) olish",
                ["User"] = "Foydalanuvchiga tegishli qismda tizim tomonidan e'lon qilingan ma'lumotlarga shaxsiy kabinet orqali kirish; tizim texnik yordam xizmatiga so'rov yuborish; foydalanuvchi yoki foydalanuvchi LC ob'ektlari haqida ma'lumotlarni yangilash bo'yicha belgilangan biznes jarayonlari doirasida tizim yordam xizmatiga tuzatuvchi ma'lumotlarni yuborish",
                ["Foreman"] = "Brigadir rolining tavsifi",
                ["FieldWorker"] = "Maydon xodimi rolining tavsifi"
            };

            foreach (var item in _roleNames)
            {
                var names = new LocalizationString();
                names["en-US"] = item;
                names["ru-RU"] = roleNamesRu[item];
                names["uz-Latn-UZ"] = roleNamesUz[item];

                var descrs = new LocalizationString();
                descrs["en-US"] = roleDescEn[item];
                descrs["ru-RU"] = roleDescRu[item];
                descrs["uz-Latn-UZ"] = roleDescUz[item];

                var role = await RoleManager.FindByNameAsync(item);
                if (role != null)
                {
                    role.Names = names;
                    role.Descriptions = descrs;
                    await RoleManager.UpdateAsync(role);
                    continue;
                }

                var result = await RoleManager.CreateAsync(new Masofa.Common.Models.Identity.Role()
                {
                    Name = item,
                    NormalizedName = item.ToUpper(),
                    Names = names,
                    Descriptions = descrs
                });

                if (result.Succeeded)
                {
                    continue;
                }

                Console.WriteLine($"Failed to create role: {item}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        private async Task SeedAdminAsync()
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
                Approved = true
            }, "hIjKlMnOpQrS");

            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create user: Admin. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            adminUser = await UserManager.FindByNameAsync("Admin");

            await UserManager.AddToRoleAsync(adminUser, "Admin");
        }

        private async Task SeedUsersAsync()
        {
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

            Random random = new();

            string outputFilePath = $"GeneratedAccounts_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}__{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.md";

            using (StreamWriter writer = new StreamWriter(File.Open(outputFilePath, FileMode.OpenOrCreate)))
            {
                await writer.WriteLineAsync($"| Login | Email | Password | Role |");
                await writer.WriteLineAsync($"| - | - | - |");

                foreach (var roleName in _roleNames)
                {
                    if (roleName == "Admin") continue;

                    foreach (var name in femaleNames)
                    {
                        string email = $"{name.ToLower()}@sfck.ru";
                        string password = RandomGenerator.GenerateStrongPassword(12);
                        var user = new User
                        {
                            UserName = name,
                            FirstName = name,
                            Email = email,
                            EmailConfirmed = true,
                            Approved = true
                        };
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
                            await UserManager.AddToRoleAsync(user, roleName);

                            Console.WriteLine($"Created user: {name} with email: {email} with role: {roleName}");
                            await writer.WriteLineAsync($"| {name} | {email} | {password} | {roleName} |");
                            continue;
                        }

                        Console.WriteLine($"Failed to create user: {name}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private async Task SeedFromCsv()
        {
            var csvDirectory = Path.Combine(AppContext.BaseDirectory, "Cli", "DbSeeders", "Csv", "Identity");

            var csvFiles = Directory.GetFiles(csvDirectory, "*.csv");
            foreach (var csvFile in csvFiles)
            {
                string outputFilePath = $"GeneratedAccounts_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}__{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.md";
                var fileName = Path.GetFileNameWithoutExtension(csvFile);

                using var reader = new StreamReader(csvFile);
                using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    TrimOptions = CsvHelper.Configuration.TrimOptions.Trim
                });

                csv.Read();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;

                var records = new List<object>();
                using (StreamWriter writer = new StreamWriter(File.Open(outputFilePath, FileMode.OpenOrCreate)))
                {
                    await writer.WriteLineAsync($"| Login | FirstName | LastName | Email | Password |");
                    await writer.WriteLineAsync($"| - | - | - | - | - |");

                    while (csv.Read())
                    {
                        string email = csv.GetField<string>("Email") ?? string.Empty;
                        string userName = csv.GetField<string>("UserName") ?? string.Empty;
                        string firstName = csv.GetField<string>("FirstName") ?? string.Empty;
                        string lastName = csv.GetField<string>("LastName") ?? string.Empty;

                        if (userName.IsNullOrWhiteSpace()) continue;

                        string password = PasswordGeneratorHelper.GeneratePassword();
                        var user = new User
                        {
                            UserName = userName,
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            EmailConfirmed = true,
                            Approved = true
                        };

                        var existingUser = await UserManager.FindByNameAsync(userName);
                        if (existingUser != null)
                        {
                            Console.WriteLine($"User {userName} already exists.");
                            continue;
                        }

                        var result = await UserManager.CreateAsync(user, password);
                        if (result.Succeeded)
                        {
                            user = await UserManager.FindByNameAsync(userName);
                            await UserManager.AddToRoleAsync(user, "User");

                            Console.WriteLine($"Created user: {userName} with email: {email} with role: User");
                            await writer.WriteLineAsync($"| {userName} | {firstName} | {lastName} | {email} | {password} |");
                            continue;
                        }

                        Console.WriteLine($"Failed to create user: {userName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
