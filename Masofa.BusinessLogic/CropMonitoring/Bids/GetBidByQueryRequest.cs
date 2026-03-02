using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Linq.Expressions;

namespace Masofa.BusinessLogic.CropMonitoring.Bids
{
    public class GetBidByQueryRequest : IRequest<List<BidGetViewModel>>
    {
        public BaseGetQuery<Bid> Query { get; init; } = default!;
    }

    public class GetBidByQueryRequestHandler : IRequestHandler<GetBidByQueryRequest, List<BidGetViewModel>>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }

        public GetBidByQueryRequestHandler(
            MasofaCropMonitoringDbContext cm,
            MasofaDictionariesDbContext dict,
            MasofaIdentityDbContext id)
        {
            MasofaCropMonitoringDbContext = cm;
            MasofaDictionariesDbContext = dict;
            MasofaIdentityDbContext = id;
        }

        public async Task<List<BidGetViewModel>> Handle(GetBidByQueryRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Bid> q = MasofaCropMonitoringDbContext.Bids.AsNoTracking().IgnoreAutoIncludes();

            q = ApplyBaseGetQuery(q, request.Query);

            var bids = await q.ToListAsync(cancellationToken);
            if (bids.Count == 0)
            {
                return new List<BidGetViewModel>();
            }

            var cropIds = bids
                .Select(x => x.CropId)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var userIds = bids
                .SelectMany(b => new[] { b.WorkerId, b.ForemanId, b.LastUpdateUser, b.CreateUser })
                .Where(id => id.HasValue && id.Value != Guid.Empty)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var crops = await MasofaDictionariesDbContext.Crops
                .AsNoTracking()
                .Where(c => cropIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c, cancellationToken);

            var users = await MasofaIdentityDbContext.Users
                .AsNoTracking()
                .ToListAsync();

            var rolesByUser = await MasofaIdentityDbContext.UserRoles
                .AsNoTracking()
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(MasofaIdentityDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .GroupBy(x => x.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Name).ToList(), cancellationToken);

            var vms = new List<BidGetViewModel>(bids.Count);

            foreach (var b in bids)
            {
                var worker = MasofaIdentityDbContext.Users.FirstOrDefault(w => w.Id == b.WorkerId);
                var foreman = MasofaIdentityDbContext.Users.FirstOrDefault(f => f.Id == b.ForemanId);
                var lu = MasofaIdentityDbContext.Users.FirstOrDefault(l => l.Id == b.LastUpdateUser);
                var cu = MasofaIdentityDbContext.Users.FirstOrDefault(l => l.Id == b.CreateUser);

                rolesByUser.TryGetValue(b.WorkerId ?? Guid.Empty, out var workerRoles);
                rolesByUser.TryGetValue(b.ForemanId ?? Guid.Empty, out var foremanRoles);
                rolesByUser.TryGetValue(b.LastUpdateUser, out var luRoles);
                rolesByUser.TryGetValue(b.CreateUser, out var cuRoles);

                var workerVm = ToProfileVm(worker, workerRoles);
                var foremanVm = ToProfileVm(foreman, foremanRoles);
                var luVm = ToProfileVm(lu, luRoles);
                var cuVm = ToProfileVm(cu, cuRoles);

                crops.TryGetValue(b.CropId, out var crop);

                var field = await MasofaCropMonitoringDbContext.Fields.AsNoTracking().FirstOrDefaultAsync(f => f.Id == b.FieldId);
                var region = await MasofaDictionariesDbContext.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == b.RegionId);

                vms.Add(new BidGetViewModel
                {
                    Id = b.Id,
                    Number = b.Number,
                    Crop = crop,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Worker = workerVm,
                    Foreman = foremanVm,
                    AnomaliesCount = 0,
                    LastUpdateUser = luVm,
                    CreateUser = cuVm!,
                    BidState = b.BidState,
                    Field = field,
                    FileResultId = (Guid)(b.FileResultId.HasValue ? b.FileResultId : Guid.Empty),
                    Region = region,
                    PolygonJson = ToPlygonJson(b.Polygon)
                });
            }

            return vms;
        }

        private string ToPlygonJson(Polygon polygon)
        {
            var poly = polygon;
            if (poly == null || poly.IsEmpty)
                return null;

            return poly.AsText();
        }

        private static IQueryable<Bid> ApplyBaseGetQuery(IQueryable<Bid> q, BaseGetQuery<Bid> query)
        {

            if (query == null) return q;

            if (query.Filters != null && query.Filters.Count > 0)
            {
                var param = Expression.Parameter(typeof(Bid), "e");
                Expression? body = null;

                foreach (var f in query.Filters)
                {
                    if (string.IsNullOrWhiteSpace(f.FilterField)) continue;

                    var member = Expression.PropertyOrField(param, f.FilterField);
                    var memberType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

                    object? valueObj = ConvertToType(f.FilterValue, memberType);

                    var constExpr = valueObj is null
                        ? Expression.Constant(null, member.Type)
                        : Expression.Constant(valueObj, memberType);

                    Expression left = member;
                    if (member.Type != constExpr.Type && constExpr.Type == memberType)
                        left = Expression.Convert(member, memberType);

                    Expression? predicate = null;
                    var op = f.FilterOperator ?? FilterOperator.Equals;

                    if (memberType == typeof(string) &&
                        (op == FilterOperator.Contains || op == FilterOperator.StartsWith || op == FilterOperator.EndsWith))
                    {
                        // e.Field != null && e.Field.{Contains|StartsWith|EndsWith}(value)
                        var notNull = Expression.NotEqual(member, Expression.Constant(null, member.Type));
                        var method = typeof(string).GetMethod(op.ToString(), new[] { typeof(string) })!;
                        var call = Expression.Call(member, method, Expression.Convert(constExpr, typeof(string)));
                        predicate = Expression.AndAlso(notNull, call);
                    }
                    else
                    {
                        switch (op)
                        {
                            case FilterOperator.Equals:
                                predicate = Expression.Equal(left, constExpr);
                                break;
                            case FilterOperator.NotEquals:
                                predicate = Expression.NotEqual(left, constExpr);
                                break;
                            case FilterOperator.GreaterThan:
                                predicate = Expression.GreaterThan(left, constExpr);
                                break;
                            case FilterOperator.GreaterThanOrEqual:
                                predicate = Expression.GreaterThanOrEqual(left, constExpr);
                                break;
                            case FilterOperator.LessThan:
                                predicate = Expression.LessThan(left, constExpr);
                                break;
                            case FilterOperator.LessThanOrEqual:
                                predicate = Expression.LessThanOrEqual(left, constExpr);
                                break;
                            default:
                                // Для нестроковых Contains/StartsWith/EndsWith — игнор или Equals
                                predicate = Expression.Equal(left, constExpr);
                                break;
                        }
                    }

                    if (predicate != null)
                        body = body == null ? predicate : Expression.AndAlso(body, predicate);
                }

                if (body != null)
                {
                    var lambda = Expression.Lambda<Func<Bid, bool>>(body, param);
                    q = q.Where(lambda);
                }
            }

            // 2) Сортировка
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var param = Expression.Parameter(typeof(Bid), "e");
                var member = Expression.PropertyOrField(param, query.SortBy);
                var keySelector = Expression.Lambda(member, param);

                var methodName = query.Sort == SortType.DSC ? "OrderByDescending" : "OrderBy";
                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2);

                var generic = method.MakeGenericMethod(typeof(Bid), member.Type);
                q = (IQueryable<Bid>)generic.Invoke(null, new object[] { q, keySelector })!;
            }

            // 3) Пагинация
            if (query.Offset > 0) q = q.Skip(query.Offset);
            if (query.Take.HasValue) q = q.Take(query.Take.Value);

            return q;
        }

        private static object? ConvertToType(object? value, Type target)
        {
            if (value == null) return null;

            if (value is Newtonsoft.Json.Linq.JToken jt)
                return jt.ToObject(target);

            var t = Nullable.GetUnderlyingType(target) ?? target;

            if (t == typeof(string)) return value.ToString();
            if (t == typeof(Guid)) return value is Guid g ? g : Guid.Parse(value.ToString()!);
            if (t == typeof(DateTime))
            {
                if (value is DateTime dt) return dt;
                return DateTime.Parse(value.ToString()!, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            }
            if (t == typeof(DateOnly))
            {
                if (value is DateOnly d) return d;
                if (value is DateTime dd) return DateOnly.FromDateTime(dd);
                return DateOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
            }
            if (t.IsEnum) return Enum.Parse(t, value.ToString()!, true);

            return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }

        private static ProfileInfoViewModel? ToProfileVm(User? u, List<string>? roles)
        {
            if (u == null) return null;
            return new ProfileInfoViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserName = u.UserName,
                Roles = roles ?? new List<string>()
            };
        }
    }

    public class BidGetViewModel
    {
        public Guid Id { get; set; }

        public long Number { get; set; }

        public Masofa.Common.Models.Dictionaries.Crop Crop { get; set; } = default!;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Masofa.Common.ViewModels.Account.ProfileInfoViewModel Worker { get; set; } = default!;

        public Masofa.Common.ViewModels.Account.ProfileInfoViewModel Foreman { get; set; } = default!;

        public int AnomaliesCount { get; set; }

        public Masofa.Common.ViewModels.Account.ProfileInfoViewModel LastUpdateUser { get; set; } = default!;

        public Masofa.Common.ViewModels.Account.ProfileInfoViewModel CreateUser { get; set; } = default!;

        public BidStateType BidState { get; set; }

        public Region Region { get; set; }

        public Field Field { get; set; }

        public Guid FileResultId { get; set; }

        public string? PolygonJson { get; set; }
    }
}
