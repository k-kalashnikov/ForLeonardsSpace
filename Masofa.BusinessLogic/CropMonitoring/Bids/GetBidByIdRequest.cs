using MediatR;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Masofa.BusinessLogic.CropMonitoring.Bids
{
    public class GetBidByIdRequest : IRequest<BidGetViewModel>
    {
        public Guid Id { get; init; }
    }
    public class GetBidByIdRequestHandler : IRequestHandler<GetBidByIdRequest, BidGetViewModel>
    {
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; set; }

        public GetBidByIdRequestHandler(
            MasofaCropMonitoringDbContext cm,
            MasofaDictionariesDbContext dict,
            MasofaIdentityDbContext id)
        {
            MasofaCropMonitoringDbContext = cm;
            MasofaDictionariesDbContext = dict;
            MasofaIdentityDbContext = id;
        }

        public async Task<BidGetViewModel> Handle(GetBidByIdRequest request, CancellationToken cancellationToken)
        {
            var bid = await MasofaCropMonitoringDbContext.Bids
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (bid == null) return null;

            // батчем за один объект не критично, но оставим аккуратность
            Crop? crop = null;
            if (bid.CropId != null)
            {
                crop = await MasofaDictionariesDbContext.Crops.AsNoTracking().FirstOrDefaultAsync(c => c.Id == bid.CropId, cancellationToken);
            }

            var users = await MasofaIdentityDbContext.Users.AsNoTracking()
                .Where(u => u.Id == (bid.WorkerId ?? Guid.Empty)
                         || u.Id == (bid.ForemanId ?? Guid.Empty)
                         || u.Id == bid.LastUpdateUser
                         || u.Id == bid.CreateUser)
                .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

            var userIds = users.Keys.ToList();

            var rolesByUser = await MasofaIdentityDbContext.UserRoles
                .AsNoTracking()
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(MasofaIdentityDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .GroupBy(x => x.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Name).ToList(), cancellationToken);

            users.TryGetValue(bid.WorkerId ?? Guid.Empty, out var worker);
            users.TryGetValue(bid.ForemanId ?? Guid.Empty, out var foreman);
            users.TryGetValue(bid.LastUpdateUser, out var lu);
            users.TryGetValue(bid.CreateUser, out var cu);

            rolesByUser.TryGetValue(bid.WorkerId ?? Guid.Empty, out var workerRoles);
            rolesByUser.TryGetValue(bid.ForemanId ?? Guid.Empty, out var foremanRoles);
            rolesByUser.TryGetValue(bid.LastUpdateUser, out var luRoles);
            rolesByUser.TryGetValue(bid.CreateUser, out var cuRoles);

            var region = await MasofaDictionariesDbContext.Regions.AsNoTracking().Where(r => r.Id == bid.RegionId).FirstOrDefaultAsync();
            var field = await MasofaCropMonitoringDbContext.Fields.AsNoTracking().Where(f => f.Id == bid.FieldId).FirstOrDefaultAsync();

            return new BidGetViewModel
            {
                Id = bid.Id,
                Number = bid.Number,
                Crop = crop!,
                StartDate = bid.StartDate,
                EndDate = bid.EndDate,
                Worker = ToProfileVm(worker, workerRoles)!,
                Foreman = ToProfileVm(foreman, foremanRoles)!,
                AnomaliesCount = 0,
                LastUpdateUser = ToProfileVm(lu, luRoles)!,
                CreateUser = ToProfileVm(cu, cuRoles)!,
                BidState = bid.BidState,
                Field = field,
                Region = region,
                FileResultId = (Guid)(bid.FileResultId.HasValue ? bid.FileResultId : Guid.Empty),
                PolygonJson = ToPlygonJson(bid.Polygon)
            };
        }

        private string ToPlygonJson(Polygon polygon)
        {
            var poly = polygon;
            if (poly == null || poly.IsEmpty)
                return null;

            return poly.AsText();
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
}
