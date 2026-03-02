using System;
using System.Collections.Generic;
using System.Linq;

namespace Masofa.Web.Monolith.ViewModels.FieldPhoto
{
    public class FieldPhotoCreateViewModel
    {
        /// <summary>
        /// Название снимка
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Идентификатор административной единицы (район/город)
        /// </summary>
        public Guid? RegionId { get; set; }

        /// <summary>
        /// Идентификатор родительского региона (область)
        /// </summary>
        public Guid? ParentRegionId { get; set; }

        /// <summary>
        /// Дата и время съемки (UTC)
        /// </summary>
        public DateTime? CaptureDateUtc { get; set; }

        /// <summary>
        /// Описание снимка
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Теги снимка
        /// </summary>
        public IEnumerable<Guid> TagIds { get; set; } = Enumerable.Empty<Guid>();

        /// <summary>
        /// JSON-представление точки для десериализации (WKT формат: "POINT(longitude latitude)")
        /// </summary>
        public string? PointJson { get; set; }
    }
}
