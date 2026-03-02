using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Identity
{
    public class User : IdentityUser<Guid>
    {
        public Guid? ParentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateAt { get; set; } = DateTime.UtcNow;
        public Guid CreateUser { get; set; }
        public Guid LastUpdateUser { get; set; }
        public bool Approved { get; set; }
        public DateTimeOffset? ScheduledLockoutStart { get; set; }
        public DateTimeOffset? ScheduledLockoutEnd { get; set; }
        public UserBusinessType UserBusinessType { get; set; }
        public Guid UserBusinessId { get; set; }
        
        /// <summary>
        /// Комментарий к пользователю
        /// </summary>
        public string? Comment { get; set; }
        
        /// <summary>
        /// Идентификатор заместителя пользователя
        /// </summary>
        public Guid? DeputyId { get; set; }
        
        /// <summary>
        /// Основание подключения
        /// </summary>
        public string? ConnectionBasis { get; set; }

        [NotMapped]
        public bool IsInScheduledLockout
        {
            get
            {
                var now = DateTimeOffset.UtcNow;
                return ScheduledLockoutStart.HasValue
                       && ScheduledLockoutEnd.HasValue
                       && now >= ScheduledLockoutStart.Value
                       && now <= ScheduledLockoutEnd.Value;
            }
        }

        [NotMapped]
        public bool IsActive => (!LockoutEnd.HasValue || LockoutEnd <= DateTimeOffset.UtcNow) && !IsInScheduledLockout;

        [NotMapped]
        public ICollection<UserDevice> UserDevices { get; set; }
    }

    public enum UserBusinessType
    {
        Firm = 0,
        Person = 1
    }
}
