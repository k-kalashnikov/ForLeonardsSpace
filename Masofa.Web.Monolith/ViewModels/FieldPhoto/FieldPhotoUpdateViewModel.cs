using System;
using System.Collections.Generic;
using System.Linq;

namespace Masofa.Web.Monolith.ViewModels.FieldPhoto
{
    public class FieldPhotoUpdateViewModel
    {
        /// <summary>
        /// Уникальный идентификатор снимка
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название снимка
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Описание снимка
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Дата и время съемки (UTC)
        /// </summary>
        public DateTime? CaptureDateUtc { get; set; }

        /// <summary>
        /// Теги снимка
        /// </summary>
        public IEnumerable<Guid> TagIds { get; set; } = Enumerable.Empty<Guid>();
    }
}

