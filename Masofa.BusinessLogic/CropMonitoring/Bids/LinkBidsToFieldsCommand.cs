using MediatR;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Masofa.Common.Models.CropMonitoring;

namespace Masofa.BusinessLogic.CropMonitoring.Bids
{
    /// <summary>
    /// Команда для привязки заявок к полям по координатам
    /// </summary>
    public class LinkBidsToFieldsCommand : IRequest<LinkBidsToFieldsResult>
    {
    }

    /// <summary>
    /// Результат привязки заявок к полям
    /// </summary>
    public class LinkBidsToFieldsResult
    {
        /// <summary>
        /// Количество обработанных заявок
        /// </summary>
        public int ProcessedBids { get; set; }

        /// <summary>
        /// Количество успешно привязанных заявок
        /// </summary>
        public int LinkedBids { get; set; }

        /// <summary>
        /// Количество заявок, для которых не найдено поле
        /// </summary>
        public int UnlinkedBids { get; set; }

        /// <summary>
        /// Детали по каждой обработанной заявке
        /// </summary>
        public List<BidLinkDetail> Details { get; set; } = new List<BidLinkDetail>();
    }

    /// <summary>
    /// Детали привязки заявки к полю
    /// </summary>
    public class BidLinkDetail
    {
        /// <summary>
        /// ID заявки
        /// </summary>
        public Guid BidId { get; set; }

        /// <summary>
        /// Номер заявки
        /// </summary>
        public long BidNumber { get; set; }

        /// <summary>
        /// Координаты заявки (Lat, Lng)
        /// </summary>
        public string Coordinates { get; set; } = string.Empty;

        /// <summary>
        /// ID найденного поля
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Название найденного поля
        /// </summary>
        public string? FieldName { get; set; }

        /// <summary>
        /// Статус привязки
        /// </summary>
        public BidLinkStatus Status { get; set; }

        /// <summary>
        /// Сообщение о результате
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Статус привязки заявки
    /// </summary>
    public enum BidLinkStatus
    {
        /// <summary>
        /// Успешно привязана
        /// </summary>
        Linked = 1,

        /// <summary>
        /// Не найдено поле
        /// </summary>
        FieldNotFound = 2,

        /// <summary>
        /// Ошибка при обработке
        /// </summary>
        Error = 3
    }

    /// <summary>
    /// Результат поиска поля
    /// </summary>
    public class FieldSearchResult
    {
        /// <summary>
        /// Найденное поле
        /// </summary>
        public Field? Field { get; set; }

        /// <summary>
        /// Сообщение о результате поиска
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Обработчик команды для привязки заявок к полям по координатам
    /// </summary>
    public class LinkBidsToFieldsCommandHandler : IRequestHandler<LinkBidsToFieldsCommand, LinkBidsToFieldsResult>
    {
        private readonly MasofaCropMonitoringDbContext _context;
        private readonly MasofaDictionariesDbContext _dictionariesContext;

        public LinkBidsToFieldsCommandHandler(
            MasofaCropMonitoringDbContext context,
            MasofaDictionariesDbContext dictionariesContext)
        {
            _context = context;
            _dictionariesContext = dictionariesContext;
        }

        public async Task<LinkBidsToFieldsResult> Handle(LinkBidsToFieldsCommand request, CancellationToken cancellationToken)
        {
            var result = new LinkBidsToFieldsResult();

            try
            {
                // Получаем все заявки без привязанного поля
                var bidsWithoutField = await _context.Bids
                    .Where(b => b.FieldId == null && b.Lat != 0 && b.Lng != 0)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                result.ProcessedBids = bidsWithoutField.Count;

                foreach (var bid in bidsWithoutField)
                {
                    var detail = new BidLinkDetail
                    {
                        BidId = bid.Id,
                        BidNumber = bid.Number,
                        Coordinates = $"{bid.Lat}, {bid.Lng}"
                    };

                    try
                    {
                        // Создаем точку из координат заявки
                        var point = new Point(bid.Lng, bid.Lat) { SRID = 4326 }; // WGS84

                        // Ищем поле, которое содержит эту точку
                        var fieldSearchResult = await FindFieldByPointAsync(point, cancellationToken);

                        if (fieldSearchResult.Field != null)
                        {
                            // Привязываем заявку к полю
                            bid.FieldId = fieldSearchResult.Field.Id;
                            _context.Bids.Update(bid);

                            detail.FieldId = fieldSearchResult.Field.Id;
                            detail.FieldName = fieldSearchResult.Field.Name;
                            detail.Status = BidLinkStatus.Linked;
                            detail.Message = fieldSearchResult.Message;
                            
                            result.LinkedBids++;
                        }
                        else
                        {
                            detail.Status = BidLinkStatus.FieldNotFound;
                            detail.Message = "Поле не найдено ни в одном регионе";
                            result.UnlinkedBids++;
                        }
                    }
                    catch (Exception ex)
                    {
                        detail.Status = BidLinkStatus.Error;
                        detail.Message = $"Ошибка: {ex.Message}";
                        result.UnlinkedBids++;
                    }

                    result.Details.Add(detail);
                }

                // Сохраняем изменения в базе данных
                if (result.LinkedBids > 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Логируем общую ошибку
                throw new Exception($"Ошибка при привязке заявок к полям: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Ищет поле, которое содержит указанную точку
        /// </summary>
        private async Task<FieldSearchResult> FindFieldByPointAsync(Point point, CancellationToken cancellationToken)
        {
            // Простая логика: ищем все поля, которые содержат эту точку
            var field = await _context.Fields
                .Where(f => f.Polygon != null && 
                           f.Polygon.Contains(point) &&
                           f.Status == Masofa.Common.Models.StatusType.Active)
                .FirstOrDefaultAsync(cancellationToken);

            if (field != null)
            {
                return new FieldSearchResult
                {
                    Field = field,
                    Message = $"Найдено поле '{field.Name}' - точка входит в полигон поля"
                };
            }

            return new FieldSearchResult
            {
                Field = null,
                Message = "Поле не найдено - точка не входит ни в один полигон активного поля"
            };
        }
    }
}
