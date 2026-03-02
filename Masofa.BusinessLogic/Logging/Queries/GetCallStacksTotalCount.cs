using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Logging.Queries
{
    public class GetCallStacksTotalCountQuery : IRequest<int>
    {
        public List<FilterItem>? Filters { get; set; }
    }

    public sealed class GetCallStacksTotalCountHandler : IRequestHandler<GetCallStacksTotalCountQuery, int>
    {
        private readonly MasofaCommonDbContext _db;
        private readonly MasofaIdentityDbContext _identityDb;

        public GetCallStacksTotalCountHandler(MasofaCommonDbContext db, MasofaIdentityDbContext identityDb)
        {
            _db = db;
            _identityDb = identityDb;
        }

        public async Task<int> Handle(GetCallStacksTotalCountQuery request, CancellationToken cancellationToken)
        {
            var q = _db.CallStacks.AsQueryable();

            if (request.Filters != null)
            {
                foreach (var f in request.Filters)
                {
                    if (string.Equals(f.Name, "timestamp", StringComparison.OrdinalIgnoreCase))
                    {
                        if (f.Value is Newtonsoft.Json.Linq.JArray jArray && jArray.Count == 2)
                        {
                            if (DateTime.TryParse(jArray[0]?.ToString(), out var start) && 
                                DateTime.TryParse(jArray[1]?.ToString(), out var end))
                            {
                                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                                end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
                                
                                q = q.Where(x => x.CreateAt >= start && x.CreateAt <= end);
                            }
                        }
                        else if (f.Value is DateTime dt)
                        {
                            var start = dt.Date; var end = start.AddDays(1);
                            q = q.Where(x => x.CreateAt >= start && x.CreateAt < end);
                        }
                        else if (f.Value is IEnumerable<DateTime> range)
                        {
                            var arr = range.Take(2).ToArray();
                            if (arr.Length == 2)
                            {
                                q = q.Where(x => x.CreateAt >= arr[0] && x.CreateAt <= arr[1]);
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

            return await q.CountAsync(cancellationToken);
        }
    }
}


