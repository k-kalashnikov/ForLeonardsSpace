using Masofa.BusinessLogic.CropMonitoring.Bids;
using Masofa.Common.Models;
using Masofa.Common.Models.Notifications;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Masofa.BusinessLogic.Common.UserTickets
{
    public class GetMessagesByQueryRequest : IRequest<List<UserTicketMessage>>
    {
        public MessageGetQuery<UserTicketMessage> Query { get; set; } = default!;
        //public bool? OnlyWithEmailId { get; set; } = null;
    }

    public class GetMessagesByQueryHandler : IRequestHandler<GetMessagesByQueryRequest, List<UserTicketMessage>>
    {
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }

        public GetMessagesByQueryHandler(MasofaCommonDbContext masofaCommonDbContext)
        {
            MasofaCommonDbContext = masofaCommonDbContext;
        }

        public async Task<List<UserTicketMessage>> Handle(GetMessagesByQueryRequest request, CancellationToken cancellationToken)
        {
            IQueryable<UserTicketMessage> resultQuery = MasofaCommonDbContext.UserTicketMessages.AsNoTracking();

            if (request.Query.Filters == null || !request.Query.Filters.Any())
            {
                throw new ValidationException("Message query must contain at least one filter.");
            }

            var hasTicketFilter = request.Query.Filters.Any(f =>
                string.Equals(f.FilterField, nameof(UserTicketMessage.UserTicketId), StringComparison.OrdinalIgnoreCase));

            if (!hasTicketFilter)
            {
                throw new ValidationException("Message query must include a filter for UserTicketId.");
            }

            if (request.Query.Filters.Any())
            {
                foreach (var item in request.Query.Filters)
                {
                    resultQuery = resultQuery.ApplyFiltering(item);
                }
            }

            if (!string.IsNullOrEmpty(request.Query.SortBy))
            {
                resultQuery = resultQuery.ApplyOrdering(request.Query.SortBy, request.Query.Sort);
            }

            if (request.Query.Take.HasValue)
            {
                resultQuery = resultQuery
                    .Skip(request.Query.Offset)
                    .Take(request.Query.Take.Value);
            }

            var ticketMessages = await resultQuery.ToListAsync(cancellationToken);

            foreach (var m in ticketMessages)
            {
                try
                {
                    m.AttachmentIds = string.IsNullOrWhiteSpace(m.AttachmentIdsJson) ? new List<Guid>() : (JsonConvert.DeserializeObject<List<Guid>>(m.AttachmentIdsJson) ?? new List<Guid>());
                }
                catch
                {
                    m.AttachmentIds = new List<Guid>();
                }
            }

            return ticketMessages;
        }
    }

    public class MessageGetQuery<TModel>
    {
        public int? Take { get; set; }
        public int Offset { get; set; } = 0;
        public string? SortBy { get; set; }
        public SortType Sort { get; set; } = SortType.ASC;

        public List<FieldFilter> Filters
        {
            get
            {
                return _filters;
            }
            set
            {
                _filters = FixFiltersNames(value);
            }
        }

        private List<FieldFilter> _filters = new List<FieldFilter>();

        public (bool, string) FilterIsValid()
        {
            foreach (var item in Filters)
            {
                var field = typeof(TModel).GetRuntimeProperties()
                    .FirstOrDefault(m => m.Name.Equals(item.FilterField));

                if (field == null)
                {
                    return (false, $"Field with name {item.FilterField} is not exist");
                }

                if (field.PropertyType != typeof(string))
                {
                    try
                    {
                        var temp = JsonConvert.DeserializeObject(item.FilterValue.ToString(), field.PropertyType);
                    }
                    catch (Exception e)
                    {
                        return (false, $"Can't convert {item.FilterValue} to {field.PropertyType.Name}");
                    }
                }
            }
            return (true, string.Empty);
        }

        private List<FieldFilter> FixFiltersNames(List<FieldFilter> fieldFilters)
        {
            var result = new List<FieldFilter>();

            foreach (var item in fieldFilters)
            {
                var field = typeof(TModel).GetRuntimeProperties()
                    .FirstOrDefault(m => m.Name.ToLower().Equals(item.FilterField.ToLower()));

                if (field == null)
                {
                    throw new ArgumentException($"Field with name {item.FilterField} is not exist", nameof(Filters));
                }

                result.Add(new FieldFilter()
                {
                    FilterField = field.Name,
                    FilterOperator = item.FilterOperator,
                    FilterValue = item.FilterValue
                });
            }

            return result;
        }
    }
}
