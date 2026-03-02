using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Logging.Queries
{
    public class GetLogMessagesByCallStackQuery : IRequest<List<LogMessageDto>>
    {
        public Guid Id { get; set; }
    }

    public class LogMessageDto
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string Message { get; set; }
        public string LogLevelType { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
    }

    public sealed class GetLogMessagesByCallStackHandler : IRequestHandler<GetLogMessagesByCallStackQuery, List<LogMessageDto>>
    {
        private readonly MasofaCommonDbContext _db;

        public GetLogMessagesByCallStackHandler(MasofaCommonDbContext db)
        {
            _db = db;
        }

        public async Task<List<LogMessageDto>> Handle(GetLogMessagesByCallStackQuery request, CancellationToken cancellationToken)
        {
            var list = await _db.LogMessages
                .Where(m => m.CallStackId == request.Id)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.CreateAt)
                .ToListAsync(cancellationToken);

            return list.Select(m => new LogMessageDto
            {
                Id = m.Id,
                CreateAt = m.CreateAt,
                Message = m.Message,
                LogLevelType = m.LogMessageType.ToString(),
                Path = m.Path,
                Order = m.Order
            }).ToList();
        }
    }
}


