using Masofa.BusinessLogic;
using Masofa.BusinessLogic.Cli.Migration;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NLog.Web.LayoutRenderers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Web.Monolith.Controllers.Common
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "Common")]
    public class DevelopController : BaseController
    {
        UserManager<Masofa.Common.Models.Identity.User> UserManager { get; set; }
        RoleManager<Masofa.Common.Models.Identity.Role> RoleManager { get; set; }

        public DevelopController(ILogger<DevelopController> logger, IConfiguration configuration, IMediator mediator, UserManager<Masofa.Common.Models.Identity.User> userManager, RoleManager<Masofa.Common.Models.Identity.Role> roleManager) : base(logger, configuration, mediator)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<byte[]>> GenerationUrlAccessMap()
        {
            var result = new List<UrlAccessMapItem>();
            var controllerTypes = typeof(Program).Assembly.GetTypes()
                .Where(m => ((!string.IsNullOrEmpty(m.Namespace)) && (m.Namespace.Contains("Masofa.Web.Monolith.Controllers"))))
                .Where(m => !m.Name.Contains("Base"));

            var roles = RoleManager.Roles;

            foreach (var controllerType in controllerTypes) 
            {
                var methods = controllerType.GetRuntimeMethods()
                    .Where(m => m.IsPublic)
                    .Where(m =>
                    {
                        if (m.GetCustomAttribute<HttpGetAttribute>() != null)
                        {
                            return true;
                        }

                        if (m.GetCustomAttribute<HttpPostAttribute>() != null)
                        {
                            return true;
                        }

                        if (m.GetCustomAttribute<HttpPutAttribute>() != null)
                        {
                            return true;
                        }

                        if (m.GetCustomAttribute<HttpDeleteAttribute>() != null)
                        {
                            return true;
                        }
                        return false;
                    });

                foreach (var method in methods)
                {
                    var tempItem = new UrlAccessMapItem();
                    tempItem.RoleChecks = new List<UrlAccessMapItemRoleCheck>();
                    tempItem.Url = $"/{controllerType.Namespace.Replace("Masofa.Web.Monolith.Controllers.", "")}/{controllerType.Name.Replace("Controller", "")}/{method.Name}";
                    foreach (var role in roles) 
                    {
                        tempItem.RoleChecks.Add(new UrlAccessMapItemRoleCheck()
                        {
                            IsChecked = CheckAccess(role.Name, method, controllerType),
                            RoleName = role.Name
                        });
                    }

                    result.Add(tempItem);
                }
            }

            return File(GenerateMarkdownAccessTable(result, roles.Select(m => m.Name).ToList()), "text/markdown");
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[action]")]
        public async Task<ActionResult<Guid>> CreateTask()
        {
            try
            {
                var tempCmd = new Masofa.Common.Models.SystemCrical.SystemBackgroundTask()
                {
                    ExecuteTypeName = typeof(Masofa.Web.Monolith.Jobs.Sentinel2.Sentinel2ArchiveParsingJob).FullName,
                    IsActive = true,
                    TaskType = Masofa.Common.Models.SystemCrical.SystemBackgroundTaskType.Schedule,
                    TaskOptionJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ScheduleTaskOptions()
                    {
                        StartDelaySeconds = 30,
                        GroupName = "sentinel",
                        Type = ScheduleType.Once
                    }),
                    Status = Masofa.Common.Models.StatusType.Active
                };

                var guid = await Mediator.Send(new BaseCreateCommand<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext>()
                {
                    Author = "Admin",
                    Model = tempCmd,
                });
                return Ok(guid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private bool CheckAccess(string role, MethodInfo method, Type controlerType)
        {
            try
            {
                var authAttr = controlerType.GetCustomAttribute<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
                if (authAttr == null)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(authAttr.Roles))
                {
                    return true;
                }

                if (authAttr.Roles.Contains(role))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex) 
            {
                return true;
            }

        }

        private byte[] GenerateMarkdownAccessTable(IReadOnlyList<UrlAccessMapItem> urlAccessMapItems, IReadOnlyList<string> possibleRoles)
        {
            if (urlAccessMapItems == null) throw new ArgumentNullException(nameof(urlAccessMapItems));
            if (possibleRoles == null) throw new ArgumentNullException(nameof(possibleRoles));

            var sb = new StringBuilder();

            // Заголовок таблицы
            sb.Append("| 🐉 | URL ");
            foreach (var role in possibleRoles)
            {
                sb.Append($"| {role} ");
            }
            sb.AppendLine("|");

            // Разделитель
            sb.Append("|----|-----");
            foreach (var _ in possibleRoles)
            {
                sb.Append("|:---:");
            }
            sb.AppendLine("|");

            // Строки данных
            var index = 1;
            foreach (var item in urlAccessMapItems.OrderBy(u => u.Url)) // сортируем по URL для порядка
            {
                sb.Append($"| {index} | {EscapeMarkdown(item.Url)} ");

                // Создаём словарь для быстрого поиска ролей
                var roleMap = item.RoleChecks.ToDictionary(r => r.RoleName, r => r.IsChecked, StringComparer.OrdinalIgnoreCase);

                foreach (var role in possibleRoles)
                {
                    bool isChecked = roleMap.TryGetValue(role, out bool value) && value;
                    string mark = isChecked ? "✅" : "❌";
                    sb.Append($"| {mark} ");
                }
                sb.AppendLine("|");
                index++;
            }

            string markdown = sb.ToString();
            return Encoding.UTF8.GetBytes(markdown);
        }

        private string EscapeMarkdown(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("\\", "\\\\")  // экранируем обратный слеш первым
                .Replace("|", "\\|")    // экранируем пайпы
                .Replace("\n", " ")     // убираем переносы строк
                .Replace("\r", "");
        }

        private List<UserRole> _roles = new List<UserRole>()
        {
            new UserRole("admin@uz.uz", "Admin"),
            new UserRole("user@uz.uz", "User"),
            new UserRole("user_cc@6grain.com", "User"),
            new UserRole("admin_cc@6grain.com", "Admin"),
            new UserRole("m.rusanov@6grain.com", "Admin"),
            new UserRole("user3@masofa-yer.uz", "User"),
            new UserRole("user4@masofa-yer.uz", "User"),
            new UserRole("user6@masofa-yer.uz", "User"),
            new UserRole("user5@masofa-yer.uz", "User"),
            new UserRole("user_operator@6grain.com", "Operator"),
            new UserRole("user_foreman@6grain.com", "Foreman"),
            new UserRole("test@test.oper", "Operator"),
            new UserRole("user_fw@6grain.com", "Admin"),
            new UserRole("user_fw1@6grain.com", "FieldWorker"),
            new UserRole("user_fw2@6grain.com", "FieldWorker"),
            new UserRole("user_fw3@6grain.com", "FieldWorker"),
            new UserRole("foreman_cc@uz.uz", "Foreman"),
            new UserRole("no_use@mail.ru", "Operator"),
            new UserRole("fw4_cc@uz.uz", "Admin"),
            new UserRole("fw5_cc@uz.uz", "FieldWorker"),
            new UserRole("fw6_cc@uz.uz", "FieldWorker"),
            new UserRole("OPER@uz.uz", "Operator"),
            new UserRole("user1@masofa-yer.uz", "Admin"),
            new UserRole("test4@test.tst", "FieldWorker"),
            new UserRole("test@test.asdf", "Operator"),
            new UserRole("user2@masofa-yer.uz", "FieldWorker"),
            new UserRole("operator@test.ru", "Operator"),
            new UserRole("root@uz.uz", "SuperAdministrator"),
            new UserRole("worker@uz.com", "FieldWorker"),
            new UserRole("operator@test.asdf", "Operator"),
            new UserRole("system@6grain.com", "System"),
            new UserRole("f.ortikov@agro.uz", "Operator"),
            new UserRole("nurlymuratov-na@meteo.uz", "Foreman"),
            new UserRole("dauletmuratova-ga@meteo.uz", "FieldWorker"),
            new UserRole("toreeva-zhya@meteo.uz", "FieldWorker"),
            new UserRole("kulcharov-rm@meteo.uz", "FieldWorker"),
            new UserRole("nasretdinov-ka@meteo.uz", "FieldWorker"),
            new UserRole("adilov-ab@meteo.uz", "FieldWorker"),
            new UserRole("tursinov-ab@meteo.uz", "FieldWorker"),
            new UserRole("saparov-khb@meteo.uz", "FieldWorker"),
            new UserRole("yuldasheva-gr@meteo.uz", "Foreman"),
            new UserRole("markova-tp@meteo.uz", "FieldWorker"),
            new UserRole("devonaev-ma@meteo.uz", "FieldWorker"),
            new UserRole("sodikov-im@meteo.uz", "FieldWorker"),
            new UserRole("melikuziev-shsh@meteo.uz", "FieldWorker"),
            new UserRole("mamadzhonova-mi@meteo.uz", "FieldWorker"),
            new UserRole("toshtemirov-aa@meteo.uz", "FieldWorker"),
            new UserRole("kuzieva-gn@meteo.uz", "FieldWorker"),
            new UserRole("kurbonov-ov@meteo.uz", "FieldWorker"),
            new UserRole("akhmedov-du@meteo.uz", "FieldWorker"),
            new UserRole("bakaeva-db@meteo.uz", "Foreman"),
            new UserRole("mukhiddinov-ss@meteo.uz", "FieldWorker"),
            new UserRole("farmonov-az@meteo.uz", "FieldWorker"),
            new UserRole("toshkinov-khn@meteo.uz", "FieldWorker"),
            new UserRole("zubaev-fr@meteo.uz", "Foreman"),
            new UserRole("normatov-at@meteo.uz", "FieldWorker"),
            new UserRole("kholyigitov-em@meteo.uz", "FieldWorker"),
            new UserRole("saliev-ri@meteo.uz", "FieldWorker"),
            new UserRole("korabekov-af@meteo.uz", "FieldWorker"),
            new UserRole("sidorova-kv@meteo.uz", "FieldWorker"),
            new UserRole("dzhuraeva-as@meteo.uz", "Foreman"),
            new UserRole("khasanova-rv@meteo.uz", "FieldWorker"),
            new UserRole("davronov-in@meteo.uz", "FieldWorker"),
            new UserRole("chuliev-au@meteo.uz", "FieldWorker"),
            new UserRole("zhuravleva-sa@meteo.uz", "FieldWorker"),
            new UserRole("choriev-sb@meteo.uz", "FieldWorker"),
            new UserRole("mirzaev-sf@meteo.uz", "FieldWorker"),
            new UserRole("ruzmanova-msh@meteo.uz", "FieldWorker"),
            new UserRole("abdurakhmonova-me@meteo.uz", "FieldWorker"),
            new UserRole("yadgarov-nzh@meteo.uz", "FieldWorker"),
            new UserRole("nizamova-nn@meteo.uz", "Foreman"),
            new UserRole("ashurova-mb@meteo.uz", "FieldWorker"),
            new UserRole("ergasheva-gn@meteo.uz", "FieldWorker"),
            new UserRole("turaeva-zs@meteo.uz", "FieldWorker"),
            new UserRole("abdukarimov-aa@meteo.uz", "Foreman"),
            new UserRole("turakhanov-kk@meteo.uz", "FieldWorker"),
            new UserRole("abdikodirov-a@meteo.uz", "FieldWorker"),
            new UserRole("abdullaev-fkh@meteo.uz", "FieldWorker"),
            new UserRole("ulukov-mm@meteo.uz", "FieldWorker"),
            new UserRole("abdullaeva-gf@meteo.uz", "FieldWorker"),
            new UserRole("kholova-nya@meteo.uz", "Foreman"),
            new UserRole("amirov-okh@meteo.uz", "FieldWorker"),
            new UserRole("kholmukhamedova-dn@meteo.uz", "FieldWorker"),
            new UserRole("kayumova-gkh@meteo.uz", "FieldWorker"),
            new UserRole("boybekova-nm@meteo.uz", "FieldWorker"),
            new UserRole("makhsimov-zb@meteo.uz", "FieldWorker"),
            new UserRole("shokirov-mf@meteo.uz", "FieldWorker"),
            new UserRole("zhumanov-go@meteo.uz", "FieldWorker"),
            new UserRole("kamolov-ta@meteo.uz", "FieldWorker"),
            new UserRole("kurbonsaidov-oy@meteo.uz", "Foreman"),
            new UserRole("gaipova-dkh@meteo.uz", "FieldWorker"),
            new UserRole("murotov-zht@meteo.uz", "FieldWorker"),
            new UserRole("ergashev-skh@meteo.uz", "FieldWorker"),
            new UserRole("mengnarov-sha@meteo.uz", "FieldWorker"),
            new UserRole("zulkhaydarov-im@meteo.uz", "Foreman"),
            new UserRole("sharofiddinov-aa@meteo.uz", "FieldWorker"),
            new UserRole("eshmurzaev-nd@meteo.uz", "FieldWorker"),
            new UserRole("ismailova-ee@meteo.uz", "FieldWorker"),
            new UserRole("abdaliev-ka@meteo.uz", "FieldWorker"),
            new UserRole("murotov-bb@meteo.uz", "FieldWorker"),
            new UserRole("khaydarova-sha@meteo.uz", "Foreman"),
            new UserRole("sharafutdinova-nr@meteo.uz", "FieldWorker"),
            new UserRole("khamraeva-rb@meteo.uz", "FieldWorker"),
            new UserRole("kodirova-fr@meteo.uz", "FieldWorker"),
            new UserRole("orlova-as@meteo.uz", "FieldWorker"),
            new UserRole("sazonova-li@meteo.uz", "FieldWorker"),
            new UserRole("pasynkova-na@meteo.uz", "FieldWorker"),
            new UserRole("nigay-lkh@meteo.uz", "FieldWorker"),
            new UserRole("tozhiev-akh@meteo.uz", "FieldWorker"),
            new UserRole("ismoilova-ms@meteo.uz", "Foreman"),
            new UserRole("tilyakhodzhaeva-mz@meteo.uz", "FieldWorker"),
            new UserRole("dzhumabaev-tt@meteo.uz", "FieldWorker"),
            new UserRole("bakhtierkhonov-kb@meteo.uz", "FieldWorker"),
            new UserRole("saidzhonova-lsh@meteo.uz", "Foreman"),
            new UserRole("kalandarova-fk@meteo.uz", "FieldWorker"),
            new UserRole("khamraev-sho@meteo.uz", "FieldWorker"),
            new UserRole("dosumova-nm@meteo.uz", "FieldWorker"),
            new UserRole("erkabaev-tr@meteo.uz", "FieldWorker"),
            new UserRole("rakhimbaev-zhg@meteo.uz", "FieldWorker"),
            new UserRole("orunov-ba@meteo.uz", "FieldWorker"),
            new UserRole("lev-ni@meteo.uz", "Operator"),
            new UserRole("tutieva-mk@meteo.uz", "Operator"),
            new UserRole("feldman-lv@meteo.uz", "Operator"),
            new UserRole("testpassword@test.ru", "Admin"),
            new UserRole("butaeva-gkh@meteo.uz", "FieldWorker"),
            new UserRole("askarova-em@meteo.uz", "FieldWorker"),
            new UserRole("abdullayeva-in@meteo.uz", "Foreman"),
            new UserRole("xayitov-ub@meteo.uz", "FieldWorker"),
            new UserRole("test@password.ru", "Admin"),
            new UserRole("temp@test", "User"),
            new UserRole("fields_request@6grain.com", "Operator"),
            new UserRole("rbigildin6@gmail.com", "Foreman"),
            new UserRole("boboqul77@mail.ru", "Foreman"),
            new UserRole("b.akromov@mail.ru", "Foreman"),
            new UserRole("lev-ni-b@meteo.uz", "Foreman"),
            new UserRole("lev-ni-w@meteo.uz", "FieldWorker"),
            new UserRole("tutieva-mk-w@meteo.uz", "FieldWorker"),
            new UserRole("tutieva-mk-b@meteo.uz", "Foreman"),
            new UserRole("feldman-lv-w@meteo.uz", "FieldWorker"),
            new UserRole("m@m.uz", "Foreman"),
            new UserRole("fw_cc@uz.uz", "FieldWorker"),
            new UserRole("evz37@yandex.ru", "Admin"),
            new UserRole("maks@maks.uz", "FieldWorker"),
            new UserRole("maxshnn@yandex.com", "Foreman"),
            new UserRole("lev-ni_b@meteo.uz", "Foreman"),
            new UserRole("lev-ni_w@meteo.uz", "FieldWorker"),
            new UserRole("ozodbekabdurashidov13@gmail.com", "FieldWorker"),
            new UserRole("inomjonnabiyev3477@gmail.com", "FieldWorker"),
            new UserRole("ruzieva-fa@meteo.uz", "FieldWorker"),
            new UserRole("mamaev-do@meteo.uz", "FieldWorker"),
            new UserRole("kholova-di@meteo.uz", "FieldWorker"),
            new UserRole("khokhlova-di@meteo.uz", "Operator"),
            new UserRole("mengnarova-ba@meteo.uz", "FieldWorker"),
            new UserRole("saidov-fa@meteo.uz", "Operator"),
            new UserRole("saidov-fa-b@meteo.uz", "Foreman"),
            new UserRole("azamov-khr@meteo.uz", "Operator"),
            new UserRole("feldman-lv-b@meteo.uz", "Foreman"),
            new UserRole("feldman-lv-br@meteo.uz", "Foreman"),
            new UserRole("khazratova-ze@meteo.uz", "Foreman"),
            new UserRole("numonzhanov-ru@meteo.uz", "FieldWorker"),
            new UserRole("omonullayev-af-b@meteo.uz", "Foreman"),
            new UserRole("omonullayev-af@meteo.uz", "Operator"),
            new UserRole("mavlyanov-ar@meteo.uz", "Foreman"),
            new UserRole("isamutdinova-umr@meteo.uz", "FieldWorker"),
            new UserRole("xaydarova-fo@meteo.uz", "Foreman"),
            new UserRole("mominova-di@meteo.uz", "FieldWorker"),
            new UserRole("sadriddinov-sho@meteo.uz", "FieldWorker"),
            new UserRole("mavlyanov-ar-o@meteo.uz", "Operator"),
            new UserRole("ortikov-f@agro.uz", "Operator"),
            new UserRole("mustarov-ab@meteo.uz", "FieldWorker"),
            new UserRole("shokirboyev-zi@meteo.uz", "FieldWorker"),
            new UserRole("veliulova-gy@meteo.uz", "FieldWorker"),
            new UserRole("xaydarova-fot@meteo.uz", "Operator"),
            new UserRole("xaydarova-fo-w@meteo.uz", "FieldWorker"),
            new UserRole("saidov-fa-w@meteo.uz", "FieldWorker"),
            new UserRole("mominova-di-o@meteo.uz", "Operator"),
            new UserRole("mominova-di-b@meteo.uz", "Foreman"),
            new UserRole("yunusov-sa@meteo.uz", "FieldWorker"),
            new UserRole("suyarkulov-ru@meteo.uz", "FieldWorker"),
            new UserRole("suyarkulov-ry@meteo.uz", "FieldWorker"),
            new UserRole("eshonqulov-sh@meteo.uz", "FieldWorker"),
            new UserRole("usmonov-ab@meteo.uz", "FieldWorker"),
            new UserRole("abduraupov-sh@meteo.uz", "FieldWorker"),
            new UserRole("odilov-yo@meteo.uz", "FieldWorker"),
            new UserRole("mihailova-an@meteo.uz", "FieldWorker"),
            new UserRole("mihajlova-an@meteo.uz", "FieldWorker"),
            new UserRole("sharapova-vi@meteo.uz", "FieldWorker"),
            new UserRole("mirzahanova-za@meteo.uz", "FieldWorker"),
            new UserRole("mirzayeva-um@meteo.uz", "FieldWorker"),
            new UserRole("buvrayeva-la@meteo.uz", "FieldWorker"),
            new UserRole("igamberdiyeva-na@meteo.uz", "FieldWorker"),
            new UserRole("raximjonova-sho@meteo.uz", "FieldWorker"),
            new UserRole("k-kalashnikov73@yandex.ru", "Operator"),
            new UserRole("azimova-zu@meteo.uz", "FieldWorker"),
            new UserRole("fayzullayeva-di@meteo.uz", "FieldWorker"),
            new UserRole("abduqodirov-ak@meteo.uz", "FieldWorker"),
            new UserRole("nadirkulova-da@meteo.uz", "FieldWorker"),
            new UserRole("norov-na@meteo.uz", "FieldWorker"),
            new UserRole("ibragimxodzayeva-ma@meteo.uz", "FieldWorker"),
            new UserRole("abdieva-ma@meteo.uz", "Admin"),
            new UserRole("nunayev-al@meteo.uz", "FieldWorker"),
            new UserRole("avazov-ja@meteo.uz", "FieldWorker"),
            new UserRole("abdieva-mar@meteo.uz", "FieldWorker"),
            new UserRole("normamatova-sa@meteo.uz", "FieldWorker"),
            new UserRole("baxronov-ax@meteo.uz", "FieldWorker"),
            new UserRole("majidov-ra@meteo.uz", "FieldWorker"),
            new UserRole("nematova-ni@meteo.uz", "FieldWorker"),
            new UserRole("boboev-ja@meteo.uz", "FieldWorker"),
            new UserRole("dosbekov-ul@meteo.uz", "FieldWorker"),
            new UserRole("qalandarova-mu@meteo.uz", "FieldWorker"),
            new UserRole("ergasheva-gu@meteo.uz", "FieldWorker"),
            new UserRole("abdullaeva-ma@meteo.uz", "FieldWorker"),
            new UserRole("khazratova-pa@meteo.uz", "FieldWorker"),
            new UserRole("mamadolimova-ki@meteo.uz", "FieldWorker"),
            new UserRole("mamajonova-me@meteo.uz", "FieldWorker"),
            new UserRole("muxtarov-hu@meteo.uz", "FieldWorker"),
            new UserRole("divanaev-ma@meteo.uz", "FieldWorker"),
            new UserRole("sodiqov-ik@meteo.uz", "FieldWorker"),
            new UserRole("kuzieva-gu@meteo.uz", "FieldWorker"),
            new UserRole("tashlanov-as@meteo.uz", "FieldWorker"),
            new UserRole("nomongonov-ru@meteo.uz", "FieldWorker"),
            new UserRole("gostisheva-sv@meteo.uz", "FieldWorker"),
            new UserRole("jurayev-te@meteo.uz", "FieldWorker"),
            new UserRole("maxanbetov-ku@meteo.uz", "FieldWorker"),
            new UserRole("aimbetova-ul@meteo.uz", "FieldWorker"),
            new UserRole("kabulov-ai@meteo.uz", "FieldWorker"),
            new UserRole("doshimova-ay@meteo.uz", "FieldWorker"),
            new UserRole("nasratdinov-qu@meteo.uz", "FieldWorker"),
            new UserRole("egamberdieva-ra@meteo.uz", "FieldWorker"),
            new UserRole("aliev-mu@meteo.uz", "FieldWorker"),
            new UserRole("eazhimuratova-zi@meteo.uz", "FieldWorker"),
            new UserRole("tazhimuratova-zi@meteo.uz", "FieldWorker"),
            new UserRole("begmuratova-di@meteo.uz", "FieldWorker"),
            new UserRole("sarsenbaeva-ar@meteo.uz", "FieldWorker"),
            new UserRole("nazarbaev-ad@meteo.uz", "FieldWorker"),
            new UserRole("yuldasheva-va@meteo.uz", "FieldWorker"),
            new UserRole("imomova-zi@meteo.uz", "FieldWorker"),
            new UserRole("abduvahobov-sh@meteo.uz", "FieldWorker"),
            new UserRole("uldoshova-gu@meteo.uz", "FieldWorker"),
            new UserRole("eshmurzaeva-li@meteo.uz", "FieldWorker"),
            new UserRole("yuldasheva-mo@meteo.uz", "FieldWorker"),
            new UserRole("yuldasheva-mox@meteo.uz", "FieldWorker"),
            new UserRole("Makhsudov-Bu@argo.uz", "Foreman"),
            new UserRole("Makhsudov-Bu@agro.uz", "Foreman"),
            new UserRole("Zhuraev-Zh@agro.uz", "Operator"),
            new UserRole("Zhuraev-Zh-b@agro.uz", "Foreman"),
            new UserRole("Niyozov-Fa@agro.uz", "Foreman"),
            new UserRole("Mamajonov-Ab@agro.uz", "Foreman"),
            new UserRole("Tursunov-Tu@agro.uz", "Foreman"),
            new UserRole("Rezhavaliev-Ab@agro.uz", "FieldWorker"),
            new UserRole("Karaboev-Ah@agro.uz", "FieldWorker"),
            new UserRole("Nasirdinov-Yu@agro.uz", "Foreman"),
            new UserRole("Osarov-Al@agro.uz", "FieldWorker"),
            new UserRole("Alikulov-Ga@agro.uz", "FieldWorker"),
            new UserRole("Alikulov-Kh@agro.uz", "Admin"),
            new UserRole("Dekhkonov-Av@agro.uz", "Foreman"),
            new UserRole("Abukhamidov-Ab@agro.uz", "FieldWorker"),
            new UserRole("Kamolova-Ta@agro.uz", "FieldWorker"),
            new UserRole("Shokirov-La@agro.uz", "Foreman"),
            new UserRole("Mamajonov-So@agro.uz", "FieldWorker"),
            new UserRole("Kurbonov-Ra@agro.uz", "FieldWorker"),
            new UserRole("Bozorov-Nu@agro.uz", "Foreman"),
            new UserRole("Toshkuziev-Zh@agro.uz", "FieldWorker"),
            new UserRole("Dosmatov-Do@agro.uz", "Foreman"),
            new UserRole("Shomakhmudov-Zh@agro.uz", "FieldWorker"),
            new UserRole("Abdurakhmonov-Ni@agro.uz", "Foreman"),
            new UserRole("Madaminov-Sh@agro.uz", "FieldWorker"),
            new UserRole("Abdujalilov-Mu@agro.uz", "FieldWorker"),
            new UserRole("Bokiev-Hi@agro.uz", "Foreman"),
            new UserRole("Ergasheva-Ka@agro.uz", "FieldWorker"),
            new UserRole("Kuziboev-Sh@agro.uz", "FieldWorker"),
            new UserRole("Abdullaev-Ot@agro.uz", "Foreman"),
            new UserRole("Tuychiev-Mu@agro.uz", "FieldWorker"),
            new UserRole("Tursunov-Ma@agro.uz", "FieldWorker"),
            new UserRole("Orinboyev-So@agro.uz", "Foreman"),
            new UserRole("Vakhobov-Mu@agro.uz", "FieldWorker"),
            new UserRole("Muhammadaliyev-Ab@agro.uz", "FieldWorker"),
            new UserRole("Abdurakhimov-Er@agro.uz", "FieldWorker"),
            new UserRole("Ismailov-Ad@agro.uz", "Foreman"),
            new UserRole("Khaliljonov-Ad@agro.uz", "FieldWorker"),
            new UserRole("Kholmurojonov-Ja@agro.uz", "Foreman"),
            new UserRole("Abdulkhamidov-Ka@agro.uz", "FieldWorker"),
            new UserRole("Khaidarov-Er@agro.uz", "FieldWorker"),
            new UserRole("Sobirov-Mu@agro.uz", "Foreman"),
            new UserRole("Yakubov-Is@agro.uz", "FieldWorker"),
            new UserRole("Rakhmonlaliev-Ib@agro.uz", "FieldWorker"),
            new UserRole("Salaev-Or@agro.uz", "Foreman"),
            new UserRole("Isaboev-Bu@agro.uz", "Foreman"),
            new UserRole("Muminov-Is@agro.uz", "Foreman"),
            new UserRole("Burkhonov-Ra@agro.uz", "FieldWorker"),
            new UserRole("Ermatov-An@agro.uz", "FieldWorker"),
            new UserRole("Alikulov-Xu@agro.uz", "FieldWorker"),
            new UserRole("Khoshimov-Ab@agro.uz", "FieldWorker"),
            new UserRole("myminov-is@agro.uz", "Foreman"),
            new UserRole("qutbiddinov-sh@meteo.uz", "FieldWorker")
        };
    }

    public class UserRole
    {
        public UserRole(string email, string name)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UrlAccessMapItem
    {
        public string Url { get; set; }
        public List<UrlAccessMapItemRoleCheck> RoleChecks { get; set; }
    }
    public class UrlAccessMapItemRoleCheck
    {
        public string RoleName { get; set; }
        public bool IsChecked { get; set; }
    }
}
