using Masofa.Common.Models;
using Masofa.Common.Models.Identity;

namespace Masofa.Common.ViewModels.Account
{
    public class ProfileInfoViewModel : BaseNamedEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? SecondName { get; set; }
        public bool IsActive { get; set; }
        public bool Approved { get; set; }
        public bool EmailConfirmed { get; set; }
        /// <summary>
        /// Дата и время начала блокировки пользователя
        /// </summary>
        public DateTime? LockoutStart { get; set; }
        
        /// <summary>
        /// Дата и время окончания блокировки пользователя
        /// </summary>
        public DateTime? LockoutEnd { get; set; }
        public UserBusinessType UserBusinessType { get; set; }
        public List<string> Roles { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? DeputyId { get; set; }
        public Guid? UserBusinessId { get; set; }
        public string? Position { get; set; }
        public string? Inn { get; set; }
        public string? Phone { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Comment { get; set; }
        public string? ConnectionBasis { get; set; }
        public DateTime? ScheduledLockoutStart { get; set; }
        public DateTime? ScheduledLockoutEnd { get; set; }
    }
}
