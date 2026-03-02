using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models
{
    /// <summary>
	/// Именование сущностей
	/// </summary>
    public partial class BaseNamedEntity : BaseEntity, IBaseNamedEntity
    {
        public LocalizationString Names { get; set; }
    }
}
