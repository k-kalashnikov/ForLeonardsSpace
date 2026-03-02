using Microsoft.AspNetCore.Identity;

namespace Masofa.Common.Models.Identity
{
    public partial class Role : IdentityRole<Guid>, IBaseNamedEntity
    {
        public LocalizationString Names { get; set; }
        public LocalizationString Descriptions { get; set; }
    }
}
