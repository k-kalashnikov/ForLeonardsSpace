using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Logging.Queries
{
    public class GetCallStacksByQueryQuery : IRequest<List<CallStackDto>>
    {
        public int Take { get; set; }
        public int Offset { get; set; }
        public List<FilterItem>? Filters { get; set; }
    }

    public class FilterItem
    {
        public string Name { get; set; }
        public object? Value { get; set; }
    }

    public class CallStackDto
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid? CreateUserId { get; set; }
        public string? CreateUserName { get; set; }
        public string? CreateUserFullName { get; set; }
        public string? UserName { get; set; }
        public int ActionCount { get; set; }
        public string EventClass { get; set; }
        public string Subsystem { get; set; }
        public string InfoObject { get; set; }
    }

    public sealed class GetCallStacksByQueryHandler : IRequestHandler<GetCallStacksByQueryQuery, List<CallStackDto>>
    {
        private readonly MasofaCommonDbContext _db;
        private readonly MasofaIdentityDbContext _identityDb;

        public GetCallStacksByQueryHandler(MasofaCommonDbContext db, MasofaIdentityDbContext identityDb)
        {
            _db = db;
            _identityDb = identityDb;
        }

        public async Task<List<CallStackDto>> Handle(GetCallStacksByQueryQuery request, CancellationToken cancellationToken)
        {
            var q = _db.CallStacks.AsQueryable();

            // Логируем входящие фильтры для отладки
            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    Console.WriteLine($"Filter: {filter.Name} = {filter.Value} (Type: {filter.Value?.GetType()})");
                }
            }

            // Placeholder for filters: timestamp, subsystem, createUserId
            if (request.Filters != null)
            {
                foreach (var f in request.Filters)
                {
                    if (string.Equals(f.Name, "timestamp", StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle JArray from Newtonsoft.Json: ["2025-10-07T10:08:59Z", "2025-10-08T10:08:57Z"]
                        if (f.Value is Newtonsoft.Json.Linq.JArray jArray && jArray.Count == 2)
                        {
                            if (DateTime.TryParse(jArray[0]?.ToString(), out var start) && 
                                DateTime.TryParse(jArray[1]?.ToString(), out var end))
                            {
                                // Ensure dates are in UTC for PostgreSQL
                                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                                end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
                                
                                q = q.Where(x => x.CreateAt >= start && x.CreateAt <= end);
                            }
                        }
                    }
                    else if (string.Equals(f.Name, "createUserId", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Guid.TryParse(Convert.ToString(f.Value), out var uid))
                        {
                            q = q.Where(x => x.CreateUserId == uid);
                        }
                    }
                    else if (string.Equals(f.Name, "userName", StringComparison.OrdinalIgnoreCase))
                    {
                        var userName = Convert.ToString(f.Value);
                        if (!string.IsNullOrWhiteSpace(userName))
                        {
                            // Фильтруем по логину (UserName) из таблицы Users через CreateUserId
                            var matchingUserIds = _identityDb.Users
                                .Where(u => u.UserName != null && u.UserName.Contains(userName))
                                .Select(u => u.Id)
                                .ToList();

                            if (matchingUserIds.Any())
                            {
                                q = q.Where(x => x.CreateUserId.HasValue && matchingUserIds.Contains(x.CreateUserId.Value));
                            }
                            else
                            {
                                // Если не нашли пользователей, возвращаем пустой результат
                                q = q.Where(x => false);
                            }
                        }
                    }
                    else if (string.Equals(f.Name, "subsystem", StringComparison.OrdinalIgnoreCase))
                    {
                        var ss = Convert.ToString(f.Value);
                        if (!string.IsNullOrWhiteSpace(ss))
                        {
                            // Улучшенная фильтрация по подсистеме
                            // Ищем в Path после ".Controllers." или ".BusinessLogic."
                            q = q.Where(x => _db.LogMessages.Any(m => m.CallStackId == x.Id && 
                                (m.Path.Contains($".Controllers.{ss}") || 
                                 m.Path.Contains($".BusinessLogic.{ss}") ||
                                 m.Path.Contains(ss))));
                        }
                    }
                }
            }

            q = q.OrderByDescending(x => x.CreateAt)
                 .Skip(request.Offset)
                 .Take(request.Take);

            var items = await q.ToListAsync(cancellationToken);

            // Aggregate per stack
            var ids = items.Select(i => i.Id).ToArray();
            var messages = await _db.LogMessages
                .Where(m => ids.Contains(m.CallStackId))
                .ToListAsync(cancellationToken);

            var userIds = items
                .Where(cs => cs.CreateUserId.HasValue)
                .Select(cs => cs.CreateUserId.Value)
                .Distinct()
                .ToArray();

            var usersDict = new Dictionary<Guid, string>();
            if (userIds.Length > 0)
            {
                var users = await _identityDb.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.UserName })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.UserName))
                    {
                        usersDict[user.Id] = user.UserName;
                    }
                }
            }

            var result = new List<CallStackDto>(items.Count);
            foreach (var cs in items)
            {
                var group = messages.Where(m => m.CallStackId == cs.Id).ToList();
                var actionCount = group.Count;
                var eventClass = ComputeEventClass(group);
                var subsystem = ExtractSubsystem(group);
                var infoObject = ExtractInfoObject(group);

                var userName = cs.CreateUserId.HasValue && usersDict.TryGetValue(cs.CreateUserId.Value, out var login)
                    ? login
                    : !string.IsNullOrEmpty(cs.CreateUserName)
                        ? cs.CreateUserName
                        : "Unknown User";

                result.Add(new CallStackDto
                {
                    Id = cs.Id,
                    CreateAt = cs.CreateAt,
                    CreateUserId = cs.CreateUserId,
                    CreateUserName = cs.CreateUserName,
                    CreateUserFullName = cs.CreateUserFullName,
                    UserName = userName,
                    ActionCount = actionCount,
                    EventClass = eventClass,
                    Subsystem = subsystem,
                    InfoObject = infoObject
                });
            }

            return result;
        }

        private static string ComputeEventClass(List<LogMessage> group)
        {
            if (group.Count == 0) return "Information";
            var max = group.Max(g => g.LogMessageType);
            return max.ToString();
        }

        private static string ExtractSubsystem(List<LogMessage> group)
        {
            var path = group.FirstOrDefault()?.Path ?? string.Empty;
            // Try to take segment after ".Controllers." or ".BusinessLogic."
            var idx = path.IndexOf(".Controllers.");
            if (idx >= 0)
            {
                var rest = path[(idx + ".Controllers.".Length)..];
                var slash = rest.IndexOf('.');
                if (slash > 0) return rest[..slash];
                return rest.Split('=','>')[0];
            }
            idx = path.IndexOf(".BusinessLogic.");
            if (idx >= 0)
            {
                var rest = path[(idx + ".BusinessLogic.".Length)..];
                var slash = rest.IndexOf('.');
                if (slash > 0) return rest[..slash];
                return rest.Split('`','>')[0];
            }
            return string.Empty;
        }

        private static string ExtractInfoObject(List<LogMessage> group)
        {
            var msg = group.FirstOrDefault(g => g.Message?.Length > 0)?.Message ?? string.Empty;
            if (string.IsNullOrWhiteSpace(msg)) return string.Empty;
            const string marker = "with result:";
            var i = msg.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (i > 0)
            {
                return "Result"; // не показываем тяжелый payload, даем краткий маркер
            }
            // Иначе вернем короткий класс.метод из Path
            var path = group.FirstOrDefault()?.Path ?? string.Empty;
            var lastDot = path.LastIndexOf('.');
            if (lastDot >= 0) return path[(lastDot + 1)..];
            return path;
        }
    }
}
